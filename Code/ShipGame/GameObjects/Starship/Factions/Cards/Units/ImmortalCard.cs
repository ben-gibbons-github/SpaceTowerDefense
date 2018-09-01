using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ImmortalCard : UnitCard
    {
        public ImmortalCard()
        {
            Name = "Immortal";
            GhostCount = 5;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Immortal(FactionNumber);
        }
    }
}
