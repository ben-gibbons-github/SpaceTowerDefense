using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Logic
    {
        public static float PI = (float)Math.PI;

        public static float Clerp(float from, float to, float step)
        {
            float t = ((MathHelper.WrapAngle(to - from) * (step)));
            return from + t;
        }

        public static float DistanceLineSegmentToPoint(Vector2 A, Vector2 B, Vector2 p)
        {
            Vector2 v = B - A;
            v.Normalize();

            float distanceAlongLine = Vector2.Dot(p, v) - Vector2.Dot(A, v);
            Vector2 nearestPoint;

            if (distanceAlongLine < 0)
                nearestPoint = A;
            else if (distanceAlongLine > Vector2.Distance(A, B))
                nearestPoint = B;
            else
                nearestPoint = A + distanceAlongLine * v;

            float actualDistance = Vector2.Distance(nearestPoint, p);
            return actualDistance;
        }

        public static Rectangle Rect(Vector2 Position, Vector2 Size)
        {
            return new Rectangle((int)(Position.X - Size.X / 2), (int)(Position.Y - Size.Y / 2), (int)Size.X, (int)Size.Y);
        }

        public static Rectangle Rect(Vector2 Position, Vector2 Size, Matrix M)
        {
            Vector2 UpperLeft = Vector2.Transform(Position - Size / 2, M);
            Vector2 LowerRight = Vector2.Transform(Position + Size / 2, M);
            return new Rectangle((int)(UpperLeft.X), (int)(UpperLeft.Y), (int)(LowerRight.X - UpperLeft.X), (int)(LowerRight.Y - UpperLeft.Y));
        }

        public static float RLerp(float a, float b)
        {
            float R = (float)Rand.r.NextDouble();

            return MathHelper.Lerp(a, b, R);
        }

        public static float RLerp(float a, float b, Random R)
        {
            return MathHelper.Lerp(a, b, (float)R.NextDouble());
        }

        public static Vector3 RLerp(Vector3 a, Vector3 b)
        {
            float R = (float)Rand.r.NextDouble();

            return new Vector3(
                MathHelper.Lerp(a.X, b.X, R),
                MathHelper.Lerp(a.Y, b.Y, R),
                MathHelper.Lerp(a.Z, b.Z, R));
        }

        public static Vector3 RLerp(Vector3 a, Vector3 b, Random R)
        {
            return new Vector3(
                MathHelper.Lerp(a.X, b.X, (float)R.NextDouble()),
                MathHelper.Lerp(a.Y, b.Y, (float)R.NextDouble()),
                MathHelper.Lerp(a.Z, b.Z, (float)R.NextDouble()));
        }

        public static Vector4 RLerp(Vector4 a, Vector4 b)
        {
            float R = (float)Rand.r.NextDouble();

            return new Vector4(
                MathHelper.Lerp(a.X, b.X, R),
                MathHelper.Lerp(a.Y, b.Y, R),
                MathHelper.Lerp(a.Z, b.Z, R),
                MathHelper.Lerp(a.W, b.W, R));
        }

        public static Vector4 RLerp(Vector4 a, Vector4 b, Random R)
        {
            return new Vector4(
                MathHelper.Lerp(a.X, b.X, (float)R.NextDouble()),
                MathHelper.Lerp(a.Y, b.Y, (float)R.NextDouble()),
                MathHelper.Lerp(a.Z, b.Z, (float)R.NextDouble()),
                MathHelper.Lerp(a.W, b.W, (float)R.NextDouble()));
        }

        public static int ParseI(string s)
        {
            try
            {
                return int.Parse(s);
            }
            catch (Exception e)
            {
                MasterManager.e = e;
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public static float ParseF(string s)
        {
            try
            {
                return float.Parse(s);
            }
            catch (Exception e)
            {
                MasterManager.e = e;
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public static Vector2 ToVector2(float Angle)
        {
            return Vector2.Normalize(new Vector2((float)Math.Sin(Angle), (float)Math.Cos(Angle)));
        }

        public static float ToAngle(Vector2 Angle)
        {
            Angle.Normalize();
            return ((float)Math.Atan2(Angle.X, Angle.Y));
        }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X < b.X ? a.X : b.X, a.Y < b.Y ? a.Y : b.Y);
        }

        public static Vector2 Min(Vector2 a, Vector2 b, Vector2 c)
        {
            return new Vector2(
                a.X < b.X
                ?
                a.X < c.X ? a.X : c.X
                :
                b.X < c.X ? b.X : c.X
                ,

                a.Y < b.Y
                ?
                a.Y < c.Y ? a.Y : c.Y
                :
                b.Y < c.Y ? b.Y : c.Y
                );
        }

        public static Vector2 Max(Vector2 a, Vector2 b, Vector2 c)
        {
            return new Vector2(
                a.X > b.X
                ?
                a.X > c.X ? a.X : c.X
                :
                b.X > c.X ? b.X : c.X
                ,

                a.Y > b.Y
                ?
                a.Y > c.Y ? a.Y : c.Y
                :
                b.Y > c.Y ? b.Y : c.Y
                );
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X > b.X ? a.X : b.X, a.Y > b.Y ? a.Y : b.Y);
        }

        public static Basic2DObject Nearest(LinkedList<Basic2DObject> Items, Vector2 Position)
        {
            float BestDistance = 1000000;
            Basic2DObject BestObject = null;

            foreach (Basic2DObject o in Items)
            {
                float d = Vector2.Distance(o.getPosition(), Position);
                if (d < BestDistance)
                {
                    BestDistance = d;
                    BestObject = o;
                }
            }
            return BestObject;
        }

        public static Vector2 Clamp(Vector2 MapPosition, Vector2 Min, Vector2 Max)
        {
            return Logic.Max(Logic.Min(MapPosition, Max), Min);
        }
    }
}
