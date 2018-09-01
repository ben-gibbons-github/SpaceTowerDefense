using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CameraFlybyCenterCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(CameraFlybyCenter);
            this.Catagory = "StarShip";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new CameraFlybyCenter();
        }
    }
}
