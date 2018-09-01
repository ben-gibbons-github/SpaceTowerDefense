using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class OverClock : GameObject
    {
        public override void Create()
        {
            AddTag(GameObjectTag.OverDrawViews);
            base.Create();
        }
    }
}
