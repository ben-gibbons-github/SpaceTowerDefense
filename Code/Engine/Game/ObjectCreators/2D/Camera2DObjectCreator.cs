using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class Camera2DObjectCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(Camera2DObject);
            this.Catagory = "2D";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new Camera2DObject();
        }
    }
}
