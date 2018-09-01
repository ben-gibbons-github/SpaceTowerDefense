using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Basic2DObject : GameObject
    {
        public static Vector2 GetAveragePosition(LinkedList<GameObject> SelectObjects)
        {
            Vector2 val = Vector2.Zero;
            int Count = 0;

            foreach (GameObject g in SelectObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                {
                    Basic2DObject b = (Basic2DObject)g;
                    val += b.getPosition();
                    Count++;
                }
            return val / Count;
        }

        public Vector2Value Position, Size;
        public FloatValue Rotation;
        public Basic2DScene Parent2DScene;
        private Vector2 RotationV2;
        public int QuadGridXMin, QuadGridYMin, QuadGridXMax, QuadGridYMax;

        public override void SetParents(Level ParentLevel, SceneObject ParentScene)
        {
            Parent2DScene = ParentScene.GetType().IsSubclassOf(typeof(Basic2DScene)) || ParentScene.GetType().Equals(typeof(Basic2DScene)) ?
                (Basic2DScene)ParentScene : null;
            base.SetParents(ParentLevel, ParentScene);
        }
#if EDITOR && WINDOWS
        public override bool RayCast(GameTime gameTime)
        {
            if (MouseManager.MouseClicked && !EditorSelected)
            {
                if (Logic.Rect(Position.get(), Size.get(), Parent2DScene.DrawCamera.ViewMatrix).Contains(WorldViewer.self.RelativeMousePoint))
                {
                    if (!KeyboardManager.ControlPressed())
                    {
                        if (!KeyboardManager.ShiftPressed())
                            ParentScene.ClearSelected();
                        ParentScene.AddSelected(this);
                    }
                    else
                    {
                        if (EditorSelected)
                            ParentScene.RemoveSelected(this);
                        else
                            ParentScene.AddSelected(this);
                    }
                    ParentLevel.ModifyWindows();
                    return true;
                }
            }
            else if (MouseManager.RMouseClicked)
            {
                if (Logic.Rect(Position.get(), Size.get(), Parent2DScene.DrawCamera.ViewMatrix).Contains(WorldViewer.self.RelativeMousePoint))
                {
                    if (!KeyboardManager.AltPressed())
                    {
                        ParentScene.ClearSelected();
                        ParentScene.AddSelected(this);
                        RightClick(gameTime);
                        ParentLevel.ModifyWindows();
                    }
                    else
                        Destroy();

                    return true;
                }
            }
            return false;
        }
#endif

        public override void Create()
        {
            Position = new Vector2Value("Position", EditType.Average);

            Size = new Vector2Value("Size", Vector2.One, EditType.Scalar);
            Rotation = new FloatValue("Angle", RotationChange);
            MoveValuetoFront(Position, Size, Rotation);


#if EDITOR && WINDOWS
            if (ParentScene != null && ParentScene.GetType().IsSubclassOf(typeof(Basic2DScene)))
            {
                Basic2DScene s = (Basic2DScene)ParentScene;
                if (s.GridSize != null)
                    Size.set(s.GridSize.get());
            }
#endif

            base.Create();
        }


        public void SetQuadGridPosition(Vector2 UpperLeftCorner, Vector2 LowerRightCorner)
        {
            QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

            QuadGridXMin = (int)((UpperLeftCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridXMax = (int)((LowerRightCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridYMin = (int)((UpperLeftCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);
            QuadGridYMax = (int)((LowerRightCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);

            QuadGridXMin = QuadGridXMin > quadGrid.CellsX - 1 ? quadGrid.CellsX - 1 : QuadGridXMin > 0 ? QuadGridXMin : 0;
            QuadGridXMax = QuadGridXMax > quadGrid.CellsX - 1 ? quadGrid.CellsX - 1 : QuadGridXMax > 0 ? QuadGridXMax : 0;
            QuadGridYMin = QuadGridYMin > quadGrid.CellsY - 1 ? quadGrid.CellsY - 1 : QuadGridYMin > 0 ? QuadGridYMin : 0;
            QuadGridYMax = QuadGridYMax > quadGrid.CellsY - 1 ? quadGrid.CellsY - 1 : QuadGridYMax > 0 ? QuadGridYMax : 0;
        }


        public void SetQuadGridPosition()
        {
            QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

            Vector2 UpperLeftCorner = getUpperLeftCorner();
            Vector2 LowerRightCorner = getLowerRightCorner();

            QuadGridXMin = (int)((UpperLeftCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridXMax = (int)((LowerRightCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridYMin = (int)((UpperLeftCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);
            QuadGridYMax = (int)((LowerRightCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);

            QuadGridXMin = QuadGridXMin > quadGrid.CellsX - 1 ? quadGrid.CellsX - 1 : QuadGridXMin > 0 ? QuadGridXMin : 0;
            QuadGridXMax = QuadGridXMax > quadGrid.CellsX - 1 ? quadGrid.CellsX - 1 : QuadGridXMax > 0 ? QuadGridXMax : 0;
            QuadGridYMin = QuadGridYMin > quadGrid.CellsY - 1 ? quadGrid.CellsY - 1 : QuadGridYMin > 0 ? QuadGridYMin : 0;
            QuadGridYMax = QuadGridYMax > quadGrid.CellsY - 1 ? quadGrid.CellsY - 1 : QuadGridYMax > 0 ? QuadGridYMax : 0;
        }

        public virtual void RotationChange()
        {
            float Rotation = this.Rotation.getAsRadians();
            RotationV2 = Vector2.Normalize(new Vector2((float)Math.Sin(Rotation), (float)Math.Cos(Rotation)));
        }

#if EDITOR && WINDOWS
        private void PositionChange()
        {
            if (ParentScene != null && ParentScene.GetType().IsSubclassOf(typeof(Basic2DScene)))
            {
                Basic2DScene s = (Basic2DScene)ParentScene;
                if (s.UseGrid.get())
                    ApplySnap(false);
            }
        }
#endif


        public void ApplyMove(Vector2 Force, bool ApplyToChildren)
        {
            Position.add(Force);

            if (ApplyToChildren)
                foreach (GameObject g in HierarchyChildren)
#if EDITOR && WINDOWS
                    if (!ParentLevel.LevelForEditing || !g.EditorSelected)
#endif
                        if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                        {
                            Basic2DObject b = (Basic2DObject)g;
                            b.ApplyMove(Force, true);
                        }
        }

        public void SetRotation(float Rotation, bool ApplyToChildren)
        {
            ApplyRotate(Rotation - this.Rotation.get(),Position.get(), ApplyToChildren);
        }

        public void SetRotation(Vector2 Rotation)
        {
            Rotation.Normalize();
            this.Rotation.set(MathHelper.ToDegrees((float)Math.Atan2(Rotation.X, Rotation.Y)));
        }

        public void SetRotation(float Rotation)
        {
            this.Rotation.set(Rotation);
        }

        public void SetPosition(Vector2 Position)
        {
            this.Position.set(Position);
        }

        public void SetPosition(Vector2 Position, bool ApplyToChildren)
        {
            ApplyMove(Position - this.Position.get(), ApplyToChildren);
        }

        public void ApplySnap(bool ApplyToChildren)
        {
            if (ParentScene != null && ParentScene.GetType().IsSubclassOf(typeof(Basic2DScene)))
            {
                Basic2DScene s = (Basic2DScene)ParentScene;
                if (s.GridSize != null)
                {
                    Vector2 GridSize = s.GridSize.get();
                    Vector2 Pos = Position.get();

                    Position.set(new Vector2((float)Math.Round(Position.get().X / s.GridSize.get().X), 
                        (float)Math.Round(Position.get().Y / s.GridSize.get().Y)) * s.GridSize.get());
                }
            }

            if (ApplyToChildren)
                foreach (GameObject g in HierarchyChildren)
#if EDITOR && WINDOWS
                    if (!ParentLevel.LevelForEditing || !g.EditorSelected)
#endif
                        if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                        {
                            Basic2DObject b = (Basic2DObject)g;
                            b.ApplySnap(true);
                        }
        }

        public void ApplyScale(Vector2 Force, Vector2 Origin, bool ApplyToChildren)
        {
            Size.mult(Force);
            Position.set(Origin + (Position.get() - Origin) * Force);

            if (ApplyToChildren)
                foreach (GameObject g in HierarchyChildren)
#if EDITOR && WINDOWS
                    if (!ParentLevel.LevelForEditing || !g.EditorSelected)
#endif
                        if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                        {
                            Basic2DObject b = (Basic2DObject)g;
                            b.ApplyScale(Force, Origin, true);
                        }
        }

        public void ApplyRotate(float Force, Vector2 Origin, bool ApplyToChildren)
        {
            SetRotation(Rotation.get() + Force);

            if (ApplyToChildren)
                foreach (GameObject g in HierarchyChildren)
#if EDITOR && WINDOWS
                    if (!ParentLevel.LevelForEditing || !g.EditorSelected)
#endif
                        if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                        {
                            Basic2DObject b = (Basic2DObject)g;
                            b.ApplyRotate(Force, Origin, true);
                        }
        }

        public Vector2 getSize()
        {
            return Size.get();
        }

        public Vector2 getPosition()
        {
            return Position.get();
        }

        public Vector2 getUpperLeftCorner()
        {
            return Position.get() - Size.get() / 2;
        }

        public Vector2 getLowerRightCorner()
        {
            return Position.get() + Size.get() / 2;
        }

        public float getRotation()
        {
            return Rotation.get();
        }

        public Vector2 getRotationV2()
        {
            return RotationV2;
        }

        public virtual bool CheckCollision(Basic2DObject Tester)
        {
            return false;
        }
    }
}
