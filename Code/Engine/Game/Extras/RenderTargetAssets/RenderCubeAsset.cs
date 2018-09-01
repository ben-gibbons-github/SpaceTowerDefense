using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot.RenderTargetAssets
{
    public class RenderCubeAsset : RenderTargetAsset
    {
        public RenderCubeAsset(RenderTargetCube Value)
        {
            this.Value = Value;
        }

        public RenderTargetCube Value;
    }
}
