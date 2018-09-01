using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class WaveStateManager
    {
        public WaveState CurrentState;

        public void SetState(WaveState NewState)
        {
            if (CurrentState != null)
                CurrentState.Exit();
            CurrentState = NewState;
            CurrentState.Enter();
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentState != null)
                CurrentState.Update(gameTime);
        }
    }
}
