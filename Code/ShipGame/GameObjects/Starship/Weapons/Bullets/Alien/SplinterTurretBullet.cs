using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SplinterTurretBullet : Bullet
    {
        static Color ParticleColor = new Color(0.35f, 0.05f, 0.15f);
        private Vector2 OriginalSpeed;

        bool Flashed = false;

        int SearchTime = 0;
        public int MaxSearchTime = 100;

        UnitBasic CurrentAttackTarget;

        public float SearchDistance = 150;

        public SplinterTurretBullet()
        {
            BulletCanBounce = false;
            ImpactString = "SplinterTurretImpact";
            ImpactVolume = 0.1f;
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
                                    if (!u.IsAlly(ParentUnit))
                                    {
                                        CurrentAttackTarget = u;
                                        BestDistance = d;
                                    }
                                }
                            }
                    }
                }
            }

            float Mult = (TimeAlive * 2 / LifeTime + 1) * 0.5f;

            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (60 + Rand.F() * 20) * Mult, 2);
            
            if (!Flashed)
            {
                Mult = 1.5f;
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 0);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 4);
                Flashed = true;
            }

            base.Update(gameTime);
        }

        public override void SetSpeed(Vector2 Speed)
        {
            OriginalSpeed = Speed;
            base.SetSpeed(Speed);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(10));
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 200, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 40 * 3, 0);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 40 * 3, 4);

            base.Destroy();
        }
    }
}
