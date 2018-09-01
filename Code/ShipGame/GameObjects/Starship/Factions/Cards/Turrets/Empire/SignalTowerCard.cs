using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class SignalTowerCard : TurretCard
    {
        public static float STurretSize = 80;

        public SignalTowerCard()
        {
            cardType = CardType.Empire;

            Name = "Signal Tower";
            Caption = "A resilient tower which atracts enemy attention";

            CardCellsCost = 200;
            CardCellsCostIncrease = 200;
            TurretSize = STurretSize;
            Radius = STurretSize * 2;
        }

        public override float GetTurretFragility()
        {
            return 1;
        }

        public override float GetBuildingAvoidence()
        {
            return 1f;
        }

        public override float GetBaseAvoidence()
        {
            return 1f;
        }

        public override float GetTurretAgression()
        {
            return 2f;
        }

        protected override float GetTurretWeight()
        {
            return 1f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.PurpleColor;
        }
        
        public override string GetImagePath()
        {
            return "Empire/Turret4";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new SignalTower(FactionNumber);
        }
    }
}
