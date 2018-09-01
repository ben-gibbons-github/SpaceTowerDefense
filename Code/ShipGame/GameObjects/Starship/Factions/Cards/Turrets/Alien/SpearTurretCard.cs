using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class SpearTurretCard : TurretCard
    {
        public static float STurretSize = 64;
        public static float EngagementDistance = 800;

        public SpearTurretCard()
        {
            cardType = CardType.Watcher;
            Caption = "Fires a long ranged thin beams.\nSTRONG VS. ALL";

            Name = "Spear Turret";
            CardCellsCost = 200;
            CardCellsCostIncrease = 800;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 2f;
        }

        public override float GetBuildingAvoidence()
        {
            return 0.5f;
        }

        public override float GetTurretAgression()
        {
            return 0.75f;
        }

        protected override float GetTurretWeight()
        {
            return 1.25f;
        }

        public override float GetBaseAvoidence()
        {
            return 0.5f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.WhiteColor;
        }
        
        public override string GetImagePath()
        {
            return "Alien/Turret4";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new SpearTurret(FactionNumber);
        }
    }
}
