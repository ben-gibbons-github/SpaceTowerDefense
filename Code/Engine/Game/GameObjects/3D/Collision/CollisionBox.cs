using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CollisionBox : Basic3DObject
    {
        private static Vector4 drawColor = new Vector4(1, 0.25f, 0.25f, 1);

        public override void Create()
        {
            SetCollisionShape(new OrientedBoxShape());
            AddTag(GameObjectTag._3DSolid);

            base.Create();

#if EDITOR
            if (ParentLevel.LevelForEditing)
            {
                AddTag(GameObjectTag._3DForward);
                ShapeRenderer.Load();
                ApplyScale(Vector3.One * 200, Vector3.Zero, false);
            }
#endif
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            ShapeRenderer.DrawCube(WorldMatrix, camera, drawColor);
            base.Draw3D(camera, DrawTag);
        }
    }
}
