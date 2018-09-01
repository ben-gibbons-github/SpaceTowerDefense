using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerJuggernautGun : GunBasic
    {
        public PlayerJuggernautGun()
        {
            IdealEngagementDistance = 200;
            SetFireModes(new PlayerJuggernautFireMode(this));
        }
    }
}
