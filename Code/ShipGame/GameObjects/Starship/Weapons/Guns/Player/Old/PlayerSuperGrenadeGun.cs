using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerSuperGrenadeGun : GunBasic
    {
        public PlayerSuperGrenadeGun()
        {
            
            
            
            SetFireModes(new PlayerSuperGrenadeFireMode(this));
        }
    }
}
