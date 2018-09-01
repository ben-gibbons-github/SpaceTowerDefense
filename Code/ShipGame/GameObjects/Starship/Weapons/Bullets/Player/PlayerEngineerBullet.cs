using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerEngineerBullet : Bullet
    {
        static Color ParticleColor = new Color(0.15f, 0.025f, 0.1f);
        static Color ParticleColor2 = new Color(0.5f, 0.1f, 0.3f);

        Vector2 BaseSpeed;

        int SearchTime = 0;
        public int MaxSearchTime = 100;

        UnitBasic CurrentAttackTarget;
        UnitBasic AttachedUnit;
        Vector2 AttachedOffset;

        public float SearchDistance = 150;

        int MinCollideTimeAlive = 800;

        public override void SetSpeed(Vector2 Speed)
        {
            BaseSpeed = Speed;
            base.SetSpeed(Speed);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(24));
        }

        public override void Collide(BasicShipGameObject s)
        {
            if (!s.IsAlly(ParentUnit) && s.GetType().IsSubclassOf(typeof(UnitBasic)))
            {
                AttachedUnit = (UnitBasic)s;
                AttachedOffset = Vector2.Normalize(s.Position.get() - Position.get()) * s.Size.X() / 2;

                if (TimeAlive < MinCollideTimeAlive)
                    TimeAlive = MinCollideTimeAlive;
            }
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 4);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 7);


            for (int i = 0; i < 2; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 1.5f, new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 15, 30, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 5, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);

            Armed = true;
            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            if (!BulletHasBounced)
            {
                if (CurrentAttackTarget != null && CurrentAttackTarget.CanBeTargeted())
                {
                    Speed = Vector2.Normalize(CurrentAttackTarget.Position.get() - Position.get()) * Speed.Length();
                }
                else
                {
                    if (CurrentAttackTarget == null)
                        SearchTime += gameTime.ElapsedGameTime.Milliseconds;
                    else
                        SearchTime = MaxSearchTime + 1;

                    if (SearchTime > MaxSearchTime)
                    {
                        CurrentAttackTarget = null;
                        SearchTime -= MaxSearchTime;

                        QuadGrid quad = Parent2DScene.quadGrids.First.Value;
                        float BestDistance = SearchDistance;

                        foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(SearchDistance)))
                            if (o.GetType().IsSubclassOf(typeof(UnitBasic)) && !o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                            {
                                float d = Vector2.Distance(Position.get(), o.Position.get());
                                if (d < BestDistance)
                                {
                                    UnitBasic u = (UnitBasic)o;
                                    if (u.CanBeTargeted() && !ParentUnit.IsAlly(u))
                                    {
                                        CurrentAttackTarget = u;
                                        BestDistance = d;
                                    }
                                }
                            }
                    }
                }
            }

            float Mult = (TimeAlive - MinCollideTimeAlive) / (LifeTime - MinCollideTimeAlive) * 8;
            if (Mult < 1)
                Mult = 1;
            Mult *= 3;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            FlareSystem.AddLightning(Position3, ParticleColor2, 10, 70, 3, 6);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 2);
            base.Update(gameTime);
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            if (s.TestTag(UnitTag.Building))
            {
                if (s.TestTag(UnitTag.Ring))
                    Mult *= PlayerWeapon.GetRingMult();
                else
                    Mult *= PlayerWeapon.GetTurretMult();
            }
            else if (s.TestTag(UnitTag.Player))
                Mult *= PlayerWeapon.GetPlayerMult();
            return base.getDamage(s, Mult);
        }
    }
}
