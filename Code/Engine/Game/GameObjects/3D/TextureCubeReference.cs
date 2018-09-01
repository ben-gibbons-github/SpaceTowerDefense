using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using BadRabbit.Carrot.EffectParameters;

namespace BadRabbit.Carrot
{
    public class TextureCubeReference
    {
        private TextureCube MyCube;
        private LinkedList<TextureCubeParameter> ReferenceParameters = new LinkedList<TextureCubeParameter>();

        public TextureCubeReference() { }
        public TextureCubeReference(TextureCube MyCube) { this.MyCube = MyCube; }
        public TextureCube get() { return MyCube; }

        public TextureCube set(TextureCube cube)
        {
            MyCube = cube;

            foreach (TextureCubeParameter param in ReferenceParameters.ToArray())
                param.set(param.getPath());

            return cube;
        }

        public void AddReference(TextureCubeParameter param)
        {
            ReferenceParameters.AddLast(param);
        }

        public void RemoveReference(TextureCubeParameter param)
        {
            ReferenceParameters.Remove(param);
        }
    }
}
