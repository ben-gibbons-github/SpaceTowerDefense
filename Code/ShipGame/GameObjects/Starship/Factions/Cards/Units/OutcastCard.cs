using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class OutcastCard : UnitCard
    {
        public OutcastCard()
        {
            Name = "Outcast";
            GhostCount = 3;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Outcast(FactionNumber);
        }
    }
}
