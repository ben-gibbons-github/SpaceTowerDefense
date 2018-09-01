using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class PickTurretStartState : AiState
    {
        float Theta;
        float TargetTheta;
        float MoveSpeed = 0.01f;

        int PauseTime = 0;
        int MaxPauseTime = 4000;


        public override void Enter(AiStateManager Parent)
        {
            PauseTime = 0;
            TargetTheta = Rand.F() * MathHelper.TwoPi;
            Theta = Rand.F() * MathHelper.TwoPi;
            base.Enter(Parent);
        }

        public override void Update(GameTime gameTime)
        {
            PauseTime += gameTime.ElapsedGameTime.Milliseconds;
            if (PauseTime > MaxPauseTime)
            {
                float MoveAmt = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * MoveSpeed;
                Theta = Logic.Clerp(Theta, TargetTheta, MoveAmt);
                if (MathHelper.WrapAngle(Theta - TargetTheta) < MoveAmt)
                {
                    Theta = TargetTheta;

                    if (Parent.ParentController.ParentShip != null &&
                        Parent.ParentController.ParentShip.PlacedStartingTurrets)
                    {
                        AiState s = Parent.GetExistingState(typeof(PickTurretCardsState));
                        Parent.SetState(s == null ? new PickTurretCardsState() : s);
                    }
                }
            }
            base.Update(gameTime);
        }

        public override Vector2 LeftStick()
        {
            return Logic.ToVector2(Theta);
        }

        public override bool AButton()
        {
            if (PauseTime > MaxPauseTime && Math.Abs(Theta - TargetTheta) < 0.1f)
                return true;
            return base.AButton();
        }
    }
}
