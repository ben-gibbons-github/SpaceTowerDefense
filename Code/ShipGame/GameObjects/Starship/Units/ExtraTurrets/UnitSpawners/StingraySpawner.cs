using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class StingraySpawner : UnitSpawner
    {
        public StingraySpawner(int FactionNumber)
            : base(FactionNumber)
        {

        }

        public override void Create()
        {
            SpawnCard = (UnitCard)FactionCard.FactionUnitDeck[4];
            base.Create();
            Size.set(new Vector2(100));
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 4;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship5");
        }
    }
}
