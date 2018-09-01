using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class TextParticle
    {
        public Texture2D Icon;
        public string Text;
        public int StartingTime = 0;
        public Vector3 Position;
        public Color color;
        public float TextOffset = 0;
        public Vector2 IconOffset;

        public Vector2 ULBox;
        public Vector2 LRBox;

        public void Draw(Camera3D DrawCamera, int CurrentTime)
        {
            float AlphaMult = 1 - (CurrentTime - StartingTime) / (float)TextParticleSystem.ParticleLifeTime;

            Vector3 Position3 = Game1.graphicsDevice.Viewport.Project(Position, DrawCamera.ProjectionMatrix, DrawCamera.ViewMatrix, Matrix.Identity);
            Vector2 Position2 = new Vector2(Position3.X, Position3.Y) - Render.CurrentView.Position;
            
            Position2.X -= TextOffset;
            Position2.Y -= (1 - AlphaMult) * TextParticleSystem.ParticleMoveAmount - TextParticleSystem.ParticleYOffset;

            //Render.DrawOutlineRect(Position2 + ULBox, Position2 + LRBox, 1, Col);

            if (Icon != null)
                Game1.spriteBatch.Draw(Icon, Position2 + IconOffset, Color.White * AlphaMult);

            Render.DrawShadowedText(TextParticleSystem.TextParticleFont, Text, Position2, Vector2.One,
               color * AlphaMult, Color.Black * AlphaMult);
        }
    } 
}
