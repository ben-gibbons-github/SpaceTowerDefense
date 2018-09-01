using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Deferred3DEffect : _3DEffect
    {
        private EffectParameter WorldViewIT;
        private EffectParameter TextureSize;
        private EffectTechnique DeferredTechnique;
        private EffectParameter LightPosition;
        private EffectParameter LightDistance;
        private EffectParameter UVOffset;
        private EffectParameter UVMult;
        public EffectParameter ShadowReference;


        public override void SetUp()
        {
            WorldViewIT = Collection["WorldViewIT"];
            TextureSize = Collection["TextureSize"];
            LightPosition = Collection["LightPosition"];
            LightDistance = Collection["LightDistance"];
            ShadowReference = Collection["ShadowReference"];
            UVOffset = Collection["UVOffset"];
            UVMult = Collection["UVMult"];

            DeferredTechnique = FindTechnique("Deferred");
            base.SetUp();
        }

        public void SetWorldViewIT(Camera3D camera, Basic3DObject Object)
        {
            if (WorldViewIT != null)
                WorldViewIT.SetValue(Matrix.Transpose(Matrix.Invert(Object.WorldMatrix * camera.ViewMatrix)));
        }

        public void SetTextureSize(Vector2 Size)
        {
            if (TextureSize != null)
                TextureSize.SetValue(Size);
        }

        public void SetUV(Camera3D camera)
        {
            if (UVOffset != null)
                UVOffset.SetValue(camera.Offset);
            if (UVMult != null)
                UVMult.SetValue(camera.Mult);
        }

        public void SetDeferredTechnique()
        {
            if (DeferredTechnique != null)
                MyEffect.CurrentTechnique = DeferredTechnique;
        }

        public void SetLight(Vector3 LightPosition, Vector3 LightDistance)
        {
            if (this.LightDistance != null)
            {
                this.LightDistance.SetValue(LightDistance);
                this.LightPosition.SetValue(LightPosition);
            }
        }


    }
}
