using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class LineModelRandomFlare
    {
        LineModelItem Parent;
        float Interpolation;
        float ScaleSize;

        Vector3 To;
        Vector3 From;

        public LineModelRandomFlare(LineModelItem Parent, float ScaleSize)
        {
            this.Parent = Parent;
            this.ScaleSize = ScaleSize;
            To = Rand.V3() * ScaleSize / 25f;
        }

        public void Update(GameTime gameTime)
        {
            Interpolation += gameTime.ElapsedGameTime.Milliseconds / (20f * Math.Max(1, Vector3.Distance(To, From)));
            while (Interpolation > 1)
            {
                Interpolation -= 1;
                LineParticleSystem.AddParticle(To, From, Color.White);
                From = To;
                To = Rand.V3() * ScaleSize / 25f;
            }

            Vector3 InterpolatedPosition = Vector3.Lerp(From, To, Interpolation);

            ParticleManager.CreateParticle(InterpolatedPosition, Vector3.Zero, Color.White, ScaleSize * 0.001f, 1);
        }
    }
}
