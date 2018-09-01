using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class Path3DNodeCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(Path3DNode);
            this.Catagory = "3D";
            this.Createable = false;
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new Path3DNode();
        }
    }
}
