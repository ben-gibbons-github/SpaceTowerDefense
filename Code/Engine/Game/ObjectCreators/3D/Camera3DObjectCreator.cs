using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class Camera3DObjectCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(Camera3DObject);
            this.Catagory = "3D";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new Camera3DObject();
        }
    }
}
