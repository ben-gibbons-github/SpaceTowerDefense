using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class HornetSpawnerCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 96;

        public HornetSpawnerCard()
        {
            cardType = CardType.UnitSpawners;

            Name = "Hornet Spawner";
            Caption = "Spawns mid range Hornets.\nSTRONG VS BLUE";

            CardCellsCost = 450;
            CardCellsCostIncrease = 40;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override string GetUnitImagePath()
        {
            return "Hornet";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new HornetSpawner(FactionNumber);
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.RedColor;
        }
    }
}
