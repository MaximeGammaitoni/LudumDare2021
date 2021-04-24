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
        public virtual void In()
        {

        }
        public virtual void Out()
        {

        }
    }

    public class Begin : BaseState
    {
        public override void In()
        {
            EventsManager.TriggerEvent("OnBeginIn");
        }
        public override void Out()
        {

            EventsManager.TriggerEvent("OnBeginOut");
        }
    }
    public class Run : BaseState
    {
        public override void In()
        {
            EventsManager.TriggerEvent("OnRunIn");
        }
        public override void Out()
        {

            EventsManager.TriggerEvent("OnRunOut");
        }
    }

    public class Win : BaseState
    {
        public override void In()
        {
            EventsManager.TriggerEvent("OnWinIn");
        }
        public override void Out()
        {

            EventsManager.TriggerEvent("OnWinOut");
        }
    }

    public class End : BaseState
    {
        public override void In()
        {
            EventsManager.TriggerEvent("OnEndIn");
        }
        public override void Out()
        {

            EventsManager.TriggerEvent("OnEndOut");
        }
    }

    public class Pause : BaseState
    {
        public override void In()
        {
            EventsManager.TriggerEvent("OnPauseIn");
        }
        public override void Out()
        {

            EventsManager.TriggerEvent("OnPauseOut");
        }
    }

}
