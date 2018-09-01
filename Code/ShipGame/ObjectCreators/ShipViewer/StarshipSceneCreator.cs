using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class StarshipSceneCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(StarshipScene);
            this.Catagory = "Scene";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new StarshipScene();
        }
    }
}
