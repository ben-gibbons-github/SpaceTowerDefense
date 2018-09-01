using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class Deferred3DSceneCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(Deferred3DScene);
            this.Catagory = "Scene";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new Deferred3DScene();
        }
    }
}
