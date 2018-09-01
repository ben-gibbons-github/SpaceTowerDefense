using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class HornetBullet : Bullet
    {
        static Color ParticleColor = new Color(0.25f, 0.175f, 0.15f);

        bool Flashed = false;

        public override void Create()
        {
            ImpactString = "HornetImpact";
            ImpactVolume = 0.35f;

            base.Create();
            Size.set(new Vector2(16) * (Big ? 2 : 1));
        }

        public override void Destroy()
        {
            int Mult = Big ? 2 : 1;

            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor, 20 * Mult, 5);

            FlareSystem.AddLightingPoint(Position3, new Vector3(1, 0.25f, 0.25f), new Vector3(0.5f, 0, 0.25f), 5 * Mult, 20 * Mult, 3, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400 * Mult, 5);
            for (int i = 0; i < 3; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 4, Vector3.Zero, Rand.V3() * Mult, Vector3.Zero, 20 * Mult, 10, ParticleColor.ToVector3(), ParticleColor.ToVector3(), 0, 2);


            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            int Mult = Big ? 2 : 1;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (100 + Rand.F() * 100) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (20 + Rand.F() * 40) * Mult, 2);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (20 + Rand.F() * 40) * Mult, 0);

            if (!Flashed)
            {
                Mult *= 2;
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 0);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 4);
                Flashed = true;
            }

            base.Update(gameTime);
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            if (s.TestTag(UnitTag.Building))
                Mult *= 2f;
            if (Big)
                Mult *= 3;

            return base.getDamage(s, Mult);
        }
    }
}
