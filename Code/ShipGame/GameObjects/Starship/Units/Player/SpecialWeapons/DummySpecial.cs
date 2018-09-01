using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class DummyAbility : SpecialWeapon
    {
        public DummyAbility()
        {
            MaxRechargeTime = 10000;
        }

        public override void Trigger()
        {
            if (RechargeTime >= MaxRechargeTime)
            {
                ParentShip.ParentLevel.AddObject(new Dummy(ParentShip.FactionNumber));
                RechargeTime = 0;
            }
            base.Trigger();
        }
    }
}
