using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class WallNodeCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(WallNode);
            Createable = false;
            this.Catagory = "StarShip";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new WallNode();
        }
    }
}
