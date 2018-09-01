using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot.SpyGame
{
    public class SpyPlayerSpawnCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(SpyPlayerSpawn);
            this.Catagory = "SpyGame";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new SpyPlayerSpawn();
        }
    }
}
