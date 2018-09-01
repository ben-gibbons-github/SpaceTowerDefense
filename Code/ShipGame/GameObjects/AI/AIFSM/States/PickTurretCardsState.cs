using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class PickTurretCardsState : AiState
    {
        int PauseTime = 0;
        int MaxPauseTime = 3000;

        public override void Enter(AiStateManager Parent)
        {
            PauseTime = 0;

            base.Enter(Parent);
        }

        public override void Update(GameTime gameTime)
        {
            PauseTime += gameTime.ElapsedGameTime.Milliseconds;
            if (PauseTime > MaxPauseTime)
            {
                Parent.ParentController.ParentShip.GetTurretSelection();

                AiState s = Parent.GetExistingState(typeof(PlaceTurretState));
                Parent.SetState(s == null ? new PlaceTurretState() : s);
            }

            base.Update(gameTime);
        }
    }
}
