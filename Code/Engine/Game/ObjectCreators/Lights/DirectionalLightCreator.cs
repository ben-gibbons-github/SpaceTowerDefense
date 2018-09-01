using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class DirectionalLightCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(DirectionalLight);
            this.Catagory = "Light";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new DirectionalLight();
        }
    }
}
