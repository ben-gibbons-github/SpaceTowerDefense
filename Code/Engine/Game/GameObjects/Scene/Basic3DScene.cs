using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Basic3DScene : SceneObject
    {
        public CameraControls3D cameraControls;
        public ObjectControls3D objectControls;

        public ObjectValue MyCamera;
        public Camera3D DrawCamera;

        public override void Create()
        {
            MyCamera = new ObjectValue("Camera3D", typeof(Camera3DObject));
            MyCamera.ChangeEvent = SetCamera;
            
            AddWindowForm(cameraControls = new CameraControls3D(this));
            AddWindowForm(objectControls = new ObjectControls3D(this));

            base.Create();
        }

        public override void SetWindowSize(Vector2 WindowSize)
        {
            if (DrawCamera != null)
                DrawCamera.SetSize(WindowSize);
             
            base.SetWindowSize(WindowSize);
        }

        private void SetCamera()
        {
            Camera3DObject o = (Camera3DObject)MyCamera.get();
            if (o != null)
            {
                DrawCamera = o.MyCamera;
                DrawCamera.SetSize(WindowSize);
            }
        }

        public override void PlayerJoinedEvent(PlayerProfile p)
        {
            foreach(GameObject g in WorldViewerChildren)
                if (g.GetType().Equals(typeof(Camera3DObject)))
                {
                    Camera3DObject cam = (Camera3DObject)g;
                    if (cam.MyPlayer == null && cam.AllowTakeover.get())
                    {
                        cam.MyPlayer = p;
                        return;
                    }
                }
        }

        public override void PlayerQuitEvent(PlayerProfile p)
        {
            foreach (GameObject g in WorldViewerChildren)
                if (g.GetType().Equals(typeof(Camera3DObject)))
                {
                    Camera3DObject cam = (Camera3DObject)g;
                    if (cam.MyPlayer == p)
                    {
                        cam.MyPlayer = null;
                        return;
                    }
                }
        }

#if EDITOR && WINDOWS
        public override void UpdateEditor(GameTime gameTime)
        {
            if (WorldViewer.self.ContainsMouse && !WorldViewer.self.MouseOverForm)
                RayCast(gameTime);

            if (MyCamera.get() == null)
            {
                MyCamera.set(FindObject(typeof(Camera3DObject)));
                if (MyCamera.get() != null)
                {
                    if (EditorSelected)
                        UpdateSelected();
                }
                else
                {
                    Camera3DObject camera = new Camera3DObject();
                    ParentLevel.AddObject(camera, this);
                    MyCamera.set(camera);
                    camera.Name.set("EditorCamera");
                }
            }
            if (ParentLevel.LevelForEditing)
                base.UpdateEditor(gameTime);
        }

        public virtual void DrawScene(Camera3D DrawCamera)
        {

        }

        public override bool RayCast(GameTime gameTime)
        {
            if (!objectControls.RayCast(gameTime))
            {
                foreach (GameObject o in Children)
                    if (o.RayCast(gameTime))
                        return true;
                return false;
            }
            else
                return true;
        }

        public void MoveSelected(Vector3 Force)
        {
            foreach (GameObject g in SelectedGameObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic3DObject)))
                {
                    Basic3DObject b = (Basic3DObject)g;
                    b.ApplyMove(Force, false);
                }
        }

        public void ScaleSelected(Vector3 Force, Vector3 Origin)
        {
            Vector3 Result = Vector3.Zero;
            bool Success = false;
            Basic3DObject.GetAveragePosition(SelectedGameObjects, ref Result, ref Success);

            foreach (GameObject g in SelectedGameObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic3DObject)))
                {
                    Basic3DObject b = (Basic3DObject)g;
                    b.ApplyScale(Force, Result, false);
                }
        }

        public void RotateSelected(Vector3 Force, Vector3 Origin)
        {
            Vector3 Result = Vector3.Zero;
            bool Success = false;
            Basic3DObject.GetAveragePosition(SelectedGameObjects, ref Result, ref Success);

            foreach (GameObject g in SelectedGameObjects)
                if (g.GetType().IsSubclassOf(typeof(Basic3DObject)))
                {
                    Basic3DObject b = (Basic3DObject)g;
                    b.ApplyRotate(Force, Result, false);
                }
        }
#endif

    }
}
