using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class HornetSpawner : UnitSpawner
    {
        public HornetSpawner(int FactionNumber)
            : base(FactionNumber)
        {

        }

        public override void Create()
        {
            SpawnCard = (UnitCard)FactionCard.FactionUnitDeck[1];
            base.Create();
            Size.set(new Vector2(80));
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship2");
        }
    }
}
