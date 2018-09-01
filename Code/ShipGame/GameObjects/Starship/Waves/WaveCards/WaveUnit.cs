using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BadRabbit.Carrot;

namespace ShipGame.Wave
{
    public class WaveUnit
    {
        public FactionCard Card;
        public bool Cloaked = false;
        public bool Huge = false;
        public bool Summoner = false;
        public int UnitCount;
        public int StartingUnitCount;
        public float Level;
        public int Team = 0;

        public WaveUnit(FactionCard Card, int UnitCount, float Level)
        {
            this.Card = Card;
            this.StartingUnitCount = UnitCount;
            this.Level = Level;
        }

    }
}
