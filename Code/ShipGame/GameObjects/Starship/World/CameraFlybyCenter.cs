using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CameraFlybyCenter : Basic2DObject
    {
        public FloatValue Z;
        public IntValue CenterOrder;

        public override void Create()
        {
            Z = new FloatValue("Z");
            CenterOrder = new IntValue("Center Order");
            base.Create();
        }
    }
}
