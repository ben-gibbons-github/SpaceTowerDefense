using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class TurretSpawnCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(TurretSpawn);
            this.Catagory = "StarShip";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new TurretSpawn();
        }
    }
}
