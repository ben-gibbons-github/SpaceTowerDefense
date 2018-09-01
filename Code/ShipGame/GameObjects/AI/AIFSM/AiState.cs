using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class AiState
    {
        public AiStateManager Parent;

        public virtual void Enter(AiStateManager Parent)
        {
            this.Parent = Parent;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Exit()
        {

        }

        public virtual bool AButton()
        {
            return false;
        }

        public virtual bool AButtonPrevious()
        {
            return false;
        }

        public virtual bool BButton()
        {
            return false;
        }

        public virtual bool BButtonPrevious()
        {
            return false;
        }

        public virtual bool XButton()
        {
            return false;
        }

        public virtual bool XButtonPrevious()
        {
            return false;
        }

        public virtual bool YButton()
        {
            return false;
        }

        public virtual bool YButtonPrevious()
        {
            return false;
        }

        public virtual Vector2 LeftStick()
        {
            return Vector2.Zero;
        }

        public virtual Vector2 LeftStickPrevious()
        {
            return Vector2.Zero;
        }

        public virtual Vector2 RightStick()
        {
            return Vector2.Zero;
        }

        public virtual Vector2 RightStickPrevious()
        {
            return Vector2.Zero;
        }

        public virtual bool RightTrigger()
        {
            return false;
        }

        public virtual bool RightTriggerPrevious()
        {
            return false;
        }

        public virtual bool LeftTrigger()
        {
            return false;
        }

        public virtual bool leftTriggerPrevious()
        {
            return false;
        }
    }
}
