using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CollisionBoxCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(CollisionBox);
            this.Catagory = "3D";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new CollisionBox();
        }
    }
}
