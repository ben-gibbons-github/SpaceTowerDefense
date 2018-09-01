using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerKnightGun : GunBasic
    {
        public PlayerKnightGun()
        {
            IdealEngagementDistance = 400;
            SetFireModes(new PlayerKnightFireMode(this));
        }
    }
}
