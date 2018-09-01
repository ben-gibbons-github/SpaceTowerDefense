using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class VampireSpawner : UnitSpawner
    {
        public VampireSpawner(int FactionNumber)
            : base(FactionNumber)
        {

        }

        public override void Create()
        {
            SpawnCard = (UnitCard)FactionCard.FactionUnitDeck[2];
            base.Create();
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship3");
        }
    }
}
