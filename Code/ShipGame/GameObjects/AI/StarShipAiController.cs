using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class StarShipAiController : AiController
    {
        public PlayerShip ParentShip;
        AiStateManager StateManager;

        public StarShipAiController()
        {
            StateManager = new AiStateManager(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (ParentShip != null)
                StateManager.Update(gameTime);
            base.Update(gameTime);
        }

        public override Vector2 LeftStick()
        {
            return StateManager.CurrentState.LeftStick();
        }

        public override Vector2 LeftStickPrevious()
        {
            return StateManager.CurrentState.LeftStickPrevious();
        }

        public override Vector2 RightStick()
        {
            return StateManager.CurrentState.RightStick();
        }

        public override Vector2 RightStickPrevious()
        {
            return StateManager.CurrentState.RightStickPrevious();
        }

        public override bool AButton()
        {
            return StateManager.CurrentState.AButton();
        }

        public override bool AButtonPrevious()
        {
            return StateManager.CurrentState.AButtonPrevious();
        }

        public override bool BButton()
        {
            return StateManager.CurrentState.BButton();
        }

        public override bool BButtonPrevious()
        {
            return StateManager.CurrentState.BButtonPrevious();
        }

        public override bool XButton()
        {
            return StateManager.CurrentState.XButton();
        }

        public override bool XButtonPrevious()
        {
            return StateManager.CurrentState.XButtonPrevious();
        }

        public override bool YButton()
        {
            return StateManager.CurrentState.YButton();
        }

        public override bool YButtonPrevious()
        {
            return StateManager.CurrentState.YButtonPrevious();
        }

        public override bool LeftBumper()
        {
            return false;
        }

        public override bool LeftBumperPrevious()
        {
            return false;
        }

        public override bool RightBumper()
        {
            return false;
        }

        public override bool RightBumperPrevious()
        {
            return false;
        }

        public override bool DPadUp()
        {
            return false;
        }

        public override bool DPadUpPrevious()
        {
            return false;
        }

        public override bool DPadDown()
        {
            return false;
        }

        public override bool DPadDownPrevious()
        {
            return false;
        }

        public override bool DPadRight()
        {
            return false;
        }

        public override bool DPadRightPrevious()
        {
            return false;
        }

        public override bool DPadLeft()
        {
            return false;
        }

        public override bool DPadLeftPrevious()
        {
            return false;
        }

        public override bool BackButton()
        {
            return false;
        }

        public override bool BackButtonPrevious()
        {
            return false;
        }

        public override bool LeftTrigger()
        {
            return StateManager.CurrentState.LeftTrigger();
        }

        public override bool LeftTriggerPrevious()
        {
            return StateManager.CurrentState.leftTriggerPrevious();
        }

        public override bool RightTrigger()
        {
            return StateManager.CurrentState.RightTrigger();
        }

        public override bool RightTriggerPrevious()
        {
            return StateManager.CurrentState.RightTriggerPrevious();
        }
    }
}
