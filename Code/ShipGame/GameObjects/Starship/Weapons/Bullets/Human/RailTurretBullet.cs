using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class RailTurretBullet : Bullet
    {
        static Color ParticleColor = new Color(0.2f, 0.45f, 0.35f);
        private Vector2 OriginalSpeed;

        bool Flashed = false;

        public RailTurretBullet()
        {
            ShouldDodge = true;
            ImpactString = "RailTurretImpact";
            ImpactVolume = 0.35f;
        }

        public override void Update(GameTime gameTime)
        {
            int Mult = TimeAlive * 4 / LifeTime + 1;

            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 120 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, (10 + Rand.F() * 20) * Mult, 2);

            if (!Flashed)
            {
                Mult = 2;
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
            Armed = true;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 20, ParticleColor, 40, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 5);
               
            base.Destroy();
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            BulletExplosionDistance *= 2;

            if (s.TestTag(UnitTag.Player))
                Mult *= 0.3f * PlayerWeapon.GetTurretVsPlayer();

            return base.getDamage(s, Mult);
        }
    }
}
