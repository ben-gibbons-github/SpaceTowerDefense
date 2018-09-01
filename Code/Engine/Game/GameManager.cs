using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace BadRabbit.Carrot
{
    public class GameManager
    {
        private static Level MyLevel;
        private static Level EditorLevel;

        public static void SwitchFromPlay()
        {
            MyLevel.Destroy();
            MyLevel = EditorLevel;
        }

        public static void SetSize(Vector2 Size)
        {
            if (MyLevel != null)
                MyLevel.SetSize(Size);
        }

        public static Level GetLevel()
        {
            return MyLevel;
        }

        public static void ClearLevel()
        {
            if (MyLevel != null)
            {
                MyLevel.Destroy();
                MyLevel = null;
            }
        }

        public static Level SetLevel(Level NewLevel)
        {
            MyLevel = NewLevel;
            return NewLevel;
        }

        public static Level GetEditorLevel()
        {
            return EditorLevel;
        }

        public static Level SetEditorLevel(Level NewLevel)
        {
            EditorLevel = NewLevel;
            return NewLevel;
        }

        public static bool LoadTitleLevel(string Path)
        {
            Stream s = TitleContainer.OpenStream("Content/Levels/" + Path + ".lvl");
            if (s != null)
            {
                GameManager.SetLevel(new Level(false));
                GameManager.GetLevel().Read(new BinaryReader(s));
                return true;
            }
            else
                return false;
        }

        public static void Update(GameTime gameTime)
        {
            MyLevel.Update(gameTime);
        }

        public static void Destroy()
        {
            MyLevel.Destroy();
            EditorLevel.Destroy();
        }

        public static void Draw()
        {
            MyLevel.Draw();
        }
    }
}
