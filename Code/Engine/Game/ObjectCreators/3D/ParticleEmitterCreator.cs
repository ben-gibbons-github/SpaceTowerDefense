using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ParticleEmitterCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(ParticleEmitter);
            this.Catagory = "3D";
            this.Createable = false;
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new ParticleEmitter();
        }
    }
}
