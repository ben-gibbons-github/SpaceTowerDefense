using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerKnightBullet : Bullet
    {
        static Color ParticleColor = new Color(0.3f, 0.3f, 0.3f);
        static Color ParticleColor2 = new Color(0.3f, 0.3f, 0.3f);

        public override void Create()
        {
            ShouldDodge = true;
            base.Create();
            Size.set(new Vector2(14));
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor2 * 2, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 400, 5);
            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            Vector3 PositionTo = new Vector3(Position.X() + Speed.X * 8, 0, Position.Y() + Speed.Y * 8);
            Vector3 PositionFrom = new Vector3(Position.X() - Speed.X * 8, 0, Position.Y() - Speed.Y * 8);

            for (int i = 0; i < 4; i++)
                ParticleManager.CreateParticle(Position3 + (Position3 - PositionTo) * i / 4f, Vector3.Zero, ParticleColor2 * ((3 - i) / 3f), Size.X() * 6, 1);

            for (int i = 0; i < 4; i++)
                ParticleManager.CreateParticle(Position3 + (Position3 - PositionFrom) * i / 4f, Vector3.Zero, ParticleColor2 * ((3 - i) / 3f), Size.X() * 6, 1);

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
