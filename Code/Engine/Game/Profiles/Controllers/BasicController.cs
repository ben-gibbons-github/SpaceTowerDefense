using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public abstract class BasicController
    {
        private static float MenuStickThreshhold = 0.1f;

        public float Sensitivity = 1f;

        public virtual Vector2 MenuStick(bool CountLeftStick, bool CountRightStick, bool CountDPad, bool CountTriggers, bool CountBumpers)
        {
            if (CountLeftStick && LeftStickPrevious().Length() < MenuStickThreshhold && LeftStick().Length() >= MenuStickThreshhold)
                return ConvertMenuStick(LeftStick() * new Vector2(1, -1));

            if (!IsKeyboardController() && CountRightStick && RightStickPrevious().Length() < MenuStickThreshhold && RightStick().Length() >= MenuStickThreshhold)
                return ConvertMenuStick(RightStick());

            if (CountDPad)
            {
                if (DPadLeft() && !DPadLeftPrevious())
                    return new Vector2(-1, 0);

                if (DPadRight() && !DPadRightPrevious())
                    return new Vector2(1, 0);

                if (DPadUp() && !DPadUpPrevious())
                    return new Vector2(0, -1);

                if (DPadDown() && !DPadDownPrevious())
                    return new Vector2(0, 1);
            }

            if (CountTriggers)
            {
                if (LeftTrigger() && !LeftTriggerPrevious())
                    return new Vector2(-1, 0);

                if (RightTrigger() && !RightTriggerPrevious())
                    return new Vector2(1, 0);
            }

            if (CountBumpers)
            {
                if (LeftBumper() && !LeftTriggerPrevious())
                    return new Vector2(-1, 0);

                if (RightBumper() && !RightTriggerPrevious())
                    return new Vector2(1, 0);
            }

            return Vector2.Zero;
        }

        private Vector2 ConvertMenuStick(Vector2 Stick)
        {
            return new Vector2(Math.Abs(Stick.X) > MenuStickThreshhold ? Stick.X > 0 ? 1 : -1 : 0,
              Math.Abs(Stick.Y) > MenuStickThreshhold ? Stick.Y > 0 ? 1 : -1 : 0);
        }

        public abstract bool LeftBumper();
        public abstract bool LeftBumperPrevious();

        public abstract bool RightBumper();
        public abstract bool RightBumperPrevious();

        public abstract bool DPadUp();
        public abstract bool DPadUpPrevious();
        
        public abstract bool DPadDown();
        public abstract bool DPadDownPrevious();
        
        public abstract bool DPadRight();
        public abstract bool DPadRightPrevious();
        
        public abstract bool DPadLeft();
        public abstract bool DPadLeftPrevious();

        public abstract Vector2 LeftStick();
        public abstract Vector2 LeftStickPrevious();

        public abstract Vector2 RightStick();
        public abstract Vector2 RightStickReset();
        public abstract Vector2 RightStickPrevious();

        public abstract bool AButton();
        public abstract bool AButtonPrevious();

        public abstract bool BButton();
        public abstract bool BButtonPrevious();

        public abstract bool XButton();
        public abstract bool XButtonPrevious();

        public abstract bool YButton();
        public abstract bool YButtonPrevious();

        public abstract bool BackButton();
        public abstract bool BackButtonPrevious();

        public abstract bool StartButton();
        public abstract bool StartButtonPrevious();

        public abstract bool RightTrigger();
        public abstract bool RightTriggerPrevious();

        public abstract bool LeftTrigger();
        public abstract bool LeftTriggerPrevious();

        public abstract bool IsKeyboardController();
        public abstract bool IsLocal();

        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
