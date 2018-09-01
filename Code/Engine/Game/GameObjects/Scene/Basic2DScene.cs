using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class Basic2DScene : SceneObject
    {
        public static Color GridColor = Color.LightGray;

        public CameraControls2D cameraControls;
        public ObjectControls2D objectControls;

        public Vector2Value GridSize;
        public BoolValue UseGrid;
        public ObjectValue MyCamera;
        public Vector2Value MinBoundary;
        public Vector2Value MaxBoundary;
        public Vector2Value Cells;

        public Camera2D DrawCamera;
        public LinkedList<QuadGrid> quadGrids = new LinkedList<QuadGrid>();
        public LinkedList<GameObject> UpdateChildren;

        public override void Create()
        {
            MyCamera = new ObjectValue("Camera2D", typeof(Camera2DObject));
            MyCamera.ChangeEvent = SetCamera;
            GridSize = new Vector2Value("Grid CellSize", new Vector2(32));
            UseGrid = new BoolValue("Use Grid");

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                UseGrid.ChangeEvent = SetUseGrid;
                UseGrid.Editable = false;
            }
#endif

            MinBoundary = new Vector2Value("Boundary Min", new Vector2(-320), UpdateQuadGrid);
            MaxBoundary = new Vector2Value("Boundary Max", new Vector2(320), UpdateQuadGrid);
            Cells = new Vector2Value("Cells", new Vector2(10), UpdateQuadGrid);
            UpdateQuadGrid();

            AddWindowForm(cameraControls = new CameraControls2D(this));
            AddWindowForm(objectControls = new ObjectControls2D(this));

            UpdateChildren = AddTag(GameObjectTag.Update);
            base.Create();
        }

        private void UpdateQuadGrid()
        {
            foreach (QuadGrid quadGrid in quadGrids)
                quadGrid.SetDimensions(MinBoundary.get(), MaxBoundary.get(), Cells.get());
        }

        public QuadGrid Add(QuadGrid quadGrid)
        {
            if (MinBoundary != null)
                quadGrid.SetDimensions(MinBoundary.get(), MaxBoundary.get(), Cells.get());
            quadGrids.AddFirst(quadGrid);
            return quadGrid;
        }

        public void DrawGrid()
        {
#if EDITOR && WINDOWS
            if (UseGrid.get() && GridSize.get().X > 0 && GridSize.get().Y > 0)
            {
                Vector2 StartPos = new Vector2(
                    (float)Math.Floor(DrawCamera.getTopLeftCorner().X / GridSize.get().X) * GridSize.get().X
                    ,
                    (float)Math.Floor(DrawCamera.getTopLeftCorner().Y / GridSize.get().Y) * GridSize.get().Y
                    );
                //StartPos.X = Math.Max(StartPos.X, MinBoundary.X());
                //StartPos.Y = Math.Max(StartPos.Y, MinBoundary.Y());

                Vector2 EndPos = DrawCamera.getBottomRightCorner();
                //EndPos.X = Math.Min(EndPos.X, MaxBoundary.X());
                //EndPos.Y = Math.Min(EndPos.Y, MaxBoundary.Y());

                for (float x = StartPos.X; x < EndPos.X; x += GridSize.get().X)
                    if (x >= MinBoundary.X() && x <= MaxBoundary.X())
                    {
                        Render.DrawLine(new Vector2(x, Math.Max(StartPos.Y, MinBoundary.Y())), new Vector2(x, Math.Min(EndPos.Y, MaxBoundary.Y())), GridColor * 0.5f, 1 / DrawCamera.getZoom());
                    }
                for (float y = StartPos.Y; y < EndPos.Y; y += GridSize.get().Y)
                    if (y >= MinBoundary.Y() && y <= MaxBoundary.Y())
                    {
                        Render.DrawLine(new Vector2(Math.Max(StartPos.X, MinBoundary.X()), y), new Vector2(Math.Min(EndPos.X, MaxBoundary.X()), y), GridColor * 0.5f, 1 / DrawCamera.getZoom());
                    }
            }
            Render.DrawOutlineRect(MinBoundary.get(), MaxBoundary.get(), 1 / DrawCamera.getZoom(), Color.Red);
#endif
        }

        public virtual void DrawScene(Camera2D DrawCamera)
        {

        }

#if EDITOR && WINDOWS
        private void SetUseGrid()
        {
            if (ParentLevel.LevelForEditing)
                objectControls.GridButton.Selected = UseGrid.get();
        }
#endif

        private void SetCamera()
        {
            Camera2DObject o = (Camera2DObject)MyCamera.get();
            if (o != null)
            {
                DrawCamera = o.MyCamera;
                DrawCamera.SetSize(WindowSize);
            }
        }

        public Vector2 getCameraPosition()
        {
            return DrawCamera.getPosition();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameObject g in UpdateChildren)
                g.Update(gameTime);
            foreach (GameObject g in UpdateChildren)
                g.Update2(gameTime);

            base.Update(gameTime);
        }

#if EDITOR && WINDOWS
        public override void UpdateEditor(GameTime gameTime)
        {
            if (WorldViewer.self.ContainsMouse && !WorldViewer.self.MouseOverForm)
                RayCast(gameTime);

            if (MyCamera.get() == null)
            {
                MyCamera.set(FindObject(typeof(Camera2DObject)));
                if (MyCamera.get() != null)
                {
                    if (EditorSelected)
                        UpdateSelected();
                }
                else
                {
                    Camera2DObject camera = new Camera2DObject();
                    ParentLevel.AddObject(camera, this);
                    MyCamera.set(camera);
                    camera.Name.set("EditorCamera");
                }
            }

            if (ParentLevel.LevelForEditing)
                base.UpdateEditor(gameTime);
        }

        public override bool RayCast(GameTime gameTime)
        {
            foreach (GameObject o in Children)
                if (o.RayCast(gameTime))
                    return true;

            if (!KeyboardManager.AltPressed())
                return objectControls.RayCast(gameTime);
            else
            {
                if (MouseManager.MouseClicked && CreatorBasic.LastCreator != null)
                {
                    GameObject o = CreatorBasic.LastCreator.ReturnObject();
                    ParentLevel.AddObject(o);
                    if (o.GetType().IsSubclassOf(typeof(Basic2DObject)))
                    {
                        Basic2DObject b = (Basic2DObject)o;
                        b.Position.set(Vector2.Transform(WorldViewer.self.RelativeMousePosition, Matrix.Invert(DrawCamera.ViewMatrix)));
                        ClearSelected();
                        AddSelected(b);
                        if (UseGrid.get())
                            SnapSelected();
                    }
                }
                return true;
            }
        }

        public void SnapSelected()
        {
            foreach (GameObject g in SelectedGameObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                {
                    Basic2DObject b = (Basic2DObject)g;
                    b.ApplySnap(false);
                }
        }

        public void MoveSelected(Vector2 Force)
        {
            foreach (GameObject g in SelectedGameObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                {
                    Basic2DObject b = (Basic2DObject)g;
                    b.ApplyMove(Force, false);
                }
        }

        public void ScaleSelected(Vector2 Force, Vector2 Origin)
        {
            foreach (GameObject g in SelectedGameObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                {
                    Basic2DObject b = (Basic2DObject)g;
                    b.ApplyScale(Force, Basic2DObject.GetAveragePosition(SelectedGameObjects), false);
                }
        }

        public void RotateSelected(float Force, Vector2 Origin)
        {
            foreach (GameObject g in SelectedGameObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic2DObject)))
                {
                    Basic2DObject b = (Basic2DObject)g;
                    b.ApplyRotate(Force, Basic2DObject.GetAveragePosition(SelectedGameObjects), false);
                }
        }
#endif

        public override void SetWindowSize(Vector2 WindowSize)
        {
            if (DrawCamera != null)
                DrawCamera.SetSize(WindowSize);
            base.SetWindowSize(WindowSize);
        }


 
    }
}
