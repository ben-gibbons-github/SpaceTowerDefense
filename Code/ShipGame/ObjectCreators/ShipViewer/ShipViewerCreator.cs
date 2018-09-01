using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ShipViewerCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(ShipViewer);
            this.Catagory = "ShipViewer";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new ShipViewer();
        }
    }
}
