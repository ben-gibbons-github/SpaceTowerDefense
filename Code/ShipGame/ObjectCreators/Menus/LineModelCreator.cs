using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class LineModelCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(LineModel);
            this.Catagory = "ShipMenu";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new LineModel();
        }
    }
}
