using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ScatterTurretGun : GunBasic
    {
        public ScatterTurretGun()
        {
            SetFireModes(new ScatterTurretFireMode());
        }
    }
}
