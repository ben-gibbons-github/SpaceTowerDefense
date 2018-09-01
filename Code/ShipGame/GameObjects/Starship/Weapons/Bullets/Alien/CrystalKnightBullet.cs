using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class CrystalKnightBullet : Bullet
    {
        static Color ParticleColor = new Color(0.3f, 0.2f, 0.4f);

        public UnitTurret FirstHitTurret;
        public float Level = 1;

        bool Flashed = false;

        public CrystalKnightBullet(float Level)
        {
            this.Level = Level;
            NoInstanceCommit = true;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(16));
        }

        public override void Collide(BasicShipGameObject s)
        {
            return;
        }

        public override void Update(GameTime gameTime)
        {
            float Mult = 0.1f + TimeAlive / (float)LifeTime;
            float Offset = 200 * Mult;
            Vector3 Position3 = new Vector3(0, ParentUnit.Y, 0);

            float Theta = -Logic.ToAngle(Speed) - (float)Math.PI * 1f;
            float TargetTheta = Theta + (float)(Math.PI * 1.05);
            float Alpha = 1 - (float)LifeTime / TimeAlive;

            for (;Theta < TargetTheta; Theta += (float)Math.PI / 20)
            {
                Position3.X = Position.X() + (float)Math.Cos(Theta) * Offset;
                Position3.Z = Position.Y() + (float)Math.Sin(Theta) * Offset;
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor * Alpha, Offset * 4, 1);
                //ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, Offset, 0);
            }

            Position3.X = Position.X();
            Position3.Z = Position.Y();

            if (!Flashed)
            {
                Mult *= 2;
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 0);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 4);
                Flashed = true;
            }


            QuadGrid quad = Parent2DScene.quadGrids.First.Value;
            foreach(Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(Offset * 2)))
                if (o.GetType().IsSubclassOf(typeof(UnitTurret)) && o != FirstHitTurret && 
                    Vector2.Distance(o.Position.get(), Position.get()) < Offset)
                {
                    UnitTurret u = (UnitTurret)o;
                    if (u.CanBeTargeted())
                    {
                        if (FirstHitTurret == null)
                            FirstHitTurret = u;
                        else
                            u.ShutDownTime = (int)(1000 * Level);
                    }
                }

            base.Update(gameTime);
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            return base.getDamage(s, Mult);
        }
    }
}
