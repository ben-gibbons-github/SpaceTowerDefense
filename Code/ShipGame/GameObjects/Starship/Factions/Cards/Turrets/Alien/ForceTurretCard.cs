using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ForceTurretCard : TurretCard
    {
        public static float STurretSize = 70;
        public static float EngagementDistance = 500;

        public ForceTurretCard()
        {
            Name = "Force Turret";
            Caption = "Fires rays of energy in four directions.\nSTRONG VS. LIGHT";
            StrongVs = "Light";

            CardCellsCost = 200;
            CardCellsCostIncrease = 200;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
            cardType = CardType.Watcher;
        }

        public override float GetTurretFragility()
        {
            return 0.25f;
        }

        public override float GetBuildingAvoidence()
        {
            return 1.5f;
        }

        public override float GetTurretAgression()
        {
            return 1.25f;
        }

        protected override float GetTurretWeight()
        {
            return 0.75f;
        }

        public override float GetBaseAvoidence()
        {
            return -0.5f;
        }

        public override Color GetColor()
        {
            return TurretCard.BlueColor;
        }
        
        public override string GetImagePath()
        {
            return "Alien/Turret3";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new ForceTurret(FactionNumber);
        }
    }
}
