using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DefeatUIManager
{
    private GameObject DefeatPanel;
    private Button RetryButton;
    private Button QuitButton;
    private Button MainMenuButton;
    public DefeatUIManager()
    {
        DefeatPanel = GameManager.singleton.ResourcesLoaderManager.CanvasElements.DefeatPanel;
        RetryButton = GameManager.singleton.ResourcesLoaderManager.CanvasElements.DefeatPanelRetryButton;
        QuitButton = GameManager.singleton.ResourcesLoaderManager.CanvasElements.DefeatPanelQuitButton;
        MainMenuButton = GameManager.singleton.ResourcesLoaderManager.CanvasElements.DefeatPanelMainMenuButton;
        GameManager.DeafetUiHandler += DefeateAction;
        RetryButton.onClick.AddListener(delegate { Retry(); });
        QuitButton.onClick.AddListener(delegate { Quit(); });
        MainMenuButton.onClick.AddListener(delegate { MainMenu(); });
    }

    private void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void Retry()
    {
        DefeatPanel.SetActive(false);
        GameManager.singleton.PauseManager.playerControls.Disable();
        GameManager.singleton.DestroyServices();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void DefeateAction()
    {
        DefeatPanel.SetActive(true);
        RetryButton.Select();
    }
}
