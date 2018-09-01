using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ShipScaleRing : Basic3DObject
    {
        private static Model RingModel;
        private static _3DEffect RingEffect;
        private static bool Loaded = false;

        public override void Create()
        {
            base.Create();
            Load();
            AddTag(GameObjectTag._3DForward);
            
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                AddTag(GameObjectTag._3DForward);
                ApplyScale(Vector3.One * 200, Vector3.Zero, false);
            }
#endif
        }

        new private static void Load()
        {
            if (!Loaded)
            {
                RingModel = AssetManager.Load<Model>("Models/ShipGame/Ring");
                RingEffect = (_3DEffect)new _3DEffect().Create("Effects/WhiteEffect");
                Loaded = true;
            }
        }

#if EDITOR && WINDOWS
        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (EditorSelected)
            {
                RingEffect.SetFromCamera(camera);
                RingEffect.SetFromObject(this); ;
                Render.DrawModel(RingModel, RingEffect.MyEffect);
                base.Draw3D(camera, DrawTag);
            }
        }
#endif
    }
}
