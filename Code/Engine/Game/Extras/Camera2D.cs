using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Camera2D
    {
        public Matrix ViewMatrix = Matrix.Identity;

        private float Zoom = 1;
        private Vector2 Position;
        private float Rotation = 0;
        private Vector2 ViewSize;
        public int QuadGridXMin, QuadGridYMin, QuadGridXMax, QuadGridYMax;

        private void Update()
        {
            ViewMatrix = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) * 
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) * 
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(ViewSize.X * 0.5f, ViewSize.Y * 0.5f, 0));
        }

        public void SetQuadGridPosition(QuadGrid quadGrid)
        {
            Vector2 UpperLeftCorner = getTopLeftCorner();
            Vector2 LowerRightCorner = getBottomRightCorner();

            QuadGridXMin = (int)MathHelper.Clamp((UpperLeftCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X,0,quadGrid.CellsX);
            QuadGridXMax = (int)MathHelper.Clamp((LowerRightCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X,0,quadGrid.CellsX-1);
            QuadGridYMin = (int)MathHelper.Clamp((UpperLeftCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y,0,quadGrid.CellsY);
            QuadGridYMax = (int)MathHelper.Clamp((LowerRightCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y, 0, quadGrid.CellsY-1);
        }

        public void SetRotation(float Rotation)
        {
            if (this.Rotation != Rotation)
            {
                this.Rotation = Rotation;
                Update();
            }
        }

        public void SetZoom(float Zoom)
        {
            if (this.Zoom != Zoom)
            {
                this.Zoom = Zoom;
                Update();
            }
        }

        public Vector2 getTopLeftCorner()
        {
            return Position - ViewSize / 2 / Zoom;
        }

        public Vector2 getBottomRightCorner()
        {
            return Position + ViewSize / 2 / Zoom;
        }

        public Vector2 getPosition()
        {
            return Position;
        }

        public float getZoom()
        {
            return Zoom;
        }

        public void SetPosition(Vector2 Position)
        {
            if (this.Position != Position)
            {
                this.Position = Position;
                Update();
            }
        }

        public void SetPositionNoUpdate(Vector2 Position)
        {
            this.Position = Position;
        }

        public void SetSize(Vector2 Size)
        {
            if (Size != ViewSize)
            {
                ViewSize = Size;
                Update();
            }
        }

        public void SetView(Vector2 Position, Vector2 Size)
        {
            this.Position = Position;
            this.ViewSize = Size;
            Update();
        }
    }
}
