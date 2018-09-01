using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CrystalElemental : CrystalShip
    {
        protected static Color CrystalColor = new Color(0.75f, 0.75f, 1);
        protected static float AlphaChange = 0.05f;

        protected float StunDistance = 400;
        protected int MaxStunSearchTime = 1000;
        protected UnitBasic StunnedUnit;
        protected int StunSearchTime = 0;
        protected float Alpha = 0;

        int Power = 0;
        int MaxPower = 2000;
        
        public CrystalElemental(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyCrystalExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            Add(UnitTag.Light);
            ScoreToGive = 50;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(50));

            Weakness = AttackType.Red;
            Resistence = AttackType.Green;
            ShieldColor = ShieldInstancer.WhiteShield;
            StunDistance = 800;
        }

        public override int GetUnitWeight()
        {
            return 4;
        }

        public override  void Update(GameTime gameTime)
        {
            StunDistance = 500;
            if (StunnedUnit == null)
                Power += gameTime.ElapsedGameTime.Milliseconds;

            if (Power > MaxPower)
            {
                Power = MaxPower;
                StunSearchTime += gameTime.ElapsedGameTime.Milliseconds;
                if (StunSearchTime > MaxStunSearchTime && (StunnedUnit == null || !StunnedUnit.CanBeTargeted()))
                {
                    StunSearchTime -= MaxStunSearchTime;

                    QuadGrid quad = Parent2DScene.quadGrids.First.Value;
                    float BestDistance = StunDistance;

                    foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(StunDistance)))
                        if (o.GetType().Equals(typeof(PlayerShip)))
                        {
                            float d = Vector2.Distance(Position.get(), o.Position.get());
                            if (d < BestDistance)
                            {
                                PlayerShip u = (PlayerShip)o;

                                if (WaveManager.ActiveTeam == u.GetTeam() && u.CanBeTargeted())
                                {
                                    BestDistance = d;
                                    StunnedUnit = u;
                                    Alpha = 0;
                                }
                            }
                        }
                }
            }

            if (StunnedUnit != null)
            {
                if (Power > 0 && StunnedUnit.CanBeTargeted() && Vector2.Distance(Position.get(), StunnedUnit.Position.get()) < StunDistance)
                {
                    if (Alpha < 1)
                    {
                        Alpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                        if (Alpha > 1)
                            Alpha = 1;
                    }
                    StunnedUnit.SetSpeed(Vector2.Normalize(StunnedUnit.Position.get() - Position.get()) * 8);
                    StunnedUnit.ShutDownTime = 100;

                    Vector3 ParticlePosition = new Vector3(Position.X(), Y, Position.Y());
                    Vector3 ChangeVector = (new Vector3(StunnedUnit.Position.X(), Y, StunnedUnit.Position.Y()) - ParticlePosition);
                    ChangeVector.Normalize();
                    ChangeVector *= 10;

                    float StaticDistance = Vector2.Distance(new Vector2(ParticlePosition.X, ParticlePosition.Z), StunnedUnit.Position.get());
                    float DynamicDistance = StaticDistance;

                    while (DynamicDistance > 10)
                    {
                        ParticleManager.CreateParticle(ParticlePosition, Vector3.Zero, CrystalColor * Alpha, Size.X() + (StunnedUnit.Size.X() - Size.X()) * (1 - DynamicDistance / StaticDistance) * 16, 1);
                        ParticlePosition += ChangeVector;
                        DynamicDistance = Vector2.Distance(new Vector2(ParticlePosition.X, ParticlePosition.Z), StunnedUnit.Position.get());
                    }
                }
                else
                    StunnedUnit = null;
            }
            base.Update(gameTime);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            bool NoFreeze = false;

            if (attackType != AttackType.Explosion && ShieldDamage < ShieldToughness)
            {
                if (attackType == Resistence)
                    return;

                NoFreeze = true;
                damage /= 4;
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);

            if (NoFreeze)
                FreezeTime = 0;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = (2f * Mult * (1 + Level)) / 2;
            HullToughness = 0.5f + (Level - 1) / 2f;
            ShieldToughness = 0.5f + (Level - 1) / 2f;
            Acceleration = (0.35f + (Level - 1) / 20f) * 0.35f;

            base.SetLevel(Level, Mult);
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienBasicIndex + 3;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Ship4");
        }
    }
}
