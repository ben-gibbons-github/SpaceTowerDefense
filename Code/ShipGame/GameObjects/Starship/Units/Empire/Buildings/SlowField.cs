using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace BadRabbit.Carrot
{
    public class SlowField : UnitTurret
    {
        static Color ParticleColor = new Color(0.1f, 0.175f, 0.3f);

        new int MaxSearchTime = 400;
        int SearchTime = 0;

        SoundEffectInstance SoundInstance;

        public SlowField(int FactionNumber)
            : base(FactionNumber)
        {
            MaxEngagementDistance = SlowFieldCard.EngagementDistance;
            CloakAlpha = 1;
            ShipMatrixScale *= 1.75f;
        }

        public override bool CanBeTargeted()
        {
            return false;
        }

        public override void Destroy()
        {
            if (SoundInstance != null && !SoundInstance.IsDisposed)
            {
                SoundInstance.Dispose();
                SoundInstance = null;
            }
            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            SoundInstance = SoundManager.PlayLoopingSound(SoundInstance, "SlowFieldHum",
                new Vector3(Position.X(), Y, Position.Y()), 0.03f, MaxEngagementDistance + 200, 2);

            Size.set(new Vector2(SlowFieldCard.STurretSize - 5 + 10 * Rand.F()));

            Vector2 ProjectedPosition = Position.get() + Rand.NV2() * MaxEngagementDistance;
            Vector3 Position3 = new Vector3(ProjectedPosition.X, Y, ProjectedPosition.Y);
            ParticleManager.CreateParticle(Position3, Rand.V3() * Size.X() * 2, ParticleColor, 40, 5);
            if (!Dead)
            {
                SearchTime += gameTime.ElapsedGameTime.Milliseconds;
                if (SearchTime > MaxSearchTime)
                {
                    bool CanEmp = true;
                    SearchTime -= MaxSearchTime;

                    QuadGrid quad = Parent2DScene.quadGrids.First.Value;

                    foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(MaxEngagementDistance * 2)))
                        if (o.GetType().IsSubclassOf(typeof(UnitBasic)))
                        {
                            UnitBasic s = (UnitBasic)o;
                            if (!s.Dead && !s.IsAlly(this) && Vector2.Distance(Position.get(), o.Position.get()) < MaxEngagementDistance - o.Size.X())
                            {
                                if (s.GetType().IsSubclassOf(typeof(UnitShip)))
                                {
                                    UnitShip s2 = (UnitShip)s;
                                    if (!s2.Slowed)
                                    {
                                        s2.Slowed = true;
                                        s2.Acceleration /= 2;
                                        if (s.Weakness == AttackType.Blue)
                                        {
                                            s2.Acceleration /= 2;
                                            if (CanEmp)
                                            {
                                                CanEmp = false;
                                                s2.EMP(this, IsUpdgraded ? 1 : 0);

                                                if (s2.TestTag(UnitTag.Monster))
                                                    SearchTime -= MaxSearchTime / 5;
                                            }
                                        }
                                        s2.MaxEngagementDistance /= 1.5f;
                                        s2.MinEngagementDistance /= 1.5f;
                                    }
                                }

                            }
                        }
                }
            }

            base.Update(gameTime);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(SlowFieldCard.STurretSize));
            RemoveTag(GameObjectTag._2DSolid);
        }

        protected override void Upgrade()
        {
            MaxEngagementDistance *= 2f;
            MaxSearchTime /= 2;
            base.Upgrade();
        }

        public override int GetIntType()
        {
            return InstanceManager.EmpireUnitIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Turret3", BlendState.Additive, DepthStencilState.DepthRead, 0.6f, 0.1f);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }
    }
}
