using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class SnapTurretCard : TurretCard
    {
        public static float EngagementDistance = 300;
        public static float STurretSize = 80;

        public SnapTurretCard()
        {
            Name = "Snap Turret";
            Caption = "Freezes enemy turrets.\nSTRONG VS SMALL";
            StrongVs = "Light";

            CardCellsCost = 250;
            CardCellsCostIncrease = 150;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 0.75f;
        }

        public override float GetBuildingAvoidence()
        {
            return 1.5f;
        }

        public override float GetTurretAgression()
        {
            return 1.5f;
        }

        protected override float GetTurretWeight()
        {
            return 0.75f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.BlueColor;
        }

        public override string GetImagePath()
        {
            return "Human/Turret2";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new SnapTurret(FactionNumber);
        }
    }
}
