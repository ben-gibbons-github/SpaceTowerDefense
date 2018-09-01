using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ShipWeaponPointCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(ShipWeaponPoint);
            this.Catagory = "ShipViewer";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new ShipWeaponPoint();
        }
    }
}
