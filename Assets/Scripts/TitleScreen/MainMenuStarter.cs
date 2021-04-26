using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

using static UnityEngine.InputSystem.InputAction;
using UnityEngine.Networking;



public class MainMenuStarter : MonoBehaviour
{
    public Image TwinGearsLogo;
    public TextAnimatorPlayer TextAnimator;
    public GameObject LeaderBoard;
    public GameObject MainMenu;

    public Button Play;
    public Button Ladder;
    public Button Quit;

    public List<Text> StarterTexts;
    List<string> StarterTextsContents;
    GameObject TitleScreenTextGO;
    GameObject TitleGO;
    GameObject MusicMenu;

    public GameObject LinePrefab;
    public GameObject LeaderBoardContent;

    AudioSource MenuMusic;
    float MusicVolumIncremantor = 0.02f;
    float MusicVolumLimit = 0.1f;
    bool canStart=false;
    [HideInInspector] public bool musicIsSelected = false;
    private CanvasGroup menuCanvas;
    private float fadeInSpeed = 2;
    private bool _isReadyToLaunch;
    private bool isLaunched;
    private AsyncOperation sceneLoader;
    private static bool hasSeenSplash = false;
    private PlayerControls playerControls;
    private Vector2 direction;
    private int selectedButton;
    [SerializeField] public List<Button> menuButtons = new List<Button>();
    private LeaderBoardManager leaderBoardManager;
    const string BaseUrl = "http://51.91.99.249"; //If you get this url to add your score manually, the Lord of the hell Paimon will come to you cheaty boy.
    const string GetAllEndPoint = "/leader_board/";


    void Start()
    {
        menuCanvas = MainMenu.GetComponent<CanvasGroup>();
        InitializeMenu();

        if (!hasSeenSplash)
        {
            StartCoroutine(StartMainMenuCorout());
        }
        else
        {
            MainMenu.SetActive(true);
            menuCanvas.alpha = 1;
        }
        StartCoroutine(LoadScene());

        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.MenuNavigation.Movement.started += OnAxesChanged;

        leaderBoardManager = new LeaderBoardManager();
    }

    private void OnAxesChanged(CallbackContext ctx)
    {

        if (ctx.started)
        {
            direction = ctx.ReadValue<Vector2>();
            //Debug.Log(direction);
            if(direction.y >= 0.3f)
            {
                //SelectButton(-1);
            }
            else if (direction.y <= -0.3f)
            {
                //SelectButton(1);
            }
            
        }
        else if (ctx.canceled)
        {
            direction = Vector2.zero;
        }
        Debug.Log("selectedButtonIndex " + selectedButton);
    }

    private void InitializeMenu()
    {
        menuButtons.Add(Play);
        menuButtons.Add(Ladder);
        menuButtons.Add(Quit);
        //Play.Select();
        selectedButton = 0;
    }

    private void FixedUpdate()
    {
        // (EventSystem.current)

    }

    private void SelectButton(int number)
    {
        Debug.Log("initial selectButton is " + selectedButton);
        selectedButton += number;
        Debug.Log("Select updated " + selectedButton);
        if (selectedButton < 0)
        {
            selectedButton = 0;
        }
        if(selectedButton > 2)
        {
            selectedButton = 2;
        }
        Debug.Log("Correction " + selectedButton);


        //menuButtons[selectedButton].Select();
        Debug.Log(menuButtons[selectedButton].gameObject.name);
    }

    IEnumerator StartMainMenuCorout()
    {
        TwinGearsLogo.CrossFadeAlpha(0, 0, true);

        yield return new WaitForSeconds(0.8f);
        TwinGearsLogo.CrossFadeAlpha(1, 1.5f, true);
        yield return new WaitForSeconds(1.5f);

        var targetLogoPos = new Vector2(0, 180);
        float interpolation = 0;
        while (TwinGearsLogo.rectTransform.anchoredPosition.y < targetLogoPos.y-1)
        {
            interpolation += Time.deltaTime*0.2f;
            TwinGearsLogo.rectTransform.anchoredPosition = Vector2.Lerp(TwinGearsLogo.rectTransform.anchoredPosition, targetLogoPos, interpolation);
            yield return null;
        }
        Debug.Log("start showing text ...");
        TextAnimator.ShowText("Developers :\n \n <pend> Jean-Philippe Chevalier-Lancioni \n Josselin Morau \n Maxime Gammaitoni \n John Touba </pend>\n ");
        TextAnimator.StartShowingText();
        yield return new WaitForSeconds(7.2f);
        TextAnimator.ShowText("");
        TextAnimator.ShowText("Art :\n \n <pend>Mathieu Stryzkala</pend>\n \n Music : \n \n <pend>Alexis \"Late Night\" Imperial</pend>");
        yield return new WaitForSeconds(8.2f);
        TextAnimator.ShowText("");
        yield return new WaitForSeconds(1);
        TwinGearsLogo.CrossFadeAlpha(0, 0.5f, true);
        MainMenu.SetActive(true);
        hasSeenSplash = true;
        StartCoroutine(MainMenuFadeInCoroutine());
    }

    IEnumerator MainMenuFadeInCoroutine()
    {
        while (menuCanvas.alpha <0.99)
        {
            menuCanvas.alpha += fadeInSpeed * Time.deltaTime;
            yield return null;
        }
        menuCanvas.alpha = 1;
        //InitializeMenu();
    }

    private void Update()
    {
        //Debug.Log("scene loader is null: " + sceneLoader);
        //Debug.Log(selectedButton);
    }

    public void LaunchGame()
    {

        sceneLoader.allowSceneActivation = true;
        isLaunched = true;

    }

    public void ShowLeaderBoard()
    {
        LeaderBoard.SetActive(true);
        GetRequestAndInstantiateIntoCanvas();
        Debug.Log("ShowingLeaderBoard");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }

    IEnumerator LoadScene()
    {
        Debug.Log("Started Loading");
        sceneLoader = SceneManager.LoadSceneAsync(1);
        sceneLoader.allowSceneActivation = false;
        while (sceneLoader.progress < 0.95)
        {
            //Debug.Log(sceneLoader.progress);
            yield return null;
        }
        Debug.Log("sceneLoader is rdy");
        _isReadyToLaunch = true;
        yield return null;

    }

    public void GetRequestAndInstantiateIntoCanvas()
    {
        StartCoroutine(GetRequestAndInstantiateIntoCanvasCorout());
    }
    IEnumerator GetRequestAndInstantiateIntoCanvasCorout()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(BaseUrl + GetAllEndPoint))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                GetLeadBoardResult leadBoardData = JsonUtility.FromJson<GetLeadBoardResult>(webRequest.downloadHandler.text);
                int i = 1;
                leadBoardData.lead_boards.OrderByDescending(x => x.score);
                foreach (LeadBoardData lb in leadBoardData.lead_boards)
                {
                    var Go = Instantiate(LinePrefab);
                    Go.transform.SetParent(LeaderBoardContent.transform, false);
                    var CurrentLine = Go.GetComponent<LeaderBoardUIElement>();

                    CurrentLine.Position.SetText(i.ToString());
                    CurrentLine.Name.SetText(lb.name);
                    var ts = TimeSpan.FromSeconds(lb.score);
                    var formatTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            ts.Hours,
                            ts.Minutes,
                            ts.Seconds);
                    CurrentLine.Score.SetText(formatTime);
                    i++;
                }
            }
        }
    }
}
