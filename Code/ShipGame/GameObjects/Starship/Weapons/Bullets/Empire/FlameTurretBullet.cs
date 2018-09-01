using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class FlameTurretBullet : Bullet
    {
        static Color ParticleColor = new Color(0.3f, 0.1f, 0.025f);
        static Color ParticleColor2 = new Color(0.2f, 0.1f, 0.025f);

        public override void Create()
        {
            NoInstanceCommit = true;
            ImpactString = null;

            base.Create();

            Size.set(new Vector2(8));
        }

        public override void Update(GameTime gameTime)
        {
            float Mult = (TimeAlive * 4 / (float)(LifeTime) + 1) * (Big ? 2 : 1);
            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 0);
            
            base.Update(gameTime);
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());

            //ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 5);

            base.Destroy();
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            return Damage * Mult;
        }
    }
}
