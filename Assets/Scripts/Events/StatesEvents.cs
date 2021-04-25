using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatesEvents
{
    public StatesEvents()
    {

    }

    public  UnityAction<Args> OnBeginIn;
    public  UnityAction<Args> OnBeginOut;

    public  UnityAction<Args> OnRunIn;
    public  UnityAction<Args> OnRunOut;

    public  UnityAction<Args> OnWinIn;
    public  UnityAction<Args> OnWinOut;

    public  UnityAction<Args> OnEndIn;
    public  UnityAction<Args> OnEndOut;

    public UnityAction<Args> OnPauseIn;
    public UnityAction<Args> OnPauseOut;

    public UnityAction<Args> OnFallingIn;
    public UnityAction<Args> OnFallingOut;

    public UnityAction<Args> OnLandingIn;
    public UnityAction<Args> OnLandingOut;
    
    public class StatesEventArgs : Args
    {
        
    }
}
