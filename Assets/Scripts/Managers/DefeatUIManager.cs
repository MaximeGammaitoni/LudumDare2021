using UnityEngine;
using UnityEngine.UI;

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
        Application.Quit();
    }

    private void Retry()
    {
        DefeatPanel.SetActive(false);
        GameManager.singleton.StatesManager.CurrentState = new States.Run();
    }

    private void MainMenu()
    {

    }

    private void DefeateAction()
    {
        DefeatPanel.SetActive(true);
    }
}
