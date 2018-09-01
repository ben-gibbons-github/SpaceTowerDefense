using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerSpawnCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(PlayerSpawn);
            this.Catagory = "StarShip";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new PlayerSpawn();
        }
    }
}
