using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ParticleEmitter: Basic3DObject
    {
        private static Vector4 drawColor = new Vector4(0.5f, 0.5f, 1, 1);

        public BoolValue Sphere;
        public FloatValue Delay;
        public float Timer;

        public override void Create()
        {
            Delay = new FloatValue("Delay", 60);
            Sphere = new BoolValue("Sphere");
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

        public Vector3 getRandomPosition()
        {
            if (!Sphere.get())
                return Position.get() + Vector3.Transform(Rand.V3() / 2, ScaleMatrix);
            else
                return Position.get() + Vector3.Transform(Rand.VECTV3() /2 ,ScaleMatrix);
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (!Sphere.get())
                ShapeRenderer.DrawCube(WorldMatrix, camera, drawColor);
            else
                ShapeRenderer.DrawSphere(WorldMatrix, camera, drawColor);

            base.Draw3D(camera, DrawTag);
        }
    }
}
