using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PointLightCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(PointLight);
            this.Catagory = "Light";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new PointLight();
        }
    }
}
