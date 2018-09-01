using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerLaserGun : GunBasic
    {
        public PlayerLaserGun()
        {
            SetFireModes(new PlayerLaserFireMode(this));
        }
    }
}
