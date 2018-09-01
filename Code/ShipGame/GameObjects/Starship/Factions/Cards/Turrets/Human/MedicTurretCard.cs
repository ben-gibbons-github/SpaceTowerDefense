using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class MedicTurretCard : TurretCard
    {
        public static float STurretSize = 50;
        public static float EngagementDistance = 600;

        public MedicTurretCard()
        {
            Name = "Medic Turret";
            Caption = "Sacrifices itself to revive friendly turrets.\nDoes not sacrifice itself when upgraded";

            CardCellsCost = 100;
            CardCellsCostIncrease = 100;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 1.5f;
        }

        public override float GetBuildingAvoidence()
        {
            return -2;
        }

        public override float GetTurretAgression()
        {
            return 1f;
        }

        protected override float GetTurretWeight()
        {
            return 0.35f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.GreenBlueColor;
        }
        
        public override string GetImagePath()
        {
            return "Human/Turret5";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new MedicTurret(FactionNumber);
        }
    }
}
