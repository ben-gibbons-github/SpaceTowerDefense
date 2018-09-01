using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class StingraySpawnerCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 96;

        public StingraySpawnerCard()
        {
            cardType = CardType.UnitSpawners;

            Name = "Stingray Spawner";
            Caption = "Spawns heavy Stingrays.\nSTRONG VS TURRETS";

            CardCellsCost = 600;
            CardCellsCostIncrease = 60;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override string GetUnitImagePath()
        {
            return "Stingray";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new StingraySpawner(FactionNumber);
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.BlueColor;
        }
    }
}
