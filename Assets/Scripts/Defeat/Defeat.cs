using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;


public class Defeat : MonoBehaviour
{
    public Button TryAgain;
    public Button Quit;
    public Button MainMenu;

    private PlayerControls playerControls;
    private Vector2 direction;

    [SerializeField] public List<Button> menuButtons = new List<Button>();
    private int selectedButton;


    // Start is called before the first frame update
    void Start()
    {
        InitializeMenu();
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.MenuNavigation.Movement.started += OnAxesChanged;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.MenuNavigation.Movement.started -= OnAxesChanged;
    }

    private void OnAxesChanged(CallbackContext ctx)
    {

        if (ctx.started)
        {
            direction = ctx.ReadValue<Vector2>();
            //Debug.Log(direction);
            if (direction.y > 0.0f)
            {
                SelectButton(-1);
            }
            else if (direction.y < -0.0f)
            {
                SelectButton(1);
            }

        }
        else if (ctx.canceled)
        {
            direction = Vector2.zero;
        }
        Debug.Log("selectedButtonIndex " + selectedButton);
        Debug.Log("direction is " + direction);
    }

    private void InitializeMenu()
    {
        menuButtons.Add(TryAgain);
        menuButtons.Add(Quit);
        menuButtons.Add(MainMenu);
        TryAgain.Select();
        selectedButton = 0;
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
        if (selectedButton > 2)
        {
            selectedButton = 2;
        }
        Debug.Log("Correction " + selectedButton);


        menuButtons[selectedButton].Select();
        Debug.Log(menuButtons[selectedButton].gameObject.name);
    }
}
