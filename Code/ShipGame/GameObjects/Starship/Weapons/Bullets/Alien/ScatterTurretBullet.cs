using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ScatterTurretBullet : Bullet
    {
        static Color ParticleColor = new Color(0.15f, 0.35f, 0.3f);
        private Vector2 OriginalSpeed;

        bool Flashed = false;

        public ScatterTurretBullet()
        {
            BulletCanBounce = false;
            ImpactString = "VampireImpact";
            ImpactVolume = 0.25f;
        }

        public override void Update(GameTime gameTime)
        {
            float Mult = (TimeAlive * 4 / LifeTime + 1) * 0.5f;

            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (60 + Rand.F() * 20) * Mult, 2);


            if (!Flashed)
            {
                Mult = 4;
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
            Size.set(new Vector2(20));
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 40 * 3, 0);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 40 * 3, 4);

            base.Destroy();
        }
    }
}
