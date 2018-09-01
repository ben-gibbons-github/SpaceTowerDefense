using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class NullCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(NullObject);
            this.Catagory = "Basic";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new NullObject();
        }
    }
}
