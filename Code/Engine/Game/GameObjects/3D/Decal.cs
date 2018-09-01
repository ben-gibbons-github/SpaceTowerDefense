using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Decal : Basic3DObject
    {
        public EffectValue MyEffect;
        public ModelValue MyModel;

        public override void Create()
        {
            MyModel = new ModelValue("Model");
            MyEffect = new EffectValue("Effect", EffectHolderType.Basic3D);

            AddTag(GameObjectTag._3DDepthOver);
            base.Create();
        }



        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (MyModel.get() != null && MyEffect.get() != null)
            {
                Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
                _3DEffect effect3D = (_3DEffect)MyEffect.Holder;

                effect3D.SetFromObject(this);
                effect3D.SetFromCamera(camera);

                Render.DrawModel(MyModel.get(), MyEffect.get());
            }
            base.Draw3D(camera, DrawTag);
        }
    }
}
