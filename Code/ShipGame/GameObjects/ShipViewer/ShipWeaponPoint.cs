using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ShipWeaponPoint : Basic3DObject
    {
        private static Model PointModel;
        private static _3DEffect PointEffect;
        private static bool Loaded = false;

        public IntValue Layer;

        public override void Create()
        {
            Layer = new IntValue("Layer");
            base.Create();
            Load();
            AddTag(GameObjectTag._3DForward);
        }

        new private static void Load()
        {
            if (!Loaded)
            {
                PointModel = AssetManager.Load<Model>("Models/ShipGame/Point");
                PointEffect = (_3DEffect)new _3DEffect().Create("Effects/WhiteEffect");
                Loaded = true;
            }
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            PointEffect.SetFromCamera(camera);
            PointEffect.SetFromObject(this);
            Render.DrawModel(PointModel, PointEffect.MyEffect);
            base.Draw3D(camera, DrawTag);
        }
    }
}
