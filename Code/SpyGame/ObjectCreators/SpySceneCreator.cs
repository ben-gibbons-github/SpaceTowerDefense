using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot.SpyGame
{
    public class SpySceneCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(SpyScene);
            this.Catagory = "SpyGame";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new SpyScene();
        }
    }
}
