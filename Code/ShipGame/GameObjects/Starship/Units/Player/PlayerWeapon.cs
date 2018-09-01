using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerWeapon
    {
        public static float GetRingMult()
        {
            return 8 + WaveManager.CurrentWave / 5f;
        }

        public static float GetTurretMult()
        {
            return 1.5f + WaveManager.CurrentWave / 10f;
        }

        public static float GetPlayerMult()
        {
            return 2.5f + WaveManager.CurrentWave / 5f;
        }

        public static float GetTurretVsPlayer()
        {
            return 0.75f;
        }
    }
}
