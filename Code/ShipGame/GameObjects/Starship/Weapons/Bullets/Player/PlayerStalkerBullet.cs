using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerStalkerBullet : Bullet
    {
        static Color ParticleColor = new Color(0.25f, 0.1f, 0.4f);
        static Color ParticleColor2 = new Color(0.5f, 0.1f, 1f);

        public override void Create()
        {
            ShouldDodge = true;
            base.Create();
            Size.set(new Vector2(20));
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
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 120, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 150 + Rand.F() * 100, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 20 + Rand.F() * 40, 2);

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
