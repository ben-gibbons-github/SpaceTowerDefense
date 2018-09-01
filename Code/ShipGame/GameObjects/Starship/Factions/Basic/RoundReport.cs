using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class RoundReport
    {
        public float DamageDone;
        public float DamageTaken;
        public int EnergyGained;
        public int EnergyUsed;
        public int ScoreGained;
        public int SmallBombsGained;
        public int SmallBombsUsed;
        public int PlayerKills;
        public int PlayerAssists;
        public int UnitKills;
        public int UnitsSpawned;
        public int Deaths;
        public int MiningPlatformsLost;
        public int MiningPlatformsDestroyed;
        public int TurretsLost;
        public int TurretsKilled;


        public void Reset()
        {
            DamageDone = 0;
            DamageTaken = 0;
            EnergyGained = 0;
            EnergyUsed = 0;
            ScoreGained = 0;
            SmallBombsGained = 0;
            SmallBombsUsed = 0;
            PlayerKills = 0;
            PlayerAssists = 0;
            UnitKills = 0;
            UnitsSpawned = 0;
            Deaths = 0;
            MiningPlatformsLost = 0;
            MiningPlatformsDestroyed = 0;
            TurretsLost = 0;
            TurretsKilled = 0;
        }
    }
}
