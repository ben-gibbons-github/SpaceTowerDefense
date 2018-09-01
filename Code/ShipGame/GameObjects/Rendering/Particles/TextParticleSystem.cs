using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class TextParticleSystem
    {
        const int ParticleCount = 50;
        public const float ParticleMoveAmount = 100;
        public const float ParticleYOffset = -40;
        public const int ParticleLifeTime = 2000;

        public static SpriteFont TextParticleFont;
        static TextParticle[] ParticleArray;
        static int FirstParticle = 0;
        static int LastParticle = 0;
        static bool Loaded = false;
        static int CurrentTime = 0;

        public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;

                TextParticleFont = AssetManager.Load<SpriteFont>("Fonts/ShipGame/ParticleFont");

                ParticleArray = new TextParticle[ParticleCount];
                for (int i = 0; i < ParticleCount; i++)
                    ParticleArray[i] = new TextParticle();
            }
        }

        public static void Update(GameTime gameTime)
        {
            CurrentTime += gameTime.ElapsedGameTime.Milliseconds;
            while (FirstParticle != LastParticle && ParticleArray[FirstParticle].StartingTime + ParticleLifeTime < CurrentTime)
            {
                if (++LastParticle == ParticleCount)
                    LastParticle = 0;
            }

            if (FirstParticle == LastParticle)
                CurrentTime = 0;
        }

        public static void AddParticle(Vector3 Position, string Text, byte Team)
        {
            ParticleArray[FirstParticle].Text = Text;
            ParticleArray[FirstParticle].StartingTime = CurrentTime;
            ParticleArray[FirstParticle].Position = Position;
            ParticleArray[FirstParticle].color = TeamInfo.Colors[Team];
            ParticleArray[FirstParticle].TextOffset = TextParticleFont.MeasureString(Text).X / 2;
            ParticleArray[FirstParticle].Icon = null;
            ParticleArray[FirstParticle].ULBox = new Vector2(-TextParticleFont.MeasureString(Text).X / 2 - 2, - 2);
            ParticleArray[FirstParticle].LRBox = new Vector2(TextParticleFont.MeasureString(Text).X / 2 + 2, TextParticleFont.MeasureString(Text).Y + 2);

            if (++FirstParticle == ParticleCount)
                FirstParticle = 0;
        }

        public static void AddParticle(Vector3 Position, string Text, byte Team, Texture2D Icon)
        {
            ParticleArray[FirstParticle].Text = Text;
            ParticleArray[FirstParticle].StartingTime = CurrentTime;
            ParticleArray[FirstParticle].Position = Position;
            ParticleArray[FirstParticle].color = TeamInfo.Colors[Team];
            ParticleArray[FirstParticle].TextOffset = TextParticleFont.MeasureString(Text).X / 2;
            ParticleArray[FirstParticle].Icon = Icon;
            ParticleArray[FirstParticle].IconOffset = new Vector2(-Icon.Width, (TextParticleFont.MeasureString(Text).Y - Icon.Height) / 2);
            ParticleArray[FirstParticle].ULBox = new Vector2(-Icon.Width / 2 - 2, -2);
            ParticleArray[FirstParticle].LRBox = new Vector2(TextParticleFont.MeasureString(Text).X / 2 + 2, Icon.Height + 2);

            if (++FirstParticle == ParticleCount)
                FirstParticle = 0;
        }

        public static void Draw(Camera3D DrawCamera)
        {
            if (FirstParticle > LastParticle)
            {
                for (int i = LastParticle; i < FirstParticle; i++)
                    ParticleArray[i].Draw(DrawCamera, CurrentTime);
            }
            else if (FirstParticle < LastParticle)
            {
                for (int i = LastParticle; i < ParticleCount; i++)
                    ParticleArray[i].Draw(DrawCamera, CurrentTime);

                for (int i = 0; i < FirstParticle; i++)
                    ParticleArray[i].Draw(DrawCamera, CurrentTime);
            }
        }
    }
}
