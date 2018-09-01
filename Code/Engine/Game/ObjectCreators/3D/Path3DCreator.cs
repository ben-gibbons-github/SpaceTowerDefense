using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class Path3DCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(Path3D);
            this.Catagory = "3D";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new Path3D();
        }
    }
}
