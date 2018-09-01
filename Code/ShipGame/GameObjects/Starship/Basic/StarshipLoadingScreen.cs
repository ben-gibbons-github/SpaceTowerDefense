using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class StarshipLoadingScreen : LoadingScreen
    {
        static Texture2D Gear;
        static Texture2D Gear2;
        static Texture2D Gear3;
        static Texture2D Gear4;
        static Texture2D GearShip;

        static bool Loaded;

        public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;

                Gear = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/LoadingScreen/Gear");
                Gear2 = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/LoadingScreen/Gear2");
                Gear3 = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/LoadingScreen/Gear3");
                Gear4 = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/LoadingScreen/Gear4");
                GearShip = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/LoadingScreen/GearShip");
            }
        }

        public override void Draw(int Completed, int Max, float Alpha)
        {
            if (Alpha > 0)
            {
                Load();
                Game1.spriteBatch.Begin();

                Vector2 Position = new Vector2(Game1.ResolutionX, Game1.ResolutionY) / 2;
                Vector2 Size = new Vector2(Position.Length() * Alpha);
                Alpha *= 0.75f + Rand.F() * 0.25f;

                Render.DrawSprite(Gear, Position, Size, Level.Time / 10f, Color.White * Alpha);
                Size *= 0.8f;

                Render.DrawSprite(Gear2, Position, Size, Level.Time / 5f, Color.White * (Alpha / 2));
                Size *= 0.8f;

                Render.DrawSprite(Gear3, Position, Size, Level.Time / 2.5f, Color.White * (Alpha / 2));
                Size *= 0.8f;

                Render.DrawSprite(Gear4, Position, Size, Level.Time, Color.White * (Alpha / 4));
                Size *= 0.9f;

                Render.DrawSprite(GearShip, Position, Size, 0, Color.White * (Alpha / 4));
                Size *= 0.5f;

                Position.Y += Size.Y;

                Render.DrawSolidRect(Position - Size * new Vector2(1, 0.2f),
                    Position + Size * new Vector2(1, 0.2f), Color.Black * Alpha);
                Size *= 0.85f;
                Render.DrawSolidRect(Position - Size * new Vector2(1, 0.2f),
                    Position + new Vector2(-Size.X + 2 * Size.X * (float)Completed / Max, Size.Y * 0.2f), Color.White * Alpha);

                Game1.spriteBatch.End();
            }
        }
    }
}
