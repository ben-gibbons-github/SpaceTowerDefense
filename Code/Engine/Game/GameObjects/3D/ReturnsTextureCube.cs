using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public abstract class ReturnsTextureCube: Basic3DObject
    {
        public TextureCubeReference MyCube;

        public TextureCubeReference returnTextureCube()
        {
            return MyCube;
        }
    }
}
