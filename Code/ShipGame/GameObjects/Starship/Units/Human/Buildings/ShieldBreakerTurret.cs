using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ShieldBreakerTurret : UnitTurret
    {
        new static int MaxSearchTime = 1000;
        int SearchTime = 0;

        public ShieldBreakerTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 5;
            HullToughness = 5;
            MaxEngagementDistance = SnapTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.None;
            Weakness = AttackType.None;
            ShieldColor = new Color(1, 1, 1);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(ShieldBreakerTurretCard.STurretSize));
        }

        public override void Update(GameTime gameTime)
        {
            SearchTime += gameTime.ElapsedGameTime.Milliseconds;
            if (SearchTime > MaxSearchTime)
            {
                SearchTime -= MaxSearchTime;

                QuadGrid quad = Parent2DScene.quadGrids.First.Value;

                foreach(Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(MaxEngagementDistance * 2)))
                    if (o.GetType().IsSubclassOf(typeof(UnitShip)))
                    {
                        UnitShip s = (UnitShip)o;
                        if (s.ShieldDamage < s.ShieldToughness)
                            s.ShieldFlash(1);

                        s.ShieldDamage += 10000;
                    }
            }

            base.Update(gameTime);
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanUnitIndex + 6;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Turret7");
        }
    }
}
