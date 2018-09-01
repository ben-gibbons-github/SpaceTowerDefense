using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlasmaTurretCard : TurretCard
    {
        public static float STurretSize = 64;
        public static float EngagementDistance = 400;

        public PlasmaTurretCard()
        {
            Name = "Plasma Turret";
            Caption = "Launches a steady stream of plasma bolts.\nSTRONG VS HEAVY";
            StrongVs = "Heavy";

            CardCellsCost = 125;
            CardCellsCostIncrease = 125;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.RedColor;
        }

        public override string GetImagePath()
        {
            return "Human/Turret1";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new PlasmaTurret(FactionNumber);
        }
    }
}
