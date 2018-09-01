using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PitbullCard : UnitCard
    {
        public PitbullCard()
        {
            Name = "Pitbull";
            GhostCount = 8;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Pitbull(FactionNumber);
        }
    }
}
