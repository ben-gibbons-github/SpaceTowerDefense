using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class MiteCard : UnitCard
    {
        public MiteCard()
        {
            Name = "Mite";
            GhostCount = 30;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Mite(FactionNumber);
        }

        public override int GetUnitBuildNumber()
        {
            return 3;
        }
    }
}
