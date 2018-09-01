using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SliderArrow
    {
        public static Texture2D ArrowTexture;

        static SliderArrow()
        {
            ArrowTexture = AssetManager.Load<Texture2D>("_Engine/SliderArrow");
        }

        static float AlphaChange = 0.1f;

        float Rotation;
        float OffsetDirection = 1;

        BasicGameForm Parent;
        public float FlashAlpha;

        public SliderArrow(BasicGameForm Parent, float OffsetDirection, float Rotation)
        {
            this.Parent = Parent;

            this.OffsetDirection = OffsetDirection;
            this.Rotation = Rotation;
        }

        public void Update(GameTime gameTime)
        {
            FlashAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 5000f * AlphaChange;
        }

        public void Draw(Vector2 Position, Vector2 Size)
        {
            Position += new Vector2((Size.X + Size.Y) / 2 * OffsetDirection, 0);
            Render.DrawSprite(ArrowTexture, Position, new Vector2(Size.Y), Rotation, Color.White * Parent.Alpha);
            if (FlashAlpha > 0)
                Render.DrawSprite(HudItem.GlowTexture, Position, new Vector2(Size.Y) * 8, Rotation, Color.White * (Parent.Alpha * FlashAlpha / 2));
        }
    }
}
