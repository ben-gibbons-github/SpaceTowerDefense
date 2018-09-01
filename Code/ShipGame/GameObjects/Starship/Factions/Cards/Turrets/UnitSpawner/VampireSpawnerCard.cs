using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class VampireSpawnerCard : TurretCard
    {
        public static float STurretSize = 96;
        public static float EngagementDistance = 96;

        public VampireSpawnerCard()
        {
            cardType = CardType.UnitSpawners;

            Name = "Vampire Spawner";
            Caption = "Spawns long range Vampires.\nSTRONG VS GREEN";

            CardCellsCost = 500;
            CardCellsCostIncrease = 50;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override string GetUnitImagePath()
        {
            return "Vampire";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new VampireSpawner(FactionNumber);
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return TurretCard.BlueColor;
        }
    }
}
