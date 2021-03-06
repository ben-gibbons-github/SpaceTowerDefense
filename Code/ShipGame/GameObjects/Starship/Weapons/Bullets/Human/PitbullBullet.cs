﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PitbullBullet : Bullet
    {
        static Color ParticleColor = new Color(0.1f, 0.25f, 0.25f);

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(16));
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 5);
            FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 4, Vector3.Zero, Rand.V3(), Vector3.Zero, 20, 10, ParticleColor.ToVector3(), ParticleColor.ToVector3(), 0, 2);

            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 50, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 100 + Rand.F() * 100, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 25 + Rand.F() * 25, 2);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 25 + Rand.F() * 25, 0);

            base.Update(gameTime);
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            if (s.TestTag(UnitTag.Building))
                Mult *= 0.2f;

            return base.getDamage(s, Mult);
        }
    }
}
