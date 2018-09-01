using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FactionEvent
    {
        public static SpriteFont FeedFont;
        public static Texture2D DeathTexture;
        public static Texture2D LossTexture;
        public static Texture2D ReadyTexture;
        public static Texture2D KillTexture;
        public static Texture2D DefenseTexture;
        static bool Loaded = false;

        public static int MaxLifeTime = 2500;

        public Texture2D EventTexture;
        public string EventString = "";
        public Color EventColor = Color.White;
        public int LifeTime;

        Vector2 PositionOffset;
        float Alpha = 0;

        public void Create(string EventString, Color color, Texture2D Texture)
        {
            this.EventColor = color;
            this.EventString = EventString;
            this.EventTexture = Texture;

            PositionOffset = Vector2.Zero;
            LifeTime = 0;
        }

        static FactionEvent()
        {
            if (!Loaded)
            {
                Loaded = true;

                DeathTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/Dead");
                DefenseTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/TacticDefend");
                KillTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/TacticAggressive");
                LossTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/Warning");
                ReadyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/Ready");
                FeedFont = AssetManager.Load<SpriteFont>("Fonts/ShipGame/FeedFont");
            }
        }

        public void Draw(Vector2 Position)
        {
            if (EventTexture != null)
                Render.DrawSprite(EventTexture, Position + PositionOffset + new Vector2(0, -64), new Vector2(64), 0, Color.White * Alpha);
            Render.DrawShadowedText(FeedFont, EventString, Position + PositionOffset - FeedFont.MeasureString(EventString) / 2, Vector2.Zero, EventColor * Alpha, Color.Black * Alpha);
        }

        public bool Update(GameTime gameTime)
        {
            LifeTime += gameTime.ElapsedGameTime.Milliseconds;
            if (LifeTime > MaxLifeTime)
            {
                return true;
            }

            if (LifeTime < 200)
            {
                Alpha = LifeTime / 200f;
                PositionOffset.Y = 200 - LifeTime;
            }
            else
            {
                if (LifeTime > 2000)
                {
                    PositionOffset.X = LifeTime - 2000;
                    Alpha = (2500 - LifeTime ) / 500f;
                }
                else
                {
                    PositionOffset = Vector2.Zero;
                    Alpha = 1;
                }
            }

            return false;
        }
    }
}
