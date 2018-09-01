using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class EngineerTurretGun : GunBasic
    {
        public EngineerTurretGun()
        {
            SetFireModes(new EngineerTurretFireMode());
        }
    }
}
