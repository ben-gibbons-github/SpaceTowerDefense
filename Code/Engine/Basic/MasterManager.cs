using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace BadRabbit.Carrot
{
    public class MasterManager
    {
        public static bool AllowFullScreen = false;
        private static bool NeedSizeChange = false;
        private static int TargetSizeX = Game1.ResolutionX;
        private static int TargetSizeY = Game1.ResolutionY;
        public static Viewport FullScreenViewport = new Viewport(0, 0, Game1.ResolutionX, Game1.ResolutionY);
        public static Vector2 FullScreenSize = new Vector2(Game1.ResolutionX, Game1.ResolutionY);
        public static Rectangle ProfileMessageRect = new Rectangle(Game1.ResolutionX - 150, 50, 100, 100);
        public static Exception e;
        private static bool Loaded = false;

        public static void Load()
        {
            if (Loaded)
                return;
            Loaded = true;

            CreatorBasic.Load();
            PlayerProfile.Load();

            Render.Load();
#if !EDITOR
            LoadDefaultLevel();
#endif

#if !EDITOR && AUTO
            DefaultLoader.AutoLoad();
#endif

#if EDITOR
#if !XBOX
            EditorManager.InitEditor();
#endif
#endif
        }

        public static void Update(GameTime gameTime)
        {
#if WINDOWS
            if (Game1.self.IsActive)
            {
                MouseManager.Update(gameTime);
                KeyboardManager.Update(gameTime);
            }

#endif
            bool Loading = Level.LoadLevels();

#if EDITOR
#if !XBOX
            EditorManager.Update(gameTime);
#endif
#if XBOX
            RecieverManager.Update(gameTime);
#endif
#endif

#if !EDITOR
                GameManager.Update(gameTime);
#endif
        }

        public static void Draw()
        {
#if WINDOWS
            if (NeedSizeChange)
            {
                NeedSizeChange = false;

                Game1.graphics.PreferredBackBufferWidth = TargetSizeX;
                Game1.graphics.PreferredBackBufferHeight = TargetSizeY;
                GameManager.SetSize(new Vector2(TargetSizeX, TargetSizeY));
                return;
            }
#endif

#if EDITOR
#if WINDOWS
            if (MasterManager.FullScreenViewport.Width != Game1.self.Window.ClientBounds.Width ||
                MasterManager.FullScreenViewport.Height != Game1.self.Window.ClientBounds.Height)
            {
                MasterManager.FullScreenSize = new Vector2(Game1.self.Window.ClientBounds.Width, Game1.self.Window.ClientBounds.Height);
                MasterManager.FullScreenViewport = new Viewport(0, 0, Game1.self.Window.ClientBounds.Width, Game1.self.Window.ClientBounds.Height);
                MasterManager.ProfileMessageRect.X = Game1.self.Window.ClientBounds.Width - 150;
            }
            EditorManager.Draw();
#endif
#if XBOX
            RecieverManager.Draw();
#endif
#endif

#if !EDITOR
            GameManager.Draw();
#endif
        }

        public static void ChangeSize(int Width, int Height)
        {
            NeedSizeChange = true;
            TargetSizeX = Width;
            TargetSizeY = Height;
        }

        public static void SetViewportToFullscreen()
        {
            try
            {
                Game1.graphicsDevice.Viewport = FullScreenViewport;
            }
            catch (Exception e) 
            {
                MasterManager.e = e;
                Console.WriteLine(e.Message);
            }
        }

        private static void LoadDefaultLevel()
        {
            GameManager.LoadTitleLevel("startingscreen");
        }
    }
}
