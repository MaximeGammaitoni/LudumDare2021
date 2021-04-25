using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuStarter : MonoBehaviour
{
    public Image TwinGearsLogo;
    public TextAnimatorPlayer TextAnimator;
    public List<Text> StarterTexts;
    List<string> StarterTextsContents;
    GameObject TitleScreenTextGO;
    GameObject TitleGO;
    GameObject MusicMenu;
    AudioSource MenuMusic;
    float MusicVolumIncremantor = 0.02f;
    float MusicVolumLimit = 0.1f;
    bool canStart=false;
    [HideInInspector] public bool musicIsSelected = false;
    void Start()
    { 
        StartCoroutine(StartMainMenuCorout());
    }

    IEnumerator StartMusicVolumUpper()
    {
        while (MenuMusic.volume <= MusicVolumLimit)
        {
            MenuMusic.volume += MusicVolumIncremantor;
            yield return new WaitForSeconds(0.5f);
        }
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
        TextAnimator.ShowText("Developers :\n \n <pend> JP \n Jojo \n Maxime Gammaitoni \n John Touba </pend>\n ");
        TextAnimator.StartShowingText();
        yield return new WaitForSeconds(7.2f);
        TextAnimator.ShowText("");
        TextAnimator.ShowText("Art :\n \n <pend>Mathieu Stryzkala</pend>\n \n Music : \n \n <pend>Alexis \"Late Night\" Imperial</pend>");
        yield return new WaitForSeconds(8.2f);
        TextAnimator.ShowText("");
        yield return new WaitForSeconds(1);
        TwinGearsLogo.CrossFadeAlpha(0, 0.5f, true);
    }

    private void Update()
    {
        if (canStart && Input.anyKeyDown)
        {
           this.LoadMusicMenu();
            canStart = false;
        }
        if (musicIsSelected)
        {
            StartCoroutine(NowLoading());
        }
    }

    private void LoadMusicMenu()
    {
        GameObject musicMenu = Instantiate(MusicMenu, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        musicMenu.transform.SetParent(GameObject.Find("Canvas").transform, false);
        TitleScreenTextGO.SetActive(false);
        TitleGO.SetActive(false);
    }
    IEnumerator NowLoading()
    {
        canStart = false;
        MenuMusic.Stop();
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync("Game");
    }
}
