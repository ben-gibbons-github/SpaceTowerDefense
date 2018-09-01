#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class KeyboardController : BasicController
    {
        KeyboardState keyboardState;
        KeyboardState PreviousKeyboardState;
        public static float MouseDivide = 50;

        public static Keys AKey = Keys.Space;
        public static Keys BKey = Keys.LeftControl;
        public static Keys XKey = Keys.R;
        public static Keys YKey = Keys.E;

        public static Keys BackKey = Keys.Tab;
        public static Keys StartKey = Keys.Escape;

#if EDITOR && WINDOWS
        public static Keys AltStartKey = Keys.Back;
#endif

        public static Keys LUpKey = Keys.W;
        public static Keys LDownKey = Keys.S;
        public static Keys LLeftKey = Keys.A;
        public static Keys LRightKey = Keys.D;

        public static Keys DUpKey = Keys.Up;
        public static Keys DDownKey = Keys.Down;
        public static Keys DLeftKey = Keys.Left;
        public static Keys DRightKey = Keys.Right;

        public static Keys RightBumperKey = Keys.X;
        public static Keys LeftBumperKey = Keys.Z;

        public override bool LeftBumper()
        {
            return keyboardState.IsKeyDown(LeftBumperKey);
        }

        public override bool LeftBumperPrevious()
        {
            return keyboardState.IsKeyUp(LeftBumperKey);
        }

        public override bool RightBumper()
        {
            return keyboardState.IsKeyDown(RightBumperKey);
        }

        public override bool RightBumperPrevious()
        {
            return keyboardState.IsKeyUp(RightBumperKey);
        }

        public override bool DPadUp()
        {
            return keyboardState.IsKeyDown(DUpKey);
        }

        public override bool DPadUpPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(DUpKey);
        }

        public override bool DPadDown()
        {
            return keyboardState.IsKeyDown(DDownKey);
        }

        public override bool DPadDownPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(DDownKey);
        }

        public override bool DPadRight()
        {
            return keyboardState.IsKeyDown(DRightKey);
        }

        public override bool DPadRightPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(DRightKey);
        }

        public override bool DPadLeft()
        {
            return keyboardState.IsKeyDown(DLeftKey);
        }

        public override bool DPadLeftPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(DLeftKey);
        }

        public override bool IsKeyboardController()
        {
            return true;
        }

        public override bool IsLocal()
        {
            return true;
        }

        public override bool BackButton()
        {
            return keyboardState.IsKeyDown(BackKey);
        }

        public override bool BackButtonPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(BackKey);
        }

        public override bool StartButton()
        {
#if EDITOR && WINDOWS
            return keyboardState.IsKeyDown(AltStartKey);
#endif
#if !EDITOR || !WINDOWS
            return keyboardState.IsKeyDown(StartKey);
#endif
        }

        public override bool StartButtonPrevious()
        {
#if EDITOR && WINDOWS
            return PreviousKeyboardState.IsKeyDown(AltStartKey);
#endif
#if !EDITOR || !WINDOWS
            return PreviousKeyboardState.IsKeyDown(StartKey);
#endif
        }

        private Vector2 CalculateStick(KeyboardState state)
        {
            return new Vector2(
                state.IsKeyDown(LRightKey) ? 1 : state.IsKeyDown(LLeftKey) ? -1 : 0,
                state.IsKeyDown(LUpKey) ? 1 : state.IsKeyDown(LDownKey) ? -1 : 0);
        }

        public override Vector2 LeftStick()
        {
            return CalculateStick(keyboardState);
        }

        public override Vector2 LeftStickPrevious()
        {
            return CalculateStick(PreviousKeyboardState);
        }

        public override Vector2 RightStick()
        {
            return MouseManager.MouseMovement / MouseDivide;
        }

        public override Vector2 RightStickReset()
        {
            MouseManager.ResetMouse();
            return MouseManager.MouseMovement / MouseDivide;
        }

        public override Vector2 RightStickPrevious()
        {
            return MouseManager.PreviousMouseMovement / MouseDivide;
        }

        public override bool LeftTrigger()
        {
            return MouseManager.mouseState.RightButton == ButtonState.Pressed;
        }

        public override bool LeftTriggerPrevious()
        {
            return MouseManager.mouseStatePrevious.RightButton == ButtonState.Pressed;
        }

        public override bool RightTrigger()
        {
            return MouseManager.mouseState.LeftButton == ButtonState.Pressed;
        }

        public override bool RightTriggerPrevious()
        {
            return MouseManager.mouseStatePrevious.LeftButton == ButtonState.Pressed;
        }

        public override bool AButton()
        {
            return keyboardState.IsKeyDown(AKey);
        }

        public override bool BButton()
        {
            return keyboardState.IsKeyDown(BKey);
        }

        public override bool XButton()
        {
            return keyboardState.IsKeyDown(XKey);
        }

        public override bool YButton()
        {
            return keyboardState.IsKeyDown(YKey);
        }

        public override bool AButtonPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(AKey);
        }

        public override bool BButtonPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(BKey);
        }

        public override bool XButtonPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(XKey);
        }

        public override bool YButtonPrevious()
        {
            return PreviousKeyboardState.IsKeyDown(YKey);
        }

        public override void Update(GameTime gameTime)
        {
            PreviousKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            base.Update(gameTime);
        }
    }
}
#endif