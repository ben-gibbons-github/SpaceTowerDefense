using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ProtectionCaster : UnitTurret
    {
        static Color ParticleColor = new Color(0.1f, 0.3f, 0.175f);

        int ProtectionSearchTime = 500;
        int SearchTime = 0;

        float HealingAmount = 0;
        float MaxHealingAmount = 50;
        float HealingReduction = 1;

        public ProtectionCaster(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 30;
            HullToughness = 30;
            MaxEngagementDistance = ProtectionCasterCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.None;
            Weakness = AttackType.None;
            ShieldColor = ShieldInstancer.WhiteShield;
        }

        public override void NewWaveEvent()
        {
            HealingAmount = MaxHealingAmount;
            base.NewWaveEvent();
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(ProtectionCasterCard.STurretSize));
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

                    if (SearchTime > ProtectionSearchTime)
                    {
                        SearchTime -= ProtectionSearchTime;

                        bool Found = false;
                        QuadGrid quad = Parent2DScene.quadGrids.First.Value;
                        float NewHealingAmount = HealingAmount;
                        NewHealingAmount -= Heal(HealingAmount) / HealingAmount * HealingReduction;

                        foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(MaxEngagementDistance * 2)))
                            if (o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                            {
                                UnitBuilding s = (UnitBuilding)o;
                                if (!s.Dead && s.IsAlly(this) && Vector2.Distance(Position.get(), o.Position.get()) < MaxEngagementDistance)
                                {
                                    if (s.CanBeTargeted())
                                    {
                                        NewHealingAmount -= s.Heal(HealingAmount) / HealingAmount * HealingReduction;
                                    }

                                    Found = true;
                                }
                            }

                        if (Found)
                        {
                            HealingAmount = NewHealingAmount;

                            SoundManager.Play3DSound("Healing",
                                new Vector3(Position.X(), Y, Position.Y()),
                                0.75f, 800, 2);

                            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
                            for (int i = 0; i < 30; i++)
                                ParticleManager.CreateParticle(Position3, Rand.V3() * MaxEngagementDistance / 1000f * 3, ParticleColor, 40, 5);

                            for (int i = 0; i < 2; i++)
                            {
                                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, MaxEngagementDistance * 2, 5);
                                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, MaxEngagementDistance * 1, 4);
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
            MaxHealingAmount *= 2;
            base.Upgrade();
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("ExtraTurrets/Turret2");
        }
    }
}
