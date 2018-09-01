using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PolorPoint
    {
        Vector3 NewPosition;
        Vector3 OldPosition;
        public Vector3 DrawPosition;
        float Interpolation;
        float T;

        public PolorPoint()
        {
        }

        public Vector3 Function()
        {
            T += 20;

            float Theta = T / 100f;
            float R = ((float)Math.Sin(0.3 * Theta) * 30);
            return new Vector3((float)Math.Sin(Theta) * R, 0, (float)Math.Cos(Theta) * R);
        }

        public void Update(GameTime gameTime)
        {
            Interpolation += gameTime.ElapsedGameTime.Milliseconds / (2f * Math.Max(1, Vector3.Distance(OldPosition, NewPosition)));
            while (Interpolation > 1)
            {
                OldPosition = NewPosition;
                NewPosition = Function();

                LineParticleSystem.AddParticle(OldPosition, NewPosition, Color.White);

                Interpolation = 0;
            }
            DrawPosition = Vector3.Lerp(OldPosition, NewPosition, Interpolation);

            ParticleManager.CreateParticle(DrawPosition, Vector3.Zero, Color.White, 10, 1);
            ParticleManager.CreateParticle(DrawPosition, Vector3.Zero, Color.White * 0.1f, 10, 5);
        }
    }
}
