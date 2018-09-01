using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class OrientedBoxShape : Basic3DShape
    {
        public BoundingOrientedBox MyBox = new BoundingOrientedBox(Vector3.Zero, Vector3.One / 2, Quaternion.Identity);

        public override bool Intersects(OrientedBoxShape other)
        {
            return MyBox.Intersects(ref other.MyBox);
        }

        public override bool Intersects(Point3D other)
        {
            return MyBox.Contains(ref other.Point);
        }

        public override void SetPosition(Vector3 Position)
        {
            MyBox.Center = Position;
        }

        public override void SetScale(Vector3 Scale)
        {
            MyBox.HalfExtent = Scale * Size / 2;
        }

        public override void SetRotation(Vector3 Rotation)
        {
            MyBox.Orientation = Quaternion.CreateFromYawPitchRoll(-MathHelper.ToRadians(Rotation.Y), MathHelper.ToRadians(Rotation.X), MathHelper.ToRadians(Rotation.Z));
        }

    }
}
