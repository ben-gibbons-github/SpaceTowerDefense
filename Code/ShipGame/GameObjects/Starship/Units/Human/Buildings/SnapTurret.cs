using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SnapTurret : UnitTurret
    {
        static Color ParticleColor = new Color(0.1f, 0.175f, 0.3f);

        int SnapSearchTime = 500;
        int StartingMaxSearchTime = 500;
        int SearchTime = 0;

        public SnapTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 60;
            HullToughness = 60;
            MaxEngagementDistance = SnapTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Green;
            Weakness = AttackType.Blue;
            ShieldColor = new Color(0.5f, 1, 0.5f);
        }

        public override void NewWaveEvent()
        {
            SnapSearchTime = StartingMaxSearchTime;
            base.NewWaveEvent();
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(SnapTurretCard.STurretSize));
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                if (ShutDownTime > 0 || VirusTime > 0)
                {
                    if (ShutDownTime > 0)
                        ShutDownTime -= gameTime.ElapsedGameTime.Milliseconds * 3;
                    else
                        Rotation.set(Rotation.get() + MathHelper.ToRadians(gameTime.ElapsedGameTime.Milliseconds * 20));
                }
                else
                {
                    SearchTime += gameTime.ElapsedGameTime.Milliseconds;

                    if (SearchTime > SnapSearchTime)
                    {
                        SearchTime -= SnapSearchTime;

                        bool Found = false;
                        QuadGrid quad = Parent2DScene.quadGrids.First.Value;

                        foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(MaxEngagementDistance * 2)))
                            if (o.GetType().IsSubclassOf(typeof(UnitShip)))
                            {
                                UnitShip s = (UnitShip)o;
                                if (!s.Dead && !s.IsAlly(this) && Vector2.Distance(Position.get(), o.Position.get()) < MaxEngagementDistance)
                                {
                                    if (s.CanBeTargeted() && s.SnapBounce())
                                        s.EMP(this, IsUpdgraded ? 1 : 0);
                                    SnapSearchTime += (int)(StartingMaxSearchTime / 20 * (1.5f + s.UnitLevel) / 2f);
                                    Found = true;
                                }
                            }

                        if (Found)
                        {
                            SoundManager.Play3DSound("SnapTurretFire",
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
            return InstanceManager.HumanUnitIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Turret2");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Melee)
                damage *= 0.2f;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
