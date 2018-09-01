using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CameraFlybyNode : Basic2DObject
    {
        public FloatValue Z;
        public IntValue NodeOrder;

        public override void Create()
        {
            Z = new FloatValue("Z");
            NodeOrder = new IntValue("Node Order");
            base.Create();
        }
    }
}
