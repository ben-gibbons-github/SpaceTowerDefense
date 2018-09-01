using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CobraSpawner : UnitSpawner
    {
        public CobraSpawner(int FactionNumber)
         : base(FactionNumber)
        {

        }

        public override void Create()
        {
            SpawnCard = (UnitCard)FactionCard.FactionUnitDeck[0];
            base.Create();
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship1");
        }
    }
}
