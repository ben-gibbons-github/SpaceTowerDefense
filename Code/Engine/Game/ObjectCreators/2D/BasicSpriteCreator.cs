using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class BasicSpriteCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(BasicSprite);
            this.Catagory = "2D";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new BasicSprite();
        }
    }
}
