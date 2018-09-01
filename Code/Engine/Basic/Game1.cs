using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BadRabbit.Carrot
{
    public class Game1 : Game
    {
        public const string EngineTitle = "Carrot";
        public static GameTime gameTime;
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static ContentManager content;
        public static Game1 self;
        public static GraphicsDevice graphicsDevice;
        public static Color ClearColor = Color.Black;
        public static Random random = new Random();

        const float ResMult = 1f;
        public const int ResolutionX = (int)(1280 * ResMult);
        public const int ResolutionY = (int)(720 * ResMult);


        public Game1()
        {
            IsFixedTimeStep = false;
            self = this;
            graphics = new GraphicsDeviceManager(this);
#if XBOX
            graphics.PreferMultiSampling = true;
#endif

            Content.RootDirectory = "Content";
            content = Content;

            graphics.PreferredBackBufferHeight = ResolutionY;
            graphics.PreferredBackBufferWidth = ResolutionX;
            Window.Title = EngineTitle;

#if !WINDOWS || !EDITOR
            //graphics.IsFullScreen = true;
#endif
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsDevice = graphics.GraphicsDevice;
            MasterManager.Load();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
#if EDITOR
            if (GameManager.GetLevel() != null && GameManager.GetLevel().MyScene != null)
                GameManager.GetLevel().MyScene.UpdateTime.Start();
#endif

            Game1.gameTime = gameTime;
            if (gameTime.ElapsedGameTime.Milliseconds > 100)
                MasterManager.Update(new GameTime(gameTime.ElapsedGameTime, TimeSpan.FromMilliseconds(100)));
            else
                MasterManager.Update(gameTime);

#if EDITOR
            if (GameManager.GetLevel() != null && GameManager.GetLevel().MyScene != null)
                GameManager.GetLevel().MyScene.UpdateTime.Stop();
#endif

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
#if EDITOR
            if (GameManager.GetLevel() != null && GameManager.GetLevel().MyScene != null)
                GameManager.GetLevel().MyScene.DrawTime.Start();
#endif
            MasterManager.Draw();

#if EDITOR
            if (GameManager.GetLevel() != null && GameManager.GetLevel().MyScene != null)
                GameManager.GetLevel().MyScene.DrawTime.Stop();
#endif
            base.Draw(gameTime);
        }
    }
}
