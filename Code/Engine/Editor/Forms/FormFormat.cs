
#if EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FormFormat
    {
        public static Color TextColor = Color.Black;
        public static Color SelectedTextColor = Color.White;
        public static Color BorderColor = Color.DarkGray;
        public static Color BackgroundColor = Color.DarkGray;
        public static Color BackgroundHighlightColor = Color.LightGray;
        public static Color HighlightedTextColor = new Color(0.1f,0.1f,0.1f);

        public static Color WindowOuterColor = Color.DarkGray;
        public static Color WindowInnerColor = Color.Gray;
        public static Color WindowBorderColor = Color.Black;

        public static SpriteFont NormalFont;

        public static void Load()
        {
            NormalFont = Render.BasicFont;
        }
    }
}
#endif