using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class StasisBombCard : TurretCard
    {
        public static float STurretSize = 64;
        public static float EngagementDistance = 400;

        public StasisBombCard()
        {
            cardType = CardType.Rebels;

            Name = "Stasis Bomb";
            Caption = "Destroys all nearby units when triggered by proximity.\nAlso destroys itself.\nSTRONG VS LIGHT";
            StrongVs = "Light";

            CardCellsCost = 100;
            CardCellsCostIncrease = 400;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return -0.5f;
        }

        public override float GetBuildingAvoidence()
        {
            return 0;
        }

        public override float GetTurretAgression()
        {
            return 0.25f;
        }

        protected override float GetTurretWeight()
        {
            return 0.75f;
        }

        public override float GetBaseAvoidence()
        {
            return -0.25f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.GreenBlueColor;
        }
        
        public override string GetImagePath()
        {
            return "Human/Turret6";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new StasisBomb(FactionNumber);
        }
    }
}
