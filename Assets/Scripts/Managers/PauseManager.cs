using States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class PauseManager
{
    private PlayerControls playerControls;
    private Vector2 direction;
    private Button QuitButton;
    private Button ResumeButton;
    private Button SelectedButton;

    public PauseManager()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.Main.Pause.performed += cb => PauseGame();

        QuitButton = GameManager.singleton.ResourcesLoaderManager.CanvasElements.QuitGameButton;
        ResumeButton = GameManager.singleton.ResourcesLoaderManager.CanvasElements.ResumeGameButton;
        EventsManager.StartListening(nameof(GameManager.singleton.StatesEvents.OnPauseIn), StartPause);
        EventsManager.StartListening(nameof(GameManager.singleton.StatesEvents.OnPauseOut), QuitPause);
        
        playerControls.MenuNavigation.Movement.started += OnAxesChanged;
        GameManager.singleton.ResourcesLoaderManager.CanvasElements.PausePanel.SetActive(false);
        ResumeButton.onClick.AddListener(delegate {PauseGame();});
        QuitButton.onClick.AddListener(delegate { QuitGame();});
    }
    private void QuitGame()
    {
        Application.Quit();
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
        GameManager.singleton.ResourcesLoaderManager.CanvasElements.PausePanel.SetActive(true);
    }
    private void QuitPause(Args args)
    {
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
