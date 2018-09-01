using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class Forward3DSceneCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(Forward3DScene);
            this.Catagory = "Scene";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new Forward3DScene();
        }
    }
}
