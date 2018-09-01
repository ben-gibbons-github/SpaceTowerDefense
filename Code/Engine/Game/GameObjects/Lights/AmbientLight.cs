using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class AmbientLight : BasicLight
    {
        public override void Create()
        {
            AddTag(GameObjectTag._3DDeferredOverLighting);
            MyEffect = new EffectValue("Effect", "Deferred/AmbientLight", EffectHolderType.None);
            base.Create();
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (lightState == BasicLight.LightState.Dead || MyEffect.get() != null)
            {
                base.Draw3D(camera, DrawTag);
                return;
            }

            MyEffect.get().CurrentTechnique.Passes[0].Apply();
            FullscreenQuad.Draw();

            base.Draw3D(camera, DrawTag);
        }
    }
}
