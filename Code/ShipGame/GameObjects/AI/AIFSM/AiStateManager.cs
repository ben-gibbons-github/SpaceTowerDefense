using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class AiStateManager
    {
        public LinkedList<AiState> AllStates;
        public AiState CurrentState;
        public StarShipAiController ParentController;

        public AiStateManager(StarShipAiController ParentController)
        {
            this.ParentController = ParentController;
            AllStates = new LinkedList<AiState>();
            SetState(new PickTurretStartState());
        }

        public AiState GetExistingState(Type T)
        {
            foreach (AiState s in AllStates)
                if (s.GetType().Equals(T))
                    return s;

            return null;   
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentState != null)
                CurrentState.Update(gameTime);
        }

        public void SetState(AiState state)
        {
            if (!AllStates.Contains(state))
                AllStates.AddLast(state);
            
            if (this.CurrentState != null)
                this.CurrentState.Exit();
            this.CurrentState = state;
            this.CurrentState.Enter(this);
        }
    }
}
