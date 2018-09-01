using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Basic3DObject : GameObject
    {
        public static void GetAveragePosition(LinkedList<GameObject> SelectObjects, ref Vector3 Result, ref bool Success)
        {
            Vector3 val = Vector3.Zero;
            int Count = 0;
            Success = false;

            foreach (GameObject g in SelectObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic3DObject)))
                {
                    Success = true;
                    Basic3DObject b = (Basic3DObject)g;
                    val += b.GetPosition();
                    Count++;
                }

            Result = val / Count;
        }

        public Basic3DShape CollisionShape;
        public Vector3Value Position, Scale, Rotation;
        public Matrix RotationMatrix = Matrix.Identity;
        public Matrix ScaleMatrix = Matrix.Identity;
        public Matrix PositionMatrix = Matrix.Identity;
        public Matrix WorldMatrix = Matrix.Identity;

        public override void Create()
        {
            Position = new Vector3Value("Position", EditType.Average, ChangePosition);
            Scale = new Vector3Value("Scale", Vector3.One, EditType.Scalar, ChangeScale);
            Rotation = new Vector3Value("Rotation", EditType.Average, ChangeRotation);

            MoveValuetoFront(Position, Scale, Rotation);

            base.Create();
        }

        public void SetCollisionShape(Basic3DShape shape)
        {
            CollisionShape = shape;
            if (Position != null)
            {
                shape.SetPosition(Position.get());
                shape.SetScale(Scale.get());
                shape.SetRotation(Rotation.get());
            }
        }

        public bool TestCollision(Basic3DObject other)
        {
            return CollisionShape.Intersects(other.CollisionShape);
        }

        public Vector3 GetPosition()
        {
            return Position.get();
        }

        public virtual void ChangePosition()
        {
            UpdatePositionMatrix();
            if (CollisionShape != null)
                CollisionShape.SetPosition(Position.get());
        }

        public virtual void ChangeScale()
        {
            UpdateScaleMatrix();
            if (CollisionShape != null)
                CollisionShape.SetScale(Scale.get());
        }

        public virtual void ChangeRotation()
        {
            UpdateRotationMatrix();
            if (CollisionShape != null)
                CollisionShape.SetRotation(Rotation.get());
        }

        private void UpdateScaleMatrix()
        {
            ScaleMatrix = Matrix.CreateScale(Scale.get());
            UpdateWorldMatrix();
        }

        private void UpdatePositionMatrix()
        {
            PositionMatrix = Matrix.CreateTranslation(Position.get());
            UpdateWorldMatrix();
        }

        private void UpdateRotationMatrix()
        {
            Vector3 Rotation = this.Rotation.get();
            RotationMatrix = Matrix.CreateFromYawPitchRoll(-MathHelper.ToRadians(Rotation.Y), MathHelper.ToRadians(Rotation.X), MathHelper.ToRadians(Rotation.Z));
            UpdateWorldMatrix();
        }

        public virtual void UpdateWorldMatrix()
        {
            WorldMatrix = ScaleMatrix * RotationMatrix * PositionMatrix;
        }

        public void ApplyMove(Vector3 Force, bool ApplyToChildren)
        {
            Position.add(Force);

            if (ApplyToChildren)
                foreach (GameObject g in HierarchyChildren)
#if EDITOR && WINDOWS
                    if (!ParentLevel.LevelForEditing || !g.EditorSelected)
#endif
                        if (g.GetType().IsSubclassOf(typeof(Basic3DObject)))
                        {
                            Basic3DObject b = (Basic3DObject)g;
                            b.ApplyMove(Force, true);
                        }
        }

        public void ApplyScale(Vector3 Force, Vector3 Origin, bool ApplyToChildren)
        {
            Scale.mult(Force);
            Position.set(Origin + (Position.get() - Origin) * Force);

            if (ApplyToChildren)
                foreach (GameObject g in HierarchyChildren)
#if EDITOR && WINDOWS
                    if (!ParentLevel.LevelForEditing || !g.EditorSelected)
#endif
                        if (g.GetType().IsSubclassOf(typeof(Basic3DObject)))
                        {
                            Basic3DObject b = (Basic3DObject)g;
                            b.ApplyScale(Force, Origin, true);
                        }

        }

        public void ApplyRotate(Vector3 Force, Vector3 Origin, bool ApplyToChildren)
        {
            Rotation.add(Force);

            if (ApplyToChildren)
                foreach (GameObject g in HierarchyChildren)
#if EDITOR && WINDOWS
                    if (!ParentLevel.LevelForEditing || !g.EditorSelected)
#endif
                        if (g.GetType().IsSubclassOf(typeof(Basic3DObject)))
                        {
                            Basic3DObject b = (Basic3DObject)g;
                            b.ApplyRotate(Force, Origin, true);
                        }

        }

    }
}
