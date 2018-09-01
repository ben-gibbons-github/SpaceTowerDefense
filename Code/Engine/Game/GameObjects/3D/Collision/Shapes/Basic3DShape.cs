using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Basic3DShape
    {
        public Vector3 Size = Vector3.One;

        public bool Intersects(Basic3DShape other)
        {
            Type t = other.GetType();
            if (t.Equals(typeof(OrientedBoxShape)))
                return Intersects((OrientedBoxShape)other);
            if (t.Equals(typeof(Point3D)))
                return Intersects((Point3D)other);

            return false;
        }

        public virtual bool Intersects(OrientedBoxShape other)
        {
            return false;
        }

        public virtual bool Intersects(Point3D other)
        {
            return false;
        }

        public virtual void SetSize(Vector3 Size)
        {

        }

        public virtual void SetPosition(Vector3 Position)
        {

        }

        public virtual void SetScale(Vector3 Scale)
        {

        }

        public virtual void SetRotation(Vector3 Rotation)
        {

        }
    }
}
