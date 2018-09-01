using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class InstanceModelList
    {
        private static Dictionary<int, string> list;

        public static Dictionary<int, string> GetList()
        {
            if (list == null)
            {
                Dictionary<int, string> R = new Dictionary<int, string>();

                R.Add(0, "World/Asteroid1");
                R.Add(1, "Extra/BustedTurret");
                R.Add(2, null);

                R.Add(0 + InstanceManager.WorldIndex, "Human/PlanetRing");
                R.Add(1 + InstanceManager.WorldIndex, "Human/PlanetRing");
                R.Add(2 + InstanceManager.WorldIndex, "Human/Player");

                R.Add(InstanceManager.HumanBasicIndex + 0, "Human/Ship1");
                R.Add(InstanceManager.HumanBasicIndex + 1, "Human/Ship2");
                R.Add(InstanceManager.HumanBasicIndex + 2, "Human/Ship3");
                R.Add(InstanceManager.HumanBasicIndex + 3, "Human/Ship4");
                R.Add(InstanceManager.HumanBasicIndex + 4, "Human/Ship5");

                R.Add(InstanceManager.HumanUnitIndex + 0, "Human/Turret1");
                R.Add(InstanceManager.HumanUnitIndex + 1, "Human/Turret2");
                R.Add(InstanceManager.HumanUnitIndex + 2, "Human/Turret3");
                R.Add(InstanceManager.HumanUnitIndex + 3, "Human/Turret4");

                list = R;
                return R;
            }
            else
                return list;
        }
    }
}
