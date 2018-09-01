using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class Forward2DSceneCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(Forward2DScene);
            this.Catagory = "Scene";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new Forward2DScene();
        }
    }
}
