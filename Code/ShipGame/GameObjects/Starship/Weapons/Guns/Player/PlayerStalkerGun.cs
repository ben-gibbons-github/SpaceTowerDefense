using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerStalkerGun : GunBasic
    {
        public PlayerStalkerGun()
        {
            SetFireModes(new PlayerStalkerFireMode(this));
        }
    }
}
