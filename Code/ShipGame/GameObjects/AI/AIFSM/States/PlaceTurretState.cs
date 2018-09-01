using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class PlaceTurretState : AiState
    {
        BasicShipGameObject TargetObject;
        TurretCard TargetCard;
        Vector2 TargetCardPosition;

        bool XButtonPreviousVar;


        public override void Enter(AiStateManager Parent)
        {
            TargetObject = null;
            base.Enter(Parent);
        }

        public override void Update(GameTime gameTime)
        {
            if (WaveManager.GetState() == WaveFSM.PickEnemyState.self && WaveManager.CurrentWave > 4 && 
                WaveManager.ActiveTeam != Parent.ParentController.ParentShip.GetTeam())
            {
                AiState s = Parent.GetExistingState(typeof(PickAttackerState));
                Parent.SetState(s == null ? new PickAttackerState() : s);
            }
            else
            {
                if (TargetObject == null && TargetCard == null)
                {
                    bool Damaged = false;
                    foreach (UnitBasic u in FactionManager.SortedUnits[Parent.ParentController.ParentShip.GetTeam()])
                        if (u.GetType().IsSubclassOf(typeof(MiningPlatform)))
                            if (u.HullDamage > 0)
                            {
                                Damaged = true;
                                break;
                            }

                    if (FactionManager.CanBuildMiningPlatform(Parent.ParentController.ParentShip.FactionNumber) &&
                        (WaveManager.ActiveTeam != Parent.ParentController.ParentShip.GetTeam() || FactionManager.NeutralUnitCount < 3 ||
                        (FactionManager.Factions[Parent.ParentController.ParentShip.FactionNumber].MiningPlatformCount < 2
                        && Damaged)))
                    {
                        float BestStrength = 100000;

                        foreach (MineralRock m in Parent.ParentController.ParentShip.ParentScene.Enumerate(typeof(MineralRock)))
                            if (m.miningPlatform == null)
                            {
                                float MineralRockStrength = 0;

                                foreach (MiningPlatform f in
                                    Parent.ParentController.ParentShip.ParentScene.Enumerate(typeof(MiningPlatform)))
                                    if (f.IsAlly(Parent.ParentController.ParentShip))
                                        MineralRockStrength += Vector2.Distance(m.Position.get(), f.Position.get()) / 1000;

                                foreach (UnitTurret f in
                                    Parent.ParentController.ParentShip.ParentScene.Enumerate(typeof(UnitTurret)))
                                    if (f.IsAlly(Parent.ParentController.ParentShip))
                                        MineralRockStrength += Vector2.Distance(m.Position.get(), f.Position.get()) / 2000;

                                if (MineralRockStrength < BestStrength)
                                {
                                    BestStrength = MineralRockStrength;
                                    TargetObject = m;
                                }
                            }
                    }
                    else if (WaveManager.ActiveTeam == Parent.ParentController.ParentShip.GetTeam() &&
                        PathFindingManager.CellJobQue.Count == 0 && WaveManager.CurrentWave > 1)
                    {
                        Faction f = FactionManager.Factions[Parent.ParentController.ParentShip.FactionNumber];

                        TurretCard BestCard = null;
                        float BestStrength = 0;

                        foreach (TurretCard c in f.Cards)
                        {
                            float s = c.GetPlaceStrength(f.FactionNumber);
                            if (s > BestStrength)
                            {
                                BestStrength = s;
                                BestCard = c;
                            }
                        }

                        if (BestCard != null && (!BestCard.FactionCostIncreases.ContainsKey(f.FactionNumber) ? f.Cells >= BestCard.CardCellsCost :
                                f.Cells >= BestCard.CardCellsCost + BestCard.CardCellsCostIncrease * BestCard.FactionCostIncreases[f.FactionNumber]))
                        {
                            TargetCard = BestCard;
                            TargetCardPosition = BestCard.GetPlacePosition(f.FactionNumber);
                        }
                    }
                }
                else
                {
                    if (TargetObject != null)
                    {
                        if (TargetObject.GetType().Equals(typeof(MineralRock)))
                        {
                            MineralRock m = (MineralRock)TargetObject;
                            if (m.miningPlatform != null)
                                TargetObject = null;
                        }
                        else if (TargetObject.GetType().IsSubclassOf(typeof(UnitBuilding)))
                        {
                            UnitBuilding b = (UnitBuilding)TargetObject;
                            if (b.IsUpdgraded)
                                TargetObject = null;
                        }
                    }
                    else if (Vector2.Distance(TargetCardPosition, Parent.ParentController.ParentShip.FloatingViewPosition) < 4)
                    {
                        PlayerShip ParentShip = Parent.ParentController.ParentShip;
                        int CardCost = TargetCard.CardCellsCost;

                        if (TargetCard.FactionCostIncreases.ContainsKey(ParentShip.FactionNumber))
                            CardCost += TargetCard.CardCellsCostIncrease *
                                TargetCard.FactionCostIncreases[ParentShip.FactionNumber];

                        if (TargetCard.FactionCostIncreases.ContainsKey(ParentShip.FactionNumber))
                            TargetCard.FactionCostIncreases[ParentShip.FactionNumber]++;
                        else
                            TargetCard.FactionCostIncreases.Add(ParentShip.FactionNumber, 1);

                        FactionManager.AddCells(ParentShip.FactionNumber, -CardCost);
                        UnitTurret u = (UnitTurret)TargetCard.GetUnit(ParentShip.FactionNumber);
                        u.MyCard = TargetCard;
                        ParentShip.PlaceTurret(u);

                        TargetCard = null;
                    }
                }
            }
            if (WaveFSM.WaveStepState.WeaponsFree && !Parent.ParentController.ParentShip.Dead &&
                TargetObject == null && TargetCard == null)
            {
                if (WaveManager.ActiveTeam != Parent.ParentController.ParentShip.GetTeam())
                {
                    AiState s = Parent.GetExistingState(typeof(AttackState));
                    Parent.SetState(s == null ? new AttackState() : s);
                }
                else
                {
                    AiState s = Parent.GetExistingState(typeof(DefendState));
                    Parent.SetState(s == null ? new DefendState() : s);
                }
            }

            base.Update(gameTime);
        }

        public override bool YButton()
        {
            if (Parent.ParentController.ParentShip.viewMode == ViewMode.Ship)
                return true;
            return base.YButton();
        }

        public override bool XButton()
        {
            bool X = false;
            if (Parent.ParentController.ParentShip.InteractionObject == TargetObject)
                X = true;

            XButtonPreviousVar = X;
            return X;
        }

        public override bool XButtonPrevious()
        {
            return XButtonPreviousVar;
        }

        public override Vector2 LeftStick()
        {
            if (TargetObject != null && Vector2.Distance(TargetObject.Position.get(), 
                Parent.ParentController.ParentShip.FloatingViewPosition) > TargetObject.Size.X() / 4)
                return Vector2.Normalize(TargetObject.Position.get() - 
                    Parent.ParentController.ParentShip.FloatingViewPosition) * new Vector2(1 , -1);

            else if (TargetCard != null && Vector2.Distance(TargetCardPosition, 
                Parent.ParentController.ParentShip.FloatingViewPosition) > 4)
                return Vector2.Normalize(TargetCardPosition - 
                    Parent.ParentController.ParentShip.FloatingViewPosition) * new Vector2(1, -1);

            return base.LeftStick();
        }

        public override bool LeftTrigger()
        {
            if (WaveManager.CurrentWave > 7 && Parent.ParentController.ParentShip.Dead && 
                Parent.ParentController.ParentShip.Attacking)
            {
                TargetCard = null;
                TargetObject = null;
                return true;
            }

            return base.LeftTrigger();
        }
    }
}
