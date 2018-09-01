using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class LineModelFlare
    {
        LineModelItem Parent;
        float Interpolation;
        float ScaleSize;
        int CurrentPosition;
        
        public LineModelFlare(LineModelItem Parent, int CurrentPosition, float ScaleSize)
        {
            this.Parent = Parent;
            this.CurrentPosition = CurrentPosition;
            this.ScaleSize = ScaleSize;
        }

        public void Update(GameTime gameTime)
        {
            Vector3 OldPosition = Parent.Points[CurrentPosition];
            Vector3 NewPosition = CurrentPosition + 1 < Parent.Points.Count ? Parent.Points[CurrentPosition + 1] : Parent.Points[0];
            
            Interpolation += gameTime.ElapsedGameTime.Milliseconds / (20f * Math.Max(1, Vector3.Distance(OldPosition, NewPosition)));
            while (Interpolation > 1)
            {
                OldPosition = Parent.Points[CurrentPosition];
                NewPosition = CurrentPosition + 1 < Parent.Points.Count ? Parent.Points[CurrentPosition + 1] : Parent.Points[0];

                if (Vector3.Distance(OldPosition, NewPosition) < Parent.Distance)
                    LineParticleSystem.AddParticle(OldPosition, NewPosition, Color.White);

                Interpolation = 0;
                CurrentPosition++;
                if (CurrentPosition > Parent.Points.Count - 1)
                    CurrentPosition = 0;

                if (!Parent.BuiltPoints.Contains(Parent.Points[CurrentPosition]))
                    Parent.BuiltPoints.Add(Parent.Points[CurrentPosition]);
            }

            OldPosition = Parent.Points[CurrentPosition];
            NewPosition = CurrentPosition + 1 < Parent.Points.Count ? Parent.Points[CurrentPosition + 1] : Parent.Points[0];
            Vector3 InterpolatedPosition = Vector3.Lerp(OldPosition, NewPosition, Interpolation);

            ParticleManager.CreateParticle(InterpolatedPosition, Vector3.Zero, Color.White, ScaleSize * 0.0005f, 1);
            ParticleManager.CreateParticle(InterpolatedPosition, Vector3.Zero, Color.White * 0.1f, ScaleSize * 0.0005f, 5);
        }
    }
}
