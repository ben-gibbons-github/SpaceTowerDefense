using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class BabyCrusherCard : UnitCard
    {
        public BabyCrusherCard()
        {
            Name = "BabyCrusher";
            GhostCount = 2;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new BabyCrusher(FactionNumber);
        }
    }
}
