using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ShieldTurret : UnitTurret
    {
        public ShieldTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 20;
            HullToughness = 20;
            MaxEngagementDistance = 0;
            MaxBuildTime = 30000;
            ShieldColor = new Color(0.5f, 0.75f, 0.5f);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(256));
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanUnitIndex + 3;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Turret4");
        }
    }
}
