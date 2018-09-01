using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ParasiteCard : UnitCard
    {
        public ParasiteCard()
        {
            Name = "Parasite";
            GhostCount = 15;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Parasite(FactionNumber);
        }
    }
}
