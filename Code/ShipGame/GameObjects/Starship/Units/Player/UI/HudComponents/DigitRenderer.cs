using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class DigitRenderer
    {
        static Texture2D DigitTexture;

        static DigitRenderer()
        {
            DigitTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/Digits");
        }

        public static void DrawDigits(int Value, int Places, Vector2 Position, Vector2 Size, Color DigitColor)
        {
            int Max = 10;
            for (int i = 1; i < Places; i++)
                Max *= 10;

            if (Value > Max - 1)
                Value = Max - 1;

            Position.X += Size.X / 2;
            Position.Y -= Size.Y / 2;
            for (int i = 0; i < Places; i++)
            {
                Rectangle TargetRect = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X / Places, (int)Size.Y);
                Rectangle SourceRect = new Rectangle(DigitTexture.Width * (Value % 10) / 10, 0, DigitTexture.Width / 10, DigitTexture.Height);
                Game1.spriteBatch.Draw(DigitTexture, TargetRect, SourceRect, DigitColor);

                Position.X -= Size.X / Places;
                Value /= 10;
            }
        }
    }
}
