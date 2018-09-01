using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CloakingTurret : UnitTurret
    {
        int CloakingTokens = 0;
        int MaxCloakingTokens = 1;
        int SearchTime = 0;
        int CloakSearchTime = 2000;

        public CloakingTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 10;
            HullToughness = 30;
            MaxEngagementDistance = CloakingTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Blue;
            Weakness = AttackType.Red;
            ShieldColor = new Color(0.5f, 0.5f, 1);
            Solid = false;
        }

        public override void NewWaveEvent()
        {
            CloakingTokens = MaxCloakingTokens;
            base.NewWaveEvent();
        }

        public override void Update(GameTime gameTime)
        {
            if (CloakingTokens > 0)
            {
                SearchTime += gameTime.ElapsedGameTime.Milliseconds;
                if (SearchTime > CloakSearchTime)
                {
                    SearchTime -= CloakSearchTime;

                    QuadGrid quad = Parent2DScene.quadGrids.First.Value;

                    foreach(Basic2DObject o in quad.Enumerate(Position.get(),new Vector2(MaxEngagementDistance)))
                        if (o.GetType().IsSubclassOf(typeof(UnitTurret)))
                        {
                            UnitTurret u = (UnitTurret)o;
                            u.fieldState = FieldState.Cloaked;
                            u.FieldStateTime = 1000000;
                            CloakingTokens--;
                            fieldState = FieldState.Cloaked;
                            FieldStateTime = 1000000;
                            if (CloakingTokens == 0)
                                break;
                        }
                }
            }
            base.Update(gameTime);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(CloakingTurretCard.STurretSize));
        }

        protected override void Upgrade()
        {
            MaxEngagementDistance *= 2;
            MaxCloakingTokens += 1;
            base.Upgrade();
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanUnitIndex + 5;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Turret6");
        }
    }
}
