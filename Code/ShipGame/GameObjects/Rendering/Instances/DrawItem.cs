using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class DrawItem
    {
        public virtual void EmitParticle(int Layer, ref Vector3 Position, ref Matrix RotationMatrix, float Scale, float ColorMult) { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void DrawInstanced(LinkedList<BasicShipGameObject> ships, Camera3D DrawCamera) { }
        public virtual void DrawDistortion(LinkedList<BasicShipGameObject> ships, Camera3D DrawCamera) { }
        public virtual void DrawSingle(Vector3 Position, float Size, Vector4 Color, Camera3D DrawCamera) { }

        public virtual Vector3 GetWeaponPosition(Vector3 Position, ref Matrix RotationMatrix, int ID, float Scale) { return Vector3.Zero; }
    }
}
