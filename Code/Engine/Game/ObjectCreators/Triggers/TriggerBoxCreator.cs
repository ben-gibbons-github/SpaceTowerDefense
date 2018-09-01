using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class TriggerBoxCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(TriggerBox);
            this.Catagory = "Triggers";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new TriggerBox();
        }
    }
}
