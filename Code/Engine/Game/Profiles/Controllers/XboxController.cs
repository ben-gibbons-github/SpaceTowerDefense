using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class XboxController : BasicController
    {
        public PlayerIndex MyPlayerIndex;
        public GamePadState PadState;
        public GamePadState PreviousPadState;

        public static bool TestAny(GamePadState state)
        {
            return state.Buttons.A == ButtonState.Pressed ||
                state.Buttons.Start == ButtonState.Pressed;
        }

        public override bool IsKeyboardController()
        {
            return false;
        }

        public override bool DPadDown()
        {
            return PadState.DPad.Down == ButtonState.Pressed;
        }

        public override bool DPadDownPrevious()
        {
            return PreviousPadState.DPad.Down == ButtonState.Pressed;
        }

        public override bool DPadRight()
        {
            return PadState.DPad.Right == ButtonState.Pressed;
        }

        public override bool DPadRightPrevious()
        {
            return PreviousPadState.DPad.Right == ButtonState.Pressed;
        }

        public override bool DPadLeft()
        {
            return PadState.DPad.Left == ButtonState.Pressed;
        }

        public override bool DPadLeftPrevious()
        {
            return PreviousPadState.DPad.Left == ButtonState.Pressed;
        }

        public override bool DPadUp()
        {
            return PadState.DPad.Up == ButtonState.Pressed;
        }

        public override bool DPadUpPrevious()
        {
            return PreviousPadState.DPad.Up == ButtonState.Pressed;
        }

        public override bool RightBumper()
        {
            return PadState.Buttons.RightShoulder == ButtonState.Pressed;
        }

        public override bool RightBumperPrevious()
        {
            return PreviousPadState.Buttons.RightShoulder == ButtonState.Pressed;
        }

        public override bool LeftBumper()
        {
            return PadState.Buttons.LeftShoulder == ButtonState.Pressed;
        }

        public override bool LeftBumperPrevious()
        {
            return PreviousPadState.Buttons.LeftShoulder == ButtonState.Pressed;
        }

        public override Vector2 LeftStick()
        {
            return PadState.ThumbSticks.Left;
        }

        public override Vector2 LeftStickPrevious()
        {
            return PreviousPadState.ThumbSticks.Left;
        }

        public override Vector2 RightStick()
        {
            return PadState.ThumbSticks.Right;
        }

        public override Vector2 RightStickReset()
        {
            return PadState.ThumbSticks.Right;
        }

        public override Vector2 RightStickPrevious()
        {
            return PreviousPadState.ThumbSticks.Right;
        }

        public override bool LeftTrigger()
        {
            return PadState.Triggers.Left > 0.5f;
        }

        public override bool LeftTriggerPrevious()
        {
            return PreviousPadState.Triggers.Left > 0.5f;
        }

        public override bool RightTrigger()
        {
            return PadState.Triggers.Right > 0.5f;
        }

        public override bool RightTriggerPrevious()
        {
            return PreviousPadState.Triggers.Right > 0.5f;
        }

        public override bool AButton()
        {
            return PadState.Buttons.A == ButtonState.Pressed;
        }

        public override bool BButton()
        {
            return PadState.Buttons.B == ButtonState.Pressed;
        }

        public override bool XButton()
        {
            return PadState.Buttons.X == ButtonState.Pressed;
        }

        public override bool YButton()
        {
            return PadState.Buttons.Y == ButtonState.Pressed;
        }

        public override bool  AButtonPrevious()
        {
            return PreviousPadState.Buttons.A == ButtonState.Pressed;
        }

        public override bool BButtonPrevious()
        {
            return PreviousPadState.Buttons.B == ButtonState.Pressed;
        }

        public override bool XButtonPrevious()
        {
            return PreviousPadState.Buttons.X == ButtonState.Pressed;
        }

        public override bool YButtonPrevious()
        {
            return PreviousPadState.Buttons.Y == ButtonState.Pressed;
        }

        public override bool BackButton()
        {
            return PadState.Buttons.Back == ButtonState.Pressed;
        }

        public override bool BackButtonPrevious()
        {
            return PreviousPadState.Buttons.Back == ButtonState.Pressed;
        }

        public override bool StartButton()
        {
            return PadState.Buttons.Start == ButtonState.Pressed;
        }

        public override bool StartButtonPrevious()
        {
            return PreviousPadState.Buttons.Start == ButtonState.Pressed;
        }

        public XboxController(PlayerIndex MyPlayerIndex)
        {
            this.MyPlayerIndex = MyPlayerIndex;
        }

        public override bool IsLocal()
        {
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            PreviousPadState = PadState;
            PadState = GamePad.GetState(MyPlayerIndex);
            base.Update(gameTime);
        }
    }
}
