using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ScorpionSpawnerCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 96;

        public ScorpionSpawnerCard()
        {
            cardType = CardType.UnitSpawners;

            Name = "Scorpion Spawner";
            Caption = "Spawns heavy hitting Scorpions.\nSTRONG VS LARGE";

            CardCellsCost = 600;
            CardCellsCostIncrease = 60;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override string GetUnitImagePath()
        {
            return "Scorpion";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new ScorpionSpawner(FactionNumber);
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.RedColor;
        }
    }
}
