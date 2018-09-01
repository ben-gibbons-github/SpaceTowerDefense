using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SkyBox : Basic3DObject
    {
        public EffectValue MyEffect;
        public static Model SkyboxModel;
        public static bool Loaded = false;

        public override void Create()
        {
            MyEffect = new EffectValue("Effect", "SkyBox", EffectHolderType.Basic3D);
            Load();
            base.Create();
            AddTag(GameObjectTag._3DBackground);
        }

        new public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                SkyboxModel = AssetManager.Load<Model>("Models/Skybox");
            }
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            Render.DrawModel(SkyboxModel, MyEffect, camera, this);
            base.Draw3D(camera, DrawTag);
        }
    }
}
