using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class DevourerCard : UnitCard
    {
        public DevourerCard()
        {
            Name = "Devourer";
            GhostCount = 20;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Devourer(FactionNumber);
        }
    }
}
