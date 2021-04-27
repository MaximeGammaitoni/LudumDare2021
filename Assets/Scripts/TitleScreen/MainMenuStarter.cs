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
    public Button CloseLeaderBoard;

    public List<Text> StarterTexts;
    List<string> StarterTextsContents;
    GameObject TitleScreenTextGO;
    GameObject TitleGO;
    GameObject MusicMenu;
    public Image Black;

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
    const string BaseUrl = "http://51.91.99.249"; //If you get this url to add your score manually, the Lord of the hell Paimon will come to you cheaty boy.
    const string GetAllEndPoint = "/leader_board/";
    public bool isLeaderBoardOpen;
    private List<GameObject> listofLines = new List<GameObject>();

    void Start()
    {
        BlackFadeIn(false);
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
        //StartCoroutine(LoadScene());

        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.MenuNavigation.Movement.started += OnAxesChanged;
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

    public void LeaderOpen()
    {
        isLeaderBoardOpen = !isLeaderBoardOpen;
    }

    private void Update()
    {
        // (EventSystem.current)
        if ( isLeaderBoardOpen)
        {
            CloseLeaderBoard.Select();
        }
    }

    public void SelectPlay()
    {
        Play.Select();
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

    

    public void LaunchGame()
    {
        Play.interactable = false;
        Ladder.interactable = false;
        Quit.interactable = false;
        StartCoroutine(LoadGameCo());
    }

    IEnumerator LoadGameCo()
    {
        yield return BlackFadeInCo(true);
        yield return new WaitForSeconds(0.5f);
        yield return LoadScene();
    }

    public void ShowLeaderBoard()
    {
        LeaderBoard.SetActive(true);
        GetRequestAndInstantiateIntoCanvas();
        Debug.Log("ShowingLeaderBoard");
    }

    public void HideLeaderBoard()
    {
        LeaderBoard.SetActive(false);
        GetRequestAndInstantiateIntoCanvas();
        Debug.Log("ShowingLeaderBoard");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
        Debug.Log("Quitting");
    }

    IEnumerator LoadScene()
    {
        Debug.Log("Started Loading");
        sceneLoader = SceneManager.LoadSceneAsync(1);
        yield return sceneLoader;
        Debug.Log("Scene loaded");
    }

    public void GetRequestAndInstantiateIntoCanvas()
    {
        StartCoroutine(LeaderBoardManager.GetRequestAndInstantiateIntoCanvasCorout(LeaderBoardContent, LinePrefab));
    }

    public void DestroyLeaderBoardComponents()
    {
        foreach(GameObject element in listofLines)
        {
            Destroy(element);
        }
    }

    private void BlackFadeIn(bool value)
    {
        StartCoroutine(BlackFadeInCo(value));
    }

    IEnumerator BlackFadeInCo(bool value)
    {
        float fadeTime = 0.5f;
        float t = 0f;
        Color from, to;

        if (value)
        {
            from = Color.clear;
            to = Color.black;
        }
        else
        {
            to = Color.clear;
            from = Color.black;
        }

        while (t < fadeTime)
        {
            Black.color = Color.Lerp(from, to, t / fadeTime);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        Black.color = to;
        yield return new WaitForSeconds(0.25f);
    }
}
