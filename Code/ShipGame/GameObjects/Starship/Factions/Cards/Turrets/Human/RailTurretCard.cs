using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class RailTurretCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 600;

        public RailTurretCard()
        {
            Name = "Rail Turret";
            Caption = "Launches a steady stream of rail bolts.\nSTRONG VS MEDIUM";
            StrongVs = "Medium";

            CardCellsCost = 600;
            CardCellsCostIncrease = 200;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 0.75f;
        }

        public override float GetBuildingAvoidence()
        {
            return 1.25f;
        }

        public override float GetTurretAgression()
        {
            return 1.25f;
        }

        protected override float GetTurretWeight()
        {
            return 1f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.GreenColor;
        }

        public override string GetImagePath()
        {
            return "Human/Turret3";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new RailTurret(FactionNumber);
        }
    }
}
