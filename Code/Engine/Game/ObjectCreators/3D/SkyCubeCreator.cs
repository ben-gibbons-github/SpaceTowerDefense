using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class SkyTextureCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(SkyTexture);
            this.Catagory = "3D";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new SkyTexture();
        }
    }
}
