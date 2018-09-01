using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class DirectionalLight : BasicLight
    {
        public override void Create()
        {
            AddTag(GameObjectTag._3DDeferredOverLighting);
            MyEffect = new EffectValue("Directional Effect", "Deferred/DirectionalLight", EffectHolderType.DeferredLight);
            base.Create();
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (lightState == BasicLight.LightState.Dead || MyEffect.Holder != null)
            {
                base.Draw3D(camera, DrawTag);
                return;
            }

            DeferredLightEffect effect3D = (DeferredLightEffect)MyEffect.Holder;
            effect3D.SetTextureSize(ParentScene.WindowSize);
            effect3D.SetInverseCamera(camera);
            effect3D.MyEffect.CurrentTechnique.Passes[0].Apply();
            FullscreenQuad.Draw();

            base.Draw3D(camera, DrawTag);
        }
    }
}
