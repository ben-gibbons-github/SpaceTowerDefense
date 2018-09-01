using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class BasicModel : Basic3DObject
    {
        public BoolValue CastShadow;
        public BoolValue UseDeferred;
        public EffectValue MyEffect;
        public ModelValue MyModel;

        public override void Create()
        {
            CastShadow = new BoolValue("Cast Shadow",ClickShadow);

            UseDeferred = new BoolValue("Deferred Pass", ClickDeferred);
            MyModel = new ModelValue("Model");
            MyEffect = new EffectValue("Effect", EffectHolderType.Deferred3D);

            AddTag(GameObjectTag._3DForward);
            base.Create();
        }

        private void ClickShadow()
        {
            if (CastShadow.get())
                AddTag(GameObjectTag._3DShadow);
            else
                RemoveTag(GameObjectTag._3DShadow);
        }

        private void ClickDeferred()
        {
            if (UseDeferred.get())
                AddTag(GameObjectTag._3DDeferredGBuffer);
            else
                RemoveTag(GameObjectTag._3DDeferredGBuffer);
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (MyModel.get() != null && MyEffect.get() != null)
            {
                Deferred3DEffect effect3D = (Deferred3DEffect)MyEffect.Holder;
                switch (DrawTag)
                {
                    case GameObjectTag._3DDeferredGBuffer:
                        {
                            effect3D.SetWorldViewIT(camera, this);
                            effect3D.SetFromObject(this);
                            effect3D.SetFromCamera(camera);
                            effect3D.SetDeferredTechnique();
                            break;
                        }
                    case GameObjectTag._3DShadow:
                        {
                            effect3D.SetFromObject(this);
                            effect3D.SetFromCamera(camera);
                            effect3D.SetShadowTechnique();
                            effect3D.SetLight(Transfer.LightPosition, Transfer.LightDistance);
                            break;
                        }
                    default:
                        {
                            effect3D.SetFromCamera(camera);
                            effect3D.SetUV(camera);

                            if (!UseDeferred.get())
                                effect3D.SetFromObject(this);

                            effect3D.SetForwardTechnique();
                            break;
                        }
                }
                 
                Render.DrawModel(MyModel.get(), MyEffect.get());

            }
            base.Draw3D(camera, DrawTag);
        }
    }
}
