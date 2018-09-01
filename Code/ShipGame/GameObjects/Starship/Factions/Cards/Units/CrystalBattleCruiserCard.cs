using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CrystalBattleCruiserCard : UnitCard
    {
        public CrystalBattleCruiserCard()
        {
            Name = "CrystalBattleCruiser";
            GhostCount = 2;
            EnergyCost = 5;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new CrystalBattleCruiser(FactionNumber);
        }
    }
}
