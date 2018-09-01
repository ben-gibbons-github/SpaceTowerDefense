using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class DistortionEffectContainer
    {
        public static Effect DistortionEffect;
        public static EffectParameter WorldParam;
        public static EffectParameter ProjectionParam;
        public static EffectParameter ViewParam;

        static DistortionEffectContainer()
        {
            DistortionEffect = AssetManager.Load<Effect>("Effects/ShipGame/NormalDistorter");

            EffectParameterCollection Params = DistortionEffect.Parameters;
            WorldParam = Params["World"];
            ViewParam = Params["View"];
            ProjectionParam = Params["Projection"];
        }
    }
}
