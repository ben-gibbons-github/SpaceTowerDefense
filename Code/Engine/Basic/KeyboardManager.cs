#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class KeyboardManager
    {
        public static KeyboardState keyboardState;
        public static KeyboardState keyboardStatePrevious;

        public static void Update(GameTime gameTime)
        {
            keyboardStatePrevious = keyboardState;
            keyboardState = Keyboard.GetState();
        }

        public static bool KeyJustPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && keyboardStatePrevious.IsKeyUp(key);
        }

        public static bool ShiftPressed()
        {
            return keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
        }

        public static bool ControlPressed()
        {
            return keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
        }

        public static bool AltPressed()
        {
            return keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
        }
    }
}
#endif