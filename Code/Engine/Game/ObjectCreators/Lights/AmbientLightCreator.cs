using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class AmbientLightCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(AmbientLight);
            this.Catagory = "Light";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new AmbientLight();
        }
    }
}
