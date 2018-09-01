using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class PulseTurret : UnitTurret
    {
        static Color ParticleColor = new Color(0.3f, 0.175f, 0.1f);

        int PulseSearchTime = 500;
        int StartingMaxSearchTime = 500;
        int SearchTime = 0;

        public PulseTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 80;
            HullToughness = 80;
            MaxEngagementDistance = PulseTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Red;
            Weakness = AttackType.Green;
            ShieldColor = new Color(1, 0.5f, 0.5f);
        }

        public override void NewWaveEvent()
        {
            PulseSearchTime = StartingMaxSearchTime;
            base.NewWaveEvent();
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(PulseTurretCard.STurretSize));
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                if (ShutDownTime > 0 || VirusTime > 0)
                {
                    ShutDownTime = 0;
                    VirusTime = 0;
                }
                else
                {
                    SearchTime += gameTime.ElapsedGameTime.Milliseconds;

                    if (SearchTime > PulseSearchTime)
                    {
                        SearchTime -= PulseSearchTime;

                        bool Found = false;
                        QuadGrid quad = Parent2DScene.quadGrids.First.Value;

                        foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(MaxEngagementDistance * 2)))
                            if (o.GetType().IsSubclassOf(typeof(UnitShip)))
                            {
                                UnitShip s = (UnitShip)o;
                                if (!s.Dead && !s.IsAlly(this) && Vector2.Distance(Position.get(), o.Position.get()) < MaxEngagementDistance)
                                {
                                    if (s.CanBeTargeted() && s.SnapBounce())
                                    {
                                        s.ShieldDamage = s.ShieldToughness + 1;
                                        s.LastDamager = this;
                                        s.FreezeTime = 1000;
                                        s.StunState = AttackType.Melee;
                                        s.SetSpeed(Vector2.Normalize(s.Position.get() - Position.get()) * 16);
                                    }

                                    PulseSearchTime += (int)(StartingMaxSearchTime / 100f * (1.5f + s.UnitLevel));
                                    Found = true;
                                }
                            }

                        if (Found)
                        {
                            SoundManager.Play3DSound("PulseTurretFire",
                                new Vector3(Position.X(), Y, Position.Y()),
                                0.75f, 800, 2);

                            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
                            for (int i = 0; i < 30; i++)
                                ParticleManager.CreateParticle(Position3, Rand.V3() * MaxEngagementDistance / 1000f * 3, ParticleColor, 40, 5);

                            for (int i = 0; i < 2; i++)
                            {
                                FlareSystem.AddLightingPoint(Position3, new Vector3(0.3f), new Vector3(0, 0, 1), MaxEngagementDistance / 10, 40, 5, 10);
                                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, MaxEngagementDistance * 6, 5);
                                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, MaxEngagementDistance * 3, 4);
                            }
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Upgrade()
        {
            MaxEngagementDistance += 250;
            ShieldToughness *= 4;
            HullToughness *= 4;
            StartingMaxSearchTime /= 2;
            base.Upgrade();
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("ExtraTurrets/Turret1");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Blue || attackType == AttackType.Red)
                damage *= 0.2f;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
