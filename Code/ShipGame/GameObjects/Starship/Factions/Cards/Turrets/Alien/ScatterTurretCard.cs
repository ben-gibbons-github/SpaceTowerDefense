using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class BeamTurretCard : TurretCard
    {
        public static float STurretSize = 120;
        public static float EngagementDistance = 600;

        public BeamTurretCard()
        {
            Name = "Beam Turret";
            Caption = "Fires a rapid barage of beams.\nSTRONG VS. MEDIUM";
            StrongVs = "Medium";

            CardCellsCost = 600;
            CardCellsCostIncrease = 400;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
            cardType = CardType.Watcher;
        }

        public override float GetTurretFragility()
        {
            return 1.25f;
        }

        public override float GetBuildingAvoidence()
        {
            return 1.25f;
        }

        public override float GetTurretAgression()
        {
            return 1;
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
            return "Alien/Turret2";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new BeamTurret(FactionNumber);
        }
    }
}
