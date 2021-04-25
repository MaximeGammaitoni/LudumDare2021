using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;

public class StatesManager
{
    BaseState currentState;
    public BaseState CurrentState { 
        get
        {
            return currentState;
        }
        set
        {
            currentState?.Out();
            currentState = value;
            currentState.In();
        }
    }

    public void ChangeCurrentState(BaseState state)
    {
        CurrentState = state;
    }
}
