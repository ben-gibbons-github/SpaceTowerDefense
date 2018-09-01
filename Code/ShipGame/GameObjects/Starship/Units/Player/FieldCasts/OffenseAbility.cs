using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class OffenseAbility
    {
        public virtual Texture2D CastTexture()
        {
            return null;
        }

        public virtual bool Trigger(PlayerShip p)
        {
            return false;
        }

        public virtual int GetTriggerTime()
        {
            return 500;
        }
    }
}
