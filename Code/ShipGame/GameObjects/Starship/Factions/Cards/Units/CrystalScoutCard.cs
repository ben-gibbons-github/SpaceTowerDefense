﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CrystalScoutCard : UnitCard
    {
        public CrystalScoutCard()
        {
            Name = "CrystalScout";
            GhostCount = 4;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new CrystalScout(FactionNumber);
        }
    }
}
