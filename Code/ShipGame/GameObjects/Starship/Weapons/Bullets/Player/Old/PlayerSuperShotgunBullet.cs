using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerSuperShotGunBullet : Bullet
    {
        static Color ParticleColor = new Color(0.25f, 0.1f, 0.2f);

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(32));
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 400, ParticleColor, 50, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 5);
            FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 4, Vector3.Zero, Rand.V3(), Vector3.Zero, 100, 10, ParticleColor.ToVector3(), ParticleColor.ToVector3(), 0, 2);

            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            float Mult = 2;
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 100 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (200 + Rand.F() * 200) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (50 + Rand.F() * 50) * Mult, 2);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (50 + Rand.F() * 50) * Mult, 0);

            base.Update(gameTime);
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            if (s.TestTag(UnitTag.Building))
                Mult *= PlayerWeapon.GetTurretMult();
            else if (s.TestTag(UnitTag.Player))
                Mult *= PlayerWeapon.GetPlayerMult();

            return base.getDamage(s, Mult);
        }
    }
}
