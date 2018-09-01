using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class WaveCardText
    {
        public static SpriteFont Font;
        static float AlphaChange = 0.1f;

        public string Text;
        public float Alpha;

        static WaveCardText()
        {
            Font = AssetManager.Load<SpriteFont>("Fonts/ShipGame/EventFont");
        }

        public WaveCardText(string Text)
        {
            this.Text = Text;
        }

        public bool Update(GameTime gameTime)
        {
            Alpha += AlphaChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
            if (Alpha > 1)
            {
                Alpha = 1;
                return true;
            }
            else
                return false;
        }

        public void Draw(Vector2 Position, float Alpha)
        {
            if (Alpha > 0)
            {
                Position.X += (1 - Alpha) * 200 + 50;
                Render.DrawShadowedText(Font, Text, Position, Vector2.One, Color.White * Alpha, Color.Black * Alpha);
            }
        }
    }
}
