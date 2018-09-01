using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BankCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 96;

        public BankCard()
        {
            cardType = CardType.ExtraTurrets;

            Name = "Bank";
            Caption = "Produces 50 funds and energy\n100 when upgraded";

            CardCellsCost = 300;
            CardCellsCostIncrease = 50;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        protected override float GetTurretWeight()
        {
            return 0.2f;
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

        public override float GetBaseAvoidence()
        {
            return -0.5f;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Bank(FactionNumber);
        }

        public override Color GetColor()
        {
            return new Color(0.75f, 0.5f, 1);
        }

        public override string GetImagePath()
        {
            return "ExtraTurrets/Turret5";
        }
    }
}
