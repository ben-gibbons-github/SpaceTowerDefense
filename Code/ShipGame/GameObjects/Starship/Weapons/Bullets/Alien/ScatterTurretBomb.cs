using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ScatterTurretBomb : Bullet
    {
        static FireMode scatterTurretFireMode;
        static Color ParticleColor = new Color(0.15f, 0.35f, 0.3f);
        private Vector2 OriginalSpeed;

        bool Flashed = false;

        public ScatterTurretBomb()
        {
            BulletCanBounce = false;
            ImpactString = "ScatterTurretImpact";
            ImpactVolume = 0.75f;
            ImpactDistance = 1000;

            if (scatterTurretFireMode == null)
                scatterTurretFireMode = new ScatterBombFireMode();
        }

        public override void Update(GameTime gameTime)
        {
            float Mult = 5;

            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (60 + Rand.F() * 20) * Mult, 2);


            if (!Flashed)
            {
                Mult = 6;
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
            Size.set(new Vector2(60));
        }

        public override void Destroy()
        {
            Vector2 PreviousPosition = ParentUnit.Position.get();
            ParentUnit.Position.set(Position.get());

            scatterTurretFireMode.SetParent(ParentUnit);

            for (int i = 0; i < 8; i++)
                scatterTurretFireMode.Fire((float)Math.PI * 2 / 8f * i);

            ParentUnit.Position.set(PreviousPosition);

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 10, ParticleColor, 50, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 0);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 4);

            base.Destroy();
        }
    }
}
