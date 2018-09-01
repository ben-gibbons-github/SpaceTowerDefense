using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class VampireBullet : Bullet
    {
        static Color ParticleColor = new Color(0.3f, 0.1f, 0.3f);

        bool Flashed = false;

        public override void Create()
        {
            ImpactString = "VampireImpact";
            ImpactVolume = 0.5f;

            base.Create();
            Size.set(new Vector2(16 * (Big ? 2 : 1)));
        }

        public override void Update(GameTime gameTime)
        {
            int Mult = Big ? 2 : 1;
            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (100 + Rand.F() * 100) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (20 + Rand.F() * 40) * Mult, 2);

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
                Mult *= 4f;
            if (s.TestTag(UnitTag.Ring))
                Mult *= 0.2f;

            return base.getDamage(s, Mult);
        }
    }
}
