using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ShipViewerSceneCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(ShipViewerScene);
            this.Catagory = "ShipViewer";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new ShipViewerScene();
        }
    }
}
