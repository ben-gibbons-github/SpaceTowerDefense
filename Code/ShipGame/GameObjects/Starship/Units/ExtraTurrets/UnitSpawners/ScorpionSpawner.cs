using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ScorpionSpawner : UnitSpawner
    {
        public ScorpionSpawner(int FactionNumber)
            : base(FactionNumber)
        {

        }

        public override void Create()
        {
            SpawnCard = (UnitCard)FactionCard.FactionUnitDeck[3];
            base.Create();
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 3;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship4");
        }
    }
}
