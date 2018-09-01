using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class RecluseCard : UnitCard
    {
        public RecluseCard()
        {
            Name = "Recluse";
            GhostCount = 20;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Recluse(FactionNumber);
        }
    }
}
