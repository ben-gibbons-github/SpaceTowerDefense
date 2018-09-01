using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class WallConnector : WallItem
    {
        public Matrix WorldMatrix = Matrix.Identity;
        public WallNode ParentNode;
        public Vector2 PositionNext;

        public float LineIntercept;
        public float LineSlope;
        public bool LineIsVertical;

        public WallConnector(WallNode ParentNode)
        {
            this.ParentNode = ParentNode;
        }

        public void Destroy()
        {
            
#if EDITOR && WINDOWS
            if (!ParentNode.ParentLevel.LevelForEditing)
#endif
                WallInstancer.RemoveChild(this);
        }

        public string GetFname()
        {
            return ParentNode.ConnectorFile.get() != "" ? ParentNode.ConnectorFile.get() : ParentNode.ParentChain.ConnectorFile.get();
        }

        public Matrix GetMatrix()
        {
            return WorldMatrix;
        }

        public void SetFrom(Vector2 Position1, Vector2 Position2, float Width)
        {
            Vector3 Position3 = new Vector3((Position1.X + Position2.X) / 2, 0, (Position1.Y + Position2.Y) / 2);
            Vector3 Size = new Vector3(Width, Width, Vector2.Distance(Position1, Position2));
            float Rotation = (float)Math.Atan2(Position2.X - Position1.X, Position2.Y - Position1.Y);
            PositionNext = Position1;

            if (Position1.X == Position2.X)
            {
                LineSlope = 0;
                LineIsVertical = true;
            }
            else
                LineSlope = (Position1.Y - Position2.Y) / (Position1.X - Position2.X);
            LineIntercept = Position1.Y - LineSlope * Position1.X;

            WorldMatrix = Matrix.CreateScale(Size) * Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(Position3);
        }
    }
}
