using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ShipParticlePointCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(ShipParticlePoint);
            this.Catagory = "ShipViewer";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new ShipParticlePoint();
        }
    }
}
