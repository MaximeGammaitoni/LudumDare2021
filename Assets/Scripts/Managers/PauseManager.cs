using States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;


public class PauseManager : IManager
{
    public PlayerControls playerControls;
    private Vector2 direction;
    private Button QuitButton;
    private Button ResumeButton;
    private Button SelectedButton;

    public PauseManager()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.Main.Pause.started += OnPausePressed;
        EventsManager.StartListening(nameof(LevelLoader.OnGameFinished), DisableWASD);

        QuitButton = GameManager.singleton.ResourcesLoaderManager.CanvasElements.QuitGameButton;
        ResumeButton = GameManager.singleton.ResourcesLoaderManager.CanvasElements.ResumeGameButton;
        EventsManager.StartListening(nameof(GameManager.singleton.StatesEvents.OnPauseIn), StartPause);
        EventsManager.StartListening(nameof(GameManager.singleton.StatesEvents.OnPauseOut), QuitPause);
        
        playerControls.MenuNavigation.Movement.started += OnAxesChanged;
        GameManager.singleton.ResourcesLoaderManager.CanvasElements.PausePanel.SetActive(false);
        ResumeButton.onClick.AddListener(delegate {PauseGame();});
        QuitButton.onClick.AddListener(delegate { QuitGame();});
    }

    ~PauseManager()
    {
        Debug.Log("GOODBYE !");
        // playerControls.Disable();
        // playerControls.Dispose();
        // EventsManager.StopListening(nameof(LevelLoader.OnGameFinished), DisableWASD);
        // EventsManager.StopListening(nameof(GameManager.singleton.StatesEvents.OnPauseIn), StartPause);
        // EventsManager.StopListening(nameof(GameManager.singleton.StatesEvents.OnPauseOut), QuitPause);
    }

    public void Destroy()
    {
        Debug.Log("Destroy PauseManager");
        playerControls.Main.Pause.started -= OnPausePressed;
        playerControls.Disable();
        playerControls.Dispose();
        EventsManager.StopListening(nameof(LevelLoader.OnGameFinished), DisableWASD);
        EventsManager.StopListening(nameof(GameManager.singleton.StatesEvents.OnPauseIn), StartPause);
        EventsManager.StopListening(nameof(GameManager.singleton.StatesEvents.OnPauseOut), QuitPause);
    }

    void OnPausePressed(CallbackContext ctx)
    {
        Debug.Log("Pause ?");
        PauseGame();
    }

    void DisableWASD(Args args = null)
    {
        playerControls.MenuNavigation.Movement.Disable();
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
        //Application.Quit();
        SceneManager.LoadScene(0);
    }
    private void PauseGame()
    {
        if(GameManager.singleton.StatesManager.CurrentState is Pause)
        {
            GameManager.singleton.StatesManager.CurrentState = new Run();
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
            SelectedButton = null;
            return;
        }
        else if (GameManager.singleton.StatesManager.CurrentState.ElementsCanMove)
        {
            GameManager.singleton.StatesManager.CurrentState = new Pause();
        }
    }

    private void StartPause(Args args)
    {
        Time.timeScale = 0f;
        GameManager.singleton.ResourcesLoaderManager.CanvasElements.PausePanel.SetActive(true);
    }
    private void QuitPause(Args args)
    {
        Time.timeScale = 1f;
        GameManager.singleton.ResourcesLoaderManager.CanvasElements.PausePanel.SetActive(false);
    }
    private void OnAxesChanged(CallbackContext ctx)
    {
        if (ctx.started)
        {
            direction = ctx.ReadValue<Vector2>();
            if(direction.y >= 0f || direction.y <= -0f)
            {
                SelectButton();
            }
        }
        else if (ctx.canceled)
        {
            direction = Vector2.zero;
        }
    }
    private void SelectButton()
    {
        if (SelectedButton == null)
        {
            SelectedButton = ResumeButton;
            ResumeButton.Select();
        }
        else if(SelectedButton == ResumeButton)
        {
            SelectedButton = QuitButton;
            QuitButton.Select();
        }
        SelectedButton = ResumeButton;
        ResumeButton.Select();
    }
}
