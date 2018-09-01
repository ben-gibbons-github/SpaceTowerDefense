using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Counter: GameObject
    {
        IntValue Count;
        IntValue EventCount;
        EventValue[] Events;
        IntValue[] Thresholds;
        private int EventCountPrevious;

        public override void Create()
        {
            Count = new IntValue("Counter");
            EventCount = new IntValue("Event Count");
            EventCount.ChangeEvent = CountChange;
            base.Create();
        }

        private void CountChange()
        {
            EventValue[] NewEvents = new EventValue[EventCount.get()];
            IntValue[] NewThresholds = new IntValue[EventCount.get()];

            for (int i = 0; i < Math.Max(EventCount.get(), EventCountPrevious); i++)
            {
                if (i < EventCount.get())
                {
                    if (i < EventCountPrevious)
                    {
                        NewEvents[i] = Events[i];
                        NewThresholds[i] = Thresholds[i];
                    }
                    else
                    {
                        Level.ReferenceObject = this;
                        NewThresholds[i] = new IntValue("Threshold " + i.ToString(), i * 1000);
                        NewEvents[i] = new EventValue("Event " + i.ToString());
                    }
                }
                else
                {
                    RemoveValue(Thresholds[i]);
                    RemoveValue(Events[i]);
                }
            }

            Events = NewEvents;
            Thresholds = NewThresholds;

            EventCountPrevious = EventCount.get();
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                ParentScene.UpdateSelected();
            }
#endif
        }

#if EDITOR && WINDOWS
        public override void UpdateEditor(GameTime gameTime)
        {
            if (EventCount.get() < 1)
                EventCount.set(1);

            base.UpdateEditor(gameTime);
        }
#endif
        public override bool TriggerEvent(EventType Event, string[] args)
        {
            switch (Event)
            {
                case EventType.Add:
                    if (args.Count() > 0)
                        Count.add(Logic.ParseI(args[0]));
                    else
                        Count.add(1);
                    return true;
                case EventType.AddFrom:
                    if (args.Count() > 1 && Logic.ParseI(args[0]) == Count.get())
                        Count.set(Logic.ParseI(args[1]));
                    return true;
                case EventType.AddFromAlt:
                    if (args.Count() > 2)
                    {
                        if (Logic.ParseI(args[0]) == Count.get())
                            Count.set(Logic.ParseI(args[1]));
                        else
                            Count.set(Logic.ParseI(args[2]));
                    }
                    return true;
            }
            return base.TriggerEvent(Event, args);
        }
    }
}
