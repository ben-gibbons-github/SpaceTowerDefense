using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CrusherCard : UnitCard
    {
        public CrusherCard()
        {
            Name = "Crusher";
            GhostCount = 0;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Crusher(FactionNumber);
        }
    }
}
