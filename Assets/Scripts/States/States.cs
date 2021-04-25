using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    public interface IState
    {

    }
    public abstract class BaseState : IState
    {
        public bool ElementsCanMove;
        public virtual void In()
        {

        }
        public virtual void Out()
        {

        }
    }

    public class Begin : BaseState
    {
        public Begin()
        {
            ElementsCanMove = false;
        }
        public override void In()
        {
            EventsManager.TriggerEvent(nameof(StatesEvents.OnBeginOut));
        }
        public override void Out()
        {

            EventsManager.TriggerEvent(nameof(StatesEvents.OnBeginIn));
        }
    }
    public class Run : BaseState
    {
        public Run()
        {
            ElementsCanMove = true;
        }
        public override void In()
        {
            EventsManager.TriggerEvent(nameof(StatesEvents.OnRunIn));
        }
        public override void Out()
        {

            EventsManager.TriggerEvent(nameof(StatesEvents.OnRunOut));
        }
    }

    public class Win : BaseState
    {
        public Win()
        {
            ElementsCanMove = false;
        }
        public override void In()
        {
            EventsManager.TriggerEvent(nameof(StatesEvents.OnWinIn));
        }
        public override void Out()
        {

            EventsManager.TriggerEvent(nameof(StatesEvents.OnWinOut));
        }
    }

    public class End : BaseState
    {
        public End()
        {
            ElementsCanMove = false;
        }
        public override void In()
        {
            EventsManager.TriggerEvent(nameof(StatesEvents.OnEndIn));
        }
        public override void Out()
        {

            EventsManager.TriggerEvent(nameof(StatesEvents.OnEndOut));
        }
    }

    public class Falling : BaseState
    {
        public Falling()
        {
            ElementsCanMove = false;
        }
        public override void In()
        {
            EventsManager.TriggerEvent(nameof(StatesEvents.OnFallingIn));
        }
        public override void Out()
        {

            EventsManager.TriggerEvent(nameof(StatesEvents.OnFallingOut));
        }
    }

    public class Landing : BaseState
    {
        public Landing()
        {
            ElementsCanMove = false;
        }
        public override void In()
        {
            EventsManager.TriggerEvent(nameof(StatesEvents.OnLandingIn));
        }
        public override void Out()
        {

            EventsManager.TriggerEvent(nameof(StatesEvents.OnLandingOut));
        }
    }

    public class Pause : BaseState
    {
        public Pause()
        {
            ElementsCanMove = false;
        }
        public override void In()
        {
            EventsManager.TriggerEvent(nameof(StatesEvents.OnPauseIn));
        }
        public override void Out()
        {

            EventsManager.TriggerEvent(nameof(StatesEvents.OnPauseOut));
        }
    }

}
