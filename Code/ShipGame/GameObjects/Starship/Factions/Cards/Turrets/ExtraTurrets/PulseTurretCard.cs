using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PulseTurretCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 350;

        public PulseTurretCard()
        {
            cardType = CardType.ExtraTurrets;

            Name = "Pulse Turret";
            Caption = "Throws enemy units away.\nSTRONG VS. MELEE";

            CardCellsCost = 100;
            CardCellsCostIncrease = 500;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return -1f;
        }

        public override float GetBuildingAvoidence()
        {
            return 2;
        }

        public override float GetTurretAgression()
        {
            return 1.5f;
        }

        protected override float GetTurretWeight()
        {
            return 0.75f;
        }

        public override float GetBaseAvoidence()
        {
            return -0.25f;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new PulseTurret(FactionNumber);
        }

        public override Color GetColor()
        {
            return new Color(1, 0.75f, 0.5f);
        }

        public override string GetImagePath()
        {
            return "ExtraTurrets/Turret1";
        }
    }
}
