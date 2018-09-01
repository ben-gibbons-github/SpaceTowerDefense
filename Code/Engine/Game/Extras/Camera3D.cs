using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace BadRabbit.Carrot
{
    public class Camera3D
    {
        public AudioListener Listener = new AudioListener();

        public Vector3 PreviousPosition;

        public Vector3 Up = Vector3.Up;
        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;
        public Vector3 LookAt;
        public Vector3 Position;
        public Matrix InverseView;
        public Matrix InverseViewProjection;
        public Vector2 Size, Offset, Mult = Vector2.One;
        public float Fov, NearPlane, FarPlane;
        public BoundingFrustum Bounds = new BoundingFrustum(Matrix.Identity);

        public Camera3D()
        {
            this.NearPlane = 10;
            this.FarPlane = 1000000;
            this.Fov = MathHelper.PiOver4;
        }

        public Camera3D(float Fov, float NearPlane, float FarPlane)
        {
            this.NearPlane = NearPlane;
            this.FarPlane = FarPlane;
            this.Fov = Fov;
        }

        public Camera3D(float Fov, float NearPlane, float FarPlane, Vector2 Size)
        {
            this.Size = Size;
            this.NearPlane = NearPlane;
            this.FarPlane = FarPlane;
            this.Fov = Fov;
        }

        public void SetSize(Vector2 Size)
        {
            this.Size = Size;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(Fov, Size.X / Size.Y, NearPlane, FarPlane);
            Bounds.Matrix = ViewMatrix * ProjectionMatrix;
        }

        public void SetOffset(Vector2 Offset)
        {
            this.Offset = Offset;
        }

        public void SetLookAt(Vector3 Position, Vector3 LookAt)
        {
            this.PreviousPosition = this.Position;
            this.Position = Position;
            this.LookAt = LookAt;
            Update();
        }

        public void SetLookAt(Vector3 Position, Vector3 LookAt, Vector3 Up)
        {
            this.Up = Up;
            SetLookAt(Position, LookAt);
        }

        public void SetTopDownView(Vector3 LookAt, Vector3 OffSet, Vector2 Distance, Vector2 LookAtDistance, float Direction)
        {
            this.LookAt = LookAt + new Vector3(
(float)Math.Cos(Direction) * LookAtDistance.X, LookAtDistance.Y, (float)Math.Sin(Direction) * LookAtDistance.X);

            Position = LookAt + OffSet + new Vector3(
(float)Math.Cos(Direction) * Distance.X, Distance.Y, (float)Math.Sin(Direction) * Distance.X);

            Update();
        }

        public void Update()
        {
            ViewMatrix = Matrix.CreateLookAt(Position, LookAt, Up);
            Bounds.Matrix = ViewMatrix * ProjectionMatrix;

            Listener.Position = (LookAt + LookAt + Position) / 3;
            Listener.Forward = Vector3.Normalize(LookAt - Position);
            Listener.Up = Up;
            Listener.Velocity = Position - PreviousPosition;
        }

        public void MakeInverse()
        {
            InverseView = Matrix.Invert(ViewMatrix);
            InverseViewProjection = Matrix.Invert(ViewMatrix * ProjectionMatrix);
        }

        internal void SetMult(Vector2 Mult)
        {
            this.Mult = Mult;
        }
    }
}
