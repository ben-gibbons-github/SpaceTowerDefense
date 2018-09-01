using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Lit3DEffect : _3DEffect
    {
        private EffectParameter AmbientLightColor;
        private EffectParameter LightOneDirection;
        private EffectParameter LightOneColor;
        private EffectParameter LightTwoDirection;
        private EffectParameter LightTwoColor;

        public override void SetUp()
        {
            AmbientLightColor = Collection["AmbientLightColor"];
            LightOneDirection = Collection["LightOneDirection"];
            LightTwoDirection = Collection["LightTwoDirection"];
            LightOneColor = Collection["LightOneColor"];
            LightTwoColor = Collection["LightTwoColor"];
            base.SetUp();
        }

        public void SetAmbientLight(Vector4 Color)
        {
            if (AmbientLightColor != null)
                AmbientLightColor.SetValue(Color);
        }

        public void SetLightOne(Vector4 Color, Vector3 Direction)
        {
            if (LightOneColor != null)
            {
                LightOneColor.SetValue(Color);
                LightOneDirection.SetValue(Direction);
            }
        }

        public void SetLightTwo(Vector4 Color, Vector3 Direction)
        {
            if (LightTwoColor != null)
            {
                LightTwoColor.SetValue(Color);
                LightTwoDirection.SetValue(Direction);
            }
        }
    }
}
