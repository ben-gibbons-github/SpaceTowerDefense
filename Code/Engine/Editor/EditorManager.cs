#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class EditorManager
    {
        public static Editor MyEditor;
        public static bool EditorMode = false;
        public static bool LevelFromEditor = false;

        public static int EditorSizeX;
        public static int EditorSizeY;

        public static void InitEditor()
        {
            FormFormat.Load();

            if (MyEditor == null)
            {
                MyEditor = new Editor();
                MyEditor.Create();
                NewEditorLevel();
            }

            Game1.self.IsMouseVisible = true;
            Game1.self.Window.AllowUserResizing = true;
            EditorMode = true;
        }

        public static void Clear()
        {
            GameManager.ClearLevel();
        }

        public static void SwitchToPlay()
        {
            LevelFromEditor = true;
            EditorMode = false;

            EditorSizeX = Game1.self.Window.ClientBounds.Width;
            EditorSizeY = Game1.self.Window.ClientBounds.Height;

            GameManager.SetLevel(new Level(false));

            Stream s = new MemoryStream();
            GameManager.GetEditorLevel().Write(new BinaryWriter(s), false);
            Console.Write(s.Length);
            s.Position = 0;

            GameManager.GetLevel().Read(new BinaryReader(s));

            Game1.self.IsMouseVisible = false;

            //PauseEditor();
            MasterManager.ChangeSize(Game1.ResolutionX, Game1.ResolutionY);
            PlayerProfile.Clear();
        }

        public static void SwitchFromPlay()
        {
            EditorMode = true;
            GameManager.SwitchFromPlay();
            Game1.self.IsMouseVisible = true;
            MasterManager.ChangeSize(EditorSizeX, EditorSizeY);

            MyEditor.OldHeight = 100000;
            MyEditor.OldWidth = 100000;
            PlayerProfile.Clear();
        }

        public static void NewEditorLevel()
        {
            Clear();
            GameManager.SetEditorLevel(new Level(true));
            GameManager.SetLevel(GameManager.GetEditorLevel());
            Game1.self.Window.Title = Game1.EngineTitle;
        }

        public static void LoadNewLevel(Level level)
        {
            GameManager.SetLevel(level);
            GameManager.SetEditorLevel(level);
        }

        public static void ExitEditor()
        {
            Game1.self.IsMouseVisible = false;
            Game1.self.Window.AllowUserResizing = false;
            EditorMode = true;
            LevelFromEditor = false;
        }

        public static void Update(GameTime gameTime)
        {   
            if (EditorMode)
            {
                if (Game1.self.IsActive)
                    MyEditor.Update(gameTime);

                SenderManager.Update(gameTime);
            }
            else
            {
                if (KeyboardManager.keyboardState.IsKeyDown(Keys.Escape))
                    SwitchFromPlay();
                else
                {
                    GameManager.Update(gameTime);
                    FPSCounter.Update(gameTime);
                }
            }
        }

        public static void Destroy()
        {
            MyEditor.Destroy();
        }

        public static void Draw()
        {
            if (EditorMode)
                MyEditor.Draw();
            else
            {
                GameManager.Draw();

                MasterManager.SetViewportToFullscreen();
                Game1.graphicsDevice.SetRenderTarget(null);
                FPSCounter.Draw();
            }
        }
    }
}
#endif