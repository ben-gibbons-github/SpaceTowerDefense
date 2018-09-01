using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public abstract class AiController : BasicController
    {
        public override bool StartButton()
        {
            return false;
        }

        public override bool StartButtonPrevious()
        {
            return false;
        }

        public override bool IsKeyboardController()
        {
            return false;
        }

        public override bool IsLocal()
        {
            return false;
        }

        public override Vector2 RightStickReset()
        {
            return RightStick();
        }

    }
}
