using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerRaidGun : GunBasic
    {
        public PlayerRaidGun()
        {
            IdealEngagementDistance = 200;
            SetFireModes(new PlayerRaidFireMode(this));
        }
    }
}
