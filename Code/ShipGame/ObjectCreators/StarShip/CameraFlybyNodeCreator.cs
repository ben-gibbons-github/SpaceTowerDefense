using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CameraFlybyNodeCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(CameraFlybyNode);
            this.Catagory = "StarShip";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new CameraFlybyNode();
        }
    }
}
