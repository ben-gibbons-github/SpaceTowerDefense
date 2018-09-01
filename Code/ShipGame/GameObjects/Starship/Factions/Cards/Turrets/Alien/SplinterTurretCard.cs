using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class SplinterTurretCard : TurretCard
    {
        public static float STurretSize = 50;
        public static float EngagementDistance = 700;

        public SplinterTurretCard()
        {
            Name = "Splinter Turret";
            Caption = "Fires double splinters at long range and high velocity.\nSTRONG VS. HEAVY";
            StrongVs = "Heavy";

            CardCellsCost = 25;
            CardCellsCostIncrease = 75;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
            cardType = CardType.Watcher;
        }

        public override float GetTurretFragility()
        {
            return 2f;
        }

        public override float GetBuildingAvoidence()
        {
            return 0.25f;
        }

        public override float GetTurretAgression()
        {
            return 1f;
        }

        protected override float GetTurretWeight()
        {
            return 0.25f;
        }

        public override float GetBaseAvoidence()
        {
            return 2f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.RedColor;
        }
        
        public override string GetImagePath()
        {
            return "Alien/Turret1";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new SplinterTurret(FactionNumber);
        }
    }
}
