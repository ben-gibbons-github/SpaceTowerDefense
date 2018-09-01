using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class CrystalFighterGun : GunBasic
    {
        public CrystalFighterGun()
        {   
            SetFireModes(new CrystalFighterFireMode(this));
        }
    }
}
