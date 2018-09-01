using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Rand
    {
        public static Random r = new Random();

        public static float F()
        {
            return (float)r.NextDouble();
        }

        public static double D()
        {
            return r.NextDouble();
        }

        public static Vector3 V3()
        {
            return new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()) * 2 - Vector3.One;
        }

        public static Vector2 V2()
        {
            return new Vector2((float)r.NextDouble(), (float)r.NextDouble()) * 2 - Vector2.One;
        }

        public static Vector2 NV2()
        {
            return Vector2.Normalize(new Vector2((float)r.NextDouble(), (float)r.NextDouble()) * 2 - Vector2.One);
        }

        public static Vector3 UPV3()
        {
            return new Vector3(0, (float)r.NextDouble(), 0);
        }

        public static Vector3 VECTV3()
        {
            return Vector3.Transform(UPV3(), Rotation());
        }

        public static Matrix Rotation()
        {
            return Matrix.CreateFromYawPitchRoll((float)(r.NextDouble() * Math.PI * 2), (float)(r.NextDouble() * Math.PI * 2), (float)(r.NextDouble() * Math.PI * 2));
        }
    }
}
