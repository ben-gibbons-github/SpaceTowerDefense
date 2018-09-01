using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class WallChainCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(WallChain);
            this.Catagory = "StarShip";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new WallChain();
        }
    }
}
