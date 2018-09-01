using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class FlameTurretCard : TurretCard
    {
        public static float STurretSize = 64;
        public static float EngagementDistance = 250;

        public FlameTurretCard()
        {
            cardType = CardType.Empire;
            Caption = "Fires a steady stream of flame and does\nextreme damage when it explodes.\nSTRONG VS. HEAVY";
            StrongVs = "Heavy";

            Name = "Flame Turret";
            CardCellsCost = 75;
            CardCellsCostIncrease = 75;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 1.25f;
        }

        public override float GetBuildingAvoidence()
        {
            return 0.75f;
        }

        public override float GetTurretAgression()
        {
            return 1.25f;
        }

        protected override float GetTurretWeight()
        {
            return 0.3f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.RedColor;
        }
        
        public override string GetImagePath()
        {
            return "Empire/Turret1";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new FlameTurret(FactionNumber);
        }
    }
}
