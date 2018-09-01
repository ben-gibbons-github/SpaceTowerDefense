using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlasmaTurretBullet : Bullet
    {
        static Color ParticleColor = new Color(0.3f, 0.175f, 0.15f);
        static Color ParticleColor2 = new Color(0.2f, 0.1f, 0.05f);

        bool Flashed = false;

        public override void Create()
        {
            ShouldDodge = true;
            ImpactString = "PlasmaTurretImpact";
            ImpactVolume = 0.1f;

            base.Create();
            Size.set(new Vector2(20));
        }

        public override void Update(GameTime gameTime)
        {
            int Mult = TimeAlive * 4 / LifeTime + 1;

            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 90 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 2);

            if (!Flashed)
            {
                Mult = 2;
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 0);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80 * Mult, 4);
                Flashed = true;
            }

            base.Update(gameTime);
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 5);

            Armed = true;

            //if (TimeAlive > LifeTime * 2 / 3)
            {
                BulletExplosionDamage = Damage / 2;
                BulletExplosionDistance = 150;

                    FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 1.5f, new Vector3(0, -0.25f, 0),
                        Rand.V3(), Rand.V3() / 10, 15, 30, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 5, 4);
                for (int i = 0; i < 30; i++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);
            }

            base.Destroy();
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            if (s.TestTag(UnitTag.Player))
                Mult *= PlayerWeapon.GetTurretVsPlayer();

            return Damage * Mult;
        }
    }
}
