using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BadRabbit.Carrot.EffectParameters;

namespace BadRabbit.Carrot
{
    public class ShipViewer : Basic3DObject
    {
        public ModelValue model;
        public EffectValue effect;

        public ColorValue AmbientLightColor; 

        public Vector3Value LightOneDirection;
        public ColorValue LightOneColor;

        public Vector3Value LightTwoDirection;
        public ColorValue LightTwoColor;


        public override void Create()
        {
            model = new ModelValue("Model");
            effect = new EffectValue("Effect", EffectHolderType.Lit3D);

            AmbientLightColor = new ColorValue("Ambient Light Color", new Vector4(0.5f, 0.5f, 0.5f, 1), 0.2f);
            LightOneDirection = new Vector3Value("Light One Direction", Vector3.One);
            LightOneColor = new ColorValue("Light One Color", new Vector4(1, 0.9f, 0.85f, 1));
            LightTwoDirection = new Vector3Value("Light Two Direction", new Vector3(-1, 1, -1));
            LightTwoColor = new ColorValue("Light Two Color", new Vector4(0.1f, 0.3f, 0.65f, 1));

            base.Create();

            AddTag(GameObjectTag._3DForward);
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            Lit3DEffect effect3D = (Lit3DEffect)effect.Holder;
            if (effect3D != null)
            {
                effect3D.SetForwardTechnique();
                effect3D.SetAmbientLight(AmbientLightColor.get());
                effect3D.SetLightOne(LightOneColor.get(), Vector3.Normalize(LightOneDirection.get()));
                effect3D.SetLightTwo(LightTwoColor.get(), Vector3.Normalize(LightTwoDirection.get()));

                Render.DrawModel(model, effect, camera, this);
            }
            base.Draw3D(camera, DrawTag);
        }
    }
}
