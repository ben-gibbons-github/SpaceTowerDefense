using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerEngineerGun : GunBasic
    {
        public PlayerEngineerGun()
        {
            SetFireModes(new PlayerEngineerFireMode(this));
        }
    }
}
