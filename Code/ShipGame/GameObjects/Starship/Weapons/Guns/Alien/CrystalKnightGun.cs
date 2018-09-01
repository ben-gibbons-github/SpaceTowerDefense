using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class CrystalKnightGun : GunBasic
    {
        public CrystalKnightGun()
        {   
            SetFireModes(new CrystalKnightFireMode(this), new CrystalBattleCruiserFireMode());
        }
    }
}
