using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SlowFieldCard : TurretCard
    {
        public static float STurretSize = 200;
        public static float EngagementDistance = 400;

        public SlowFieldCard()
        {
            cardType = CardType.Empire;

            Name = "Slow Field";
            Caption = "Slows units caught inside it's field.\nSTRONG VS. LIGHT";
            StrongVs = "Light";

            CardCellsCost = 300;
            CardCellsCostIncrease = 400;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return -1f;
        }

        public override float GetTurretAvoidence()
        {
            return 1.5f;
        }

        public override float GetTurretAgression()
        {
            return 1f;
        }

        public override float GetBaseAvoidence()
        {
            return -1f;
        }

        protected override float GetTurretWeight()
        {
            return 0.6f;
        }

        public override Color GetColor()
        {
            return TurretCard.BlueColor;
        }
        
        public override string GetImagePath()
        {
            return "Empire/Turret3";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new SlowField(FactionNumber);
        }
    }
}
