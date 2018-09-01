using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CrystalWallCard : TurretCard
    {
        public static float STurretSize = 20;
        public static float EngagementDistance = 300;

        public CrystalWallCard()
        {
            cardType = CardType.Watcher;
            Caption = "String together multiple wall nodes to\n create walls of force which\nhinder unit movement";

            Name = "Crystal Wall";
            CardCellsCost = 25;
            CardCellsCostIncrease = 25;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 0;
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
            return 0.1f;
        }

        public override float GetBaseAvoidence()
        {
            return -0.5f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.WhiteColor;
        }
        
        public override string GetImagePath()
        {
            return "Alien/Turret5";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new CrystalWall(FactionNumber);
        }
    }
}
