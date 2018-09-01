using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class NullObject : GameObject {

        public BoolValue PassEvents;

        public override void Create()
        {
            PassEvents = new BoolValue("Pass Events", true);
            base.Create();
        }

        public override bool TriggerEvent(EventType Event, string[] args)
        {
            bool Passed = false;
            if (PassEvents.get())
                foreach (GameObject g in HierarchyChildren)
                    if (g.TriggerEvent(Event, args))
                        Passed = true;
            return Passed;
        }
    }
}
