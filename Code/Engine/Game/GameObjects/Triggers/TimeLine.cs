using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class TimeLine : GameObject
    {
        BoolValue Triggered;
        BoolValue Loop;
        IntValue ResetTime;
        IntValue EventCount;
        private int EventCountPrevious;
        EventValue[] Events;
        IntValue[] Times;
        int Time;
        int TimePrevious;
        int MaxTime;

        public override void Create()
        {
            Triggered = new BoolValue("Triggered");
            Triggered.ChangeEvent = Trigger;

            Loop = new BoolValue("Loop");
            ResetTime = new IntValue("Reset Time");

            EventCount = new IntValue("Event Count");
            EventCount.ChangeEvent = CountChange;

            base.Create();
        }

        private void TimeChange()
        {
            for (int i = 0; i < EventCount.get(); i++)
                if (Times[i].get() > MaxTime)
                    MaxTime = Times[i].get();
        }

        private void CountChange()
        {
            EventValue[] NewEvents = new EventValue[EventCount.get()];
            IntValue[] NewTimes = new IntValue[EventCount.get()];

            for (int i = 0; i < Math.Max(EventCount.get(), EventCountPrevious); i++)
            {
                if (i < EventCount.get())
                {
                    if (i < EventCountPrevious)
                    {
                        NewEvents[i] = Events[i];
                        NewTimes[i] = Times[i];
                    }
                    else
                    {
                        Level.ReferenceObject = this;
                        NewTimes[i] = new IntValue("Time " + i.ToString(), i * 1000);
                        NewTimes[i].ChangeEvent = TimeChange;
                        NewEvents[i] = new EventValue("Event " + i.ToString());
                    }
                }
                else
                {
                    RemoveValue(Times[i]);
                    RemoveValue(Events[i]);
                }
            }

            Events = NewEvents;
            Times = NewTimes;

            EventCountPrevious = EventCount.get();
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                ParentScene.UpdateSelected();
            }
#endif
        }

        public override bool TriggerEvent(EventType Event, string[] args)
        {
            switch (Event)
            {
                case EventType.SetTime:
                    Time = Logic.ParseI(args[0]);
                    Triggered.set(true);
                    return true;
                case EventType.ResetTime:
                    Time = ResetTime.get();
                    Triggered.set(true);
                    return true;
            }

            return base.TriggerEvent(Event, args);
        }

        private void Trigger()
        {
            if (Triggered.get())
                AddTag(GameObjectTag.Update);
            else
                RemoveTag(GameObjectTag.Update);
        }
#if EDITOR && WINDOWS
        public override void UpdateEditor(GameTime gameTime)
        {
            if (EventCount.get() < 1)
                EventCount.set(1);

            base.UpdateEditor(gameTime);
        }
#endif
        public override void Update(GameTime gameTime)
        {
            TimePrevious = Time;
            Time += gameTime.ElapsedGameTime.Milliseconds;

            for (int i = 0; i < EventCount.get(); i++)
                if (Time > Times[i].get() && TimePrevious < Times[i].get())
                    Events[i].Trigger();

            if (Time > MaxTime)
            {
                if (Loop.get())
                    Time = ResetTime.get();
                else
                    Triggered.set(false);
            }

            base.Update(gameTime);
        }
    }
}
