using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ControllerLoader
    {
        private static bool Loaded = false;
        public static Texture2D AButton;
        public static Texture2D AButtonGlow;
        public static Texture2D XButton;
        public static Texture2D XButtonGlow;

        public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                AButton = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/Keys/AButton");
                AButtonGlow = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/Keys/AButtonGlow");
                XButton = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/Keys/XButton");
                XButtonGlow = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/Keys/XButtonGlow");
            }
        }
    }
}
