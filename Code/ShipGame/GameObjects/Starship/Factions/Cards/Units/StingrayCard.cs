using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class StingrayCard : UnitCard
    {
        public StingrayCard()
        {
            Name = "Stingray";
            GhostCount = 3;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Stingray(FactionNumber);
        }
    }
}
