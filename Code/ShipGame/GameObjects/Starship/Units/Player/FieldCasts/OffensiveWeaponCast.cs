using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class OffensiveWeaponCast : OffenseAbility
    {
        protected FireMode fireMode;

        public override bool Trigger(PlayerShip p)
        {
            fireMode.Ammo = 1000;
            fireMode.SetParent(p);
            fireMode.Fire(p.Guns[0].Rotation); 
            return true;
        }
    }
}
