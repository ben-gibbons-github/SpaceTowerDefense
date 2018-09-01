using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public enum EventType
    {
        Delete,
        ChangeName,
        AddTag,
        RemoveTag,
        ViewOn,
        ViewOff,
        SetValue,
        SetTime,
        ResetTime,
        SwapViews,
        AddObject,
        Add,
        AddFrom,
        AddFromAlt,
        Kill,
        Revive,
        GotoLevel,
        GotoScene,
        SendTo,
    }

    public class EventCall
    {
        public static EventType[] AllEvents = 
        {
            EventType.Delete,
            EventType.ChangeName,
            EventType.AddTag,
            EventType.RemoveTag,
            EventType.ViewOn,
            EventType.ViewOff,
            EventType.SetValue,
            EventType.SetTime,
            EventType.ResetTime,
            EventType.SwapViews,
            EventType.AddObject,
            EventType.Add,
            EventType.AddFrom,
            EventType.AddFromAlt,
            EventType.Kill,
            EventType.Revive,
            EventType.GotoLevel,
            EventType.GotoScene,
        };


        public GameObject MyObject;
        public EventType Function;
        public string[] args;
        public EventCall next;

        public void Trigger(GameObject g)
        {
            MyObject.TriggerEvent(Function, args);
            if (next != null)
                next.Trigger(g);
        }

        public void Add(EventCall e)
        {
            if (next != null)
                next.Add(e);
            else
                next = e;
        }
    }
}
