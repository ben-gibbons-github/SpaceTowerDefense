using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ProtectionCasterCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 400;

        public ProtectionCasterCard()
        {
            cardType = CardType.ExtraTurrets;

            Name = "Protection Caster";
            Caption = "Heals damage done to friendly buildings.\nAffects friendly mining rings.";

            CardCellsCost = 100;
            CardCellsCostIncrease = 500;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 1f;
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

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new ProtectionCaster(FactionNumber);
        }

        public override Color GetColor()
        {
            return new Color(0.5f, 0.75f, 1);
        }

        public override string GetImagePath()
        {
            return "ExtraTurrets/Turret2";
        }
    }
}
