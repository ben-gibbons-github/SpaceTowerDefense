using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class StasisBomb : UnitTurret
    {
        static Color ParticleColor = new Color(0.1f, 0.175f, 0.3f);
        new static int MaxSearchTime = 300;

        int MaxUnitCount = 20;
        int StasisTime = 30000;
        float EffectDistance = 400;

        int SearchTime = 0;

        public StasisBomb(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 20;
            HullToughness = 20;
            MaxEngagementDistance = StasisBombCard.EngagementDistance;
            EffectDistance = MaxEngagementDistance * 1.5f;
            MaxBuildTime = 5000;
            Resistence = AttackType.Green;
            Weakness = AttackType.None;
            ThreatLevel = 0;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(StasisBombCard.STurretSize));
            ShieldColor = new Color(0.75f, 0.75f, 0.75f);
        }

        protected override void Upgrade()
        {
            MaxUnitCount += 15;
            EffectDistance *= 1.5f;
            MaxEngagementDistance *= 1.5f;
            StasisTime = (int)(StasisTime * 1.5f);

            base.Upgrade();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                SearchTime += gameTime.ElapsedGameTime.Milliseconds;
                if (SearchTime > MaxSearchTime)
                {
                    SearchTime -= MaxSearchTime;
                    QuadGrid quad = Parent2DScene.quadGrids.First.Value;
                    int UnitCount = 0;

                    foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(MaxEngagementDistance * 2)))
                        if (o.GetType().IsSubclassOf(typeof(UnitShip)) && Vector2.Distance(Position.get(), o.Position.get()) < MaxEngagementDistance)
                        {
                            UnitShip s = (UnitShip)o;

                            if (s.CanBeTargeted() && s.FreezeTime < 1)
                            {
                                UnitCount++;
                            }
                        }

                    if (UnitCount >= MaxUnitCount)
                    {
                        BlowUp();
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void BlowUp()
        {
            if (!Dead)
            {
                QuadGrid quad = Parent2DScene.quadGrids.First.Value;
                foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(EffectDistance * 2)))
                    if (o.GetType().IsSubclassOf(typeof(UnitShip)) && Vector2.Distance(Position.get(), o.Position.get()) < EffectDistance)
                    {
                        UnitShip s = (UnitShip)o;
                        if (s.CanBeTargeted())
                        {
                            s.CanDeathSound = false;
                            s.CanCloak = false;
                            s.Damage(4, 0, Vector2.Zero, this, AttackType.Blue);
                            s.LastDamager = this;
                            s.StunState = AttackType.Blue;
                            s.FreezeTime = StasisTime;
                            s.CanDeathSound = true;
                        }
                    }

                Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
                for (int i = 0; i < 30; i++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * MaxEngagementDistance * 3, ParticleColor, 40, 5);

                for (int i = 0; i < 2; i++)
                {
                    FlareSystem.AddLightingPoint(Position3, new Vector3(0.3f), new Vector3(0, 0, 1), MaxEngagementDistance / 10, 40, 5, 10);
                    ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, MaxEngagementDistance * 6, 5);
                    ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, MaxEngagementDistance * 3, 4);
                }
            }
            base.BlowUp();
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
