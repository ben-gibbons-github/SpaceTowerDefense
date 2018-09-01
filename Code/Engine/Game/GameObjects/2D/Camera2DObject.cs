using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Camera2DObject : Basic2DObject
    {
        public Camera2D MyCamera;
        public FloatValue ZoomDistance;

#if EDITOR && WINDOWS
        private static bool Loaded=false;
        private static Texture2D CameraIcon;
#endif

        public override void Create()
        {
            base.Create();
            MyCamera = new Camera2D();
            Values.Remove(Size);

            ZoomDistance = new FloatValue("Zoom Offset", 1, ChangeZoom);

            Position.ChangeEvent = ChangePosition;
            Rotation.ChangeEvent = ChangeRotation;

            
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                Load();
                AddTag(GameObjectTag._2DForward);
            }
#endif
        }

#if EDITOR && WINDOWS
        new private static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                CameraIcon = AssetManager.Load<Texture2D>("Editor/CameraIcon");
            }
        }
#endif
        private void ChangeZoom()
        {
            MyCamera.SetZoom(ZoomDistance.get());
        }

        private void ChangePosition()
        {
            MyCamera.SetPosition(Position.get());
        }

        private void ChangeRotation()
        {
            MyCamera.SetRotation(Rotation.get());
        }

#if EDITOR && WINDOWS
        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (ParentLevel.LevelForEditing)
                Render.DrawSprite(CameraIcon, Position, Size, Rotation);
            base.Draw2D(DrawTag);
        }
#endif
    }
}
