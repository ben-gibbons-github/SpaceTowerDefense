using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerGrenadeBullet : Bullet
    {
        static Color ParticleColor = new Color(0.15f, 0.075f, 0.025f);
        static Color ParticleColor2 = new Color(0.5f, 0.25f, 0.1f);

        Vector2 BaseSpeed;

        public override void SetSpeed(Vector2 Speed)
        {
            BaseSpeed = Speed;
            base.SetSpeed(Speed);
        }

        public override void Create()
        {
            ShouldDodge = true;
            base.Create();
            Size.set(new Vector2(24));
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 4);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 7);

            
            for (int i = 0; i < 2; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 1.5f, new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 15, 30, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 5, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);

            Armed = true;
            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            float Mult = TimeAlive * 8 / (float)(LifeTime - 8);
            if (Mult < 1)
                Mult = 1;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 2);
            base.Update(gameTime);
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            if (s.TestTag(UnitTag.Building))
            {
                if (s.TestTag(UnitTag.Ring))
                    Mult *= PlayerWeapon.GetRingMult();
                else
                    Mult *= PlayerWeapon.GetTurretMult();
            }
            else if (s.TestTag(UnitTag.Player))
                Mult *= PlayerWeapon.GetPlayerMult();
            return base.getDamage(s, Mult);
        }
    }
}
