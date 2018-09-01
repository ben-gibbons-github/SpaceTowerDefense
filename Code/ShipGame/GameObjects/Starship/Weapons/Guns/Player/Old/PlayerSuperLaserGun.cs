using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerSuperLaserGun : GunBasic
    {
        public PlayerSuperLaserGun()
        {
            
            
            
            SetFireModes(new PlayerSuperLaserFireMode(this));
        }
    }
}
