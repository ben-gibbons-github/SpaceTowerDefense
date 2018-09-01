using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class MineLayerCard : TurretCard
    {
        public static float STurretSize = 100;
        public static float EngagementDistance = 200;

        public MineLayerCard()
        {
            cardType = CardType.Empire;

            Name = "Mine Field";
            Caption = "Creates a field of mines which launch\n themselves at nearby enemies.\nSTRONG VS. MEDIUM";
            StrongVs = "Medium";

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

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.GreenColor;
        }
        
        public override string GetImagePath()
        {
            return "Empire/Turret2";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new MineLayer(FactionNumber);
        }
    }
}
