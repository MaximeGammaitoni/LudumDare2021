using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using States;

public class StatesManager
{
    BaseState currentState;

    public UnityAction<StateChangedArgs> OnStateChanged;

    public BaseState CurrentState { 
        get
        {
            return currentState;
        }
        set
        {
            var oldState = currentState;
            currentState?.Out();
            currentState = value;
            currentState.In();
            EventsManager.TriggerEvent(nameof(OnStateChanged), new StateChangedArgs() {
                oldState = oldState,
                newState = currentState
            });
        }
    }

    public void ChangeCurrentState(BaseState state)
    {
        CurrentState = state;
    }
}

public class StateChangedArgs : Args
{
    public BaseState oldState;
    public BaseState newState;
}