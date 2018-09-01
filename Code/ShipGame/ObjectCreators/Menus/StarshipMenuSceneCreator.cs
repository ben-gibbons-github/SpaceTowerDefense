using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class StarshipMenuSceneCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(StarshipMenuScene);
            this.Catagory = "ShipMenu";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new StarshipMenuScene();
        }
    }
}
