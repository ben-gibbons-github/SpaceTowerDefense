using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class EmpLauncherCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 96;

        public EmpLauncherCard()
        {
            cardType = CardType.ExtraTurrets;

            Name = "Emp Launcher";
            Caption = "Launches emp missiles at enemy buildings.\nOnly used in multiplayer.";

            CardCellsCost = 250;
            CardCellsCostIncrease = 250;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 3f;
        }

        public override float GetBuildingAvoidence()
        {
            return -2;
        }

        public override float GetTurretAgression()
        {
            return 0.25f;
        }

        protected override float GetTurretWeight()
        {
            return 0.35f;
        }

        public override float GetBaseAvoidence()
        {
            return -0.5f;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new EmpLauncher(FactionNumber);
        }

        public override Color GetColor()
        {
            return new Color(0.5f, 0.75f, 1);
        }

        public override string GetImagePath()
        {
            return "ExtraTurrets/Turret4";
        }
    }
}
