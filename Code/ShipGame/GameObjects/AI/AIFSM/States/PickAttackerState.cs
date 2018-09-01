using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShipGame.Wave;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class PickAttackerState : AiState
    {
        WaveCard SelectedCard;
        bool Moving = false;
        int PauseTime = 0;
        int MaxPauseTime = 5000;


        public override void Enter(AiStateManager Parent)
        {
            float BestScore = 1000;
            Moving = false;
            PauseTime = 0;

            foreach (WaveCard card in OverCardPicker.CurrentCards)
            {
                float Score = 0;
                foreach (UnitBasic u in FactionManager.SortedUnits[WaveManager.ActiveTeam])
                    if (u.GetType().IsSubclassOf(typeof(UnitTurret)))
                    {
                        UnitTurret t = (UnitTurret)u;
                        if (t.MyCard != null)
                        {
                            if (t.MyCard.StrongVs.Equals(card.Type))
                                Score += t.GetWeight();
                        }
                        else if (card.Type.Equals("Heavy"))
                            Score += t.GetWeight();
                    }

                if (Score < BestScore)
                {
                    SelectedCard = card;
                    BestScore = Score;
                }
            }

            base.Enter(Parent);
        }

        public override void Update(GameTime gameTime)
        {
            PauseTime += gameTime.ElapsedGameTime.Milliseconds;
            if (PauseTime > MaxPauseTime)
            {
                Moving = true;
                if (OverCardPicker.CurrentCards[OverCardPicker.TeamSelectedNodes[Parent.ParentController.ParentShip.FactionNumber]] != SelectedCard)
                {
                    OverCardPicker.TeamMoveLeft(Parent.ParentController.ParentShip.FactionNumber);
                    PauseTime -= 1000;
                }
                else
                {
                    OverCardPicker.ReadyTeam(Parent.ParentController.ParentShip.FactionNumber);
                    AiState s = Parent.GetExistingState(typeof(PlaceTurretState));
                    Parent.SetState(s == null ? new PlaceTurretState() : s);
                }
            }

            base.Update(gameTime);
        }
    }
}
