using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Point3D : Basic3DShape
    {
        public Vector3 Point;

        public override bool Intersects(OrientedBoxShape other)
        {
            return other.MyBox.Contains(ref Point);
        }

        public override bool Intersects(Point3D other)
        {
            return other.Point == Point;
        }

        public override void SetPosition(Vector3 Position)
        {
            Point = Position;
        }
    }
}
