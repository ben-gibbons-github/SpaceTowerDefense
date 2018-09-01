using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CobraSpawnerCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 96;

        public CobraSpawnerCard()
        {
            cardType = CardType.UnitSpawners;

            Name = "Cobra Spawner";
            Caption = "Spawns Melee attacking Cobras.\nSTRONG VS RED";

            CardCellsCost = 400;
            CardCellsCostIncrease = 40;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override string GetUnitImagePath()
        {
            return "Cobra";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new CobraSpawner(FactionNumber);
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.GreenColor;
        }
    }
}
