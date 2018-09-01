using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public enum LockMode
    {
        None,
        Rotate,
        Scale,
        MoveX,
        MoveY,
        MoveZ,
        MoveAll,
    }

    public class CameraControls3D : Form
    {
#if EDITOR && WINDOWS
        public Vector2 MouseLockPosition;
        public float ZoomLock = 1000;
        public LockMode CurrentMode = LockMode.None;
        private Camera3DObject editorCamera;

        public int Counter = 0;

        public static bool Loaded = false;
        public static Texture2D RotateIcon;
        public static Texture2D ScaleIcon;
        public static Texture2D MoveIcon;
        public static Texture2D MoveXIcon;
        public static Texture2D MoveYIcon;
        public static Texture2D MoveZIcon;
        public static Texture2D CenterIcon;
        public static Texture2D ResetIcon;
        
        public Vector2 TopRightCorner;
        public List<Form> MyForms = new List<Form>();

        public Basic3DScene ParentScene;
#endif

        public CameraControls3D(Basic3DScene ParentScene)
        {
#if EDITOR && WINDOWS
            this.TopRightCorner = new Vector2(WorldViewer.self.Size.X, 0);
            this.ParentScene = ParentScene;
#endif
        }
        

#if EDITOR && WINDOWS
        public override void SetPositionFromScreen(Vector2 ScreenSize)
        {
            TopRightCorner = new Vector2(ScreenSize.X, 0);
            CreateForms(TopRightCorner);
            base.SetPositionFromScreen(ScreenSize);
        }

        public override void Create(FormHolder Parent)
        {
            base.Create(Parent);
            CreateForms(TopRightCorner);
        }

        public void CreateForms(Vector2 TopRightCorner)
        {
            foreach (Form form in MyForms)
                Parent.RemoveForm(form);
            MyForms.Clear();

            Load();

            int ButtonMargin = 0;
            int ButtonX = (int)TopRightCorner.X + ButtonMargin;
            int ButtonY = (int)TopRightCorner.Y + ButtonMargin;
            int Margin = 23;
            int ButtonSize = 16;

            ButtonX -= Margin;
            AddForm(new Button(SetRotate, RotateIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetScale, ScaleIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetMoveX, MoveXIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetMoveY, MoveYIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetMoveZ, MoveZIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(Center, CenterIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

        }

        new public void AddForm(Form NewForm)
        {
            MyForms.Add(NewForm);
            Parent.AddForm(NewForm);
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            if (ParentScene.MyCamera.get() != null)
                editorCamera = (Camera3DObject)ParentScene.MyCamera.get();

            if (Updater.Size.X != TopRightCorner.X)
            {
                TopRightCorner.X = Updater.Size.X;
                CreateForms(TopRightCorner);
            }

            else if (CurrentMode != LockMode.None)
            {
                if (MouseManager.mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (CurrentMode == LockMode.Scale)
                        editorCamera.ZoomDistance.add(((MouseManager.MousePosition - MouseLockPosition).X + (MouseManager.MousePosition - MouseLockPosition).Y) * ZoomLock / 1000);
                    if (CurrentMode == LockMode.Rotate)
                        editorCamera.Rotation.add(-new Vector3(MathHelper.ToRadians(MouseManager.MousePosition.Y - MouseLockPosition.Y), MathHelper.ToRadians(MouseManager.MousePosition.X - MouseLockPosition.X), 0) / 2.5f * 25);
                    if (CurrentMode == LockMode.MoveX || CurrentMode == LockMode.MoveY || CurrentMode == LockMode.MoveZ)
                    {
                        Vector3 Direction = Vector3.Zero;
                        if (CurrentMode == LockMode.MoveX)
                            Direction = new Vector3(1, 0, 0);
                        if (CurrentMode == LockMode.MoveY)
                            Direction = new Vector3(0, 1, 0);
                        if (CurrentMode == LockMode.MoveZ)
                            Direction = new Vector3(0, 0, 1);

                        Vector3 CenterScreenPos3D = WorldViewer.self.MyViewport.Project(editorCamera.Position.get(), editorCamera.MyCamera.ProjectionMatrix, editorCamera.MyCamera.ViewMatrix, Matrix.Identity);
                        Vector2 CenterScreenPos = new Vector2(CenterScreenPos3D.X, CenterScreenPos3D.Y);
                        Vector3 ProjectedPosition = Direction + editorCamera.Position.get();
                        Vector3 ScreenPos3D = WorldViewer.self.MyViewport.Project(ProjectedPosition, editorCamera.MyCamera.ProjectionMatrix, editorCamera.MyCamera.ViewMatrix, Matrix.Identity);
                        Vector2 ScreenPos2D = new Vector2(ScreenPos3D.X, ScreenPos3D.Y);
                        Vector2 MouseMult = Vector2.Normalize(ScreenPos2D - CenterScreenPos);
                        Vector2 MouseForce = new Vector2(MouseManager.MousePosition.X - MouseLockPosition.X, MouseManager.MousePosition.Y - MouseLockPosition.Y) * MouseMult / 1000;

                        float Value = (MouseForce.X + MouseForce.Y) * ZoomLock;

                        if (CurrentMode == LockMode.MoveX)
                            editorCamera.Position.addX(Value);
                        if (CurrentMode == LockMode.MoveY)
                            editorCamera.Position.addY(Value);
                        if (CurrentMode == LockMode.MoveZ)
                            editorCamera.Position.addZ(Value);
                    }
                    Mouse.SetPosition((int)MouseLockPosition.X, (int)MouseLockPosition.Y);

                    Updater.NeedsToRedraw = true;
                }
                else
                {
                    CurrentMode = LockMode.None;
                    Game1.self.IsMouseVisible = true;
                }
            }

            base.Update(gameTime, Updater);
        }

        public static void Load()
        {
            if (!Loaded)
            {
                MoveIcon = Game1.content.Load<Texture2D>("Editor/MoveIcon");
                MoveXIcon = Game1.content.Load<Texture2D>("Editor/XIcon");
                MoveYIcon = Game1.content.Load<Texture2D>("Editor/YIcon");
                MoveZIcon = Game1.content.Load<Texture2D>("Editor/ZIcon");
                ScaleIcon = Game1.content.Load<Texture2D>("Editor/ScaleIcon");
                RotateIcon = Game1.content.Load<Texture2D>("Editor/RotateIcon");
                CenterIcon = Game1.content.Load<Texture2D>("Editor/CenterIcon");
                ResetIcon = Game1.content.Load<Texture2D>("Editor/ResetIcon");

                Loaded = true;
            }
        }

        public void SetMoveX(Button button)
        {
            CurrentMode = LockMode.MoveX;
            MouseLock();
        }

        public void SetMoveY(Button button)
        {
            CurrentMode = LockMode.MoveY;
            MouseLock();
        }

        public void SetMoveZ(Button button)
        {
            CurrentMode = LockMode.MoveZ;
            MouseLock();
        }

        public void Center(Button button)
        {
            if (editorCamera != null)
            {
                bool Success = false;
                Vector3 Result= Vector3.Zero;
                Basic3DObject.GetAveragePosition(ParentScene.SelectedGameObjects, ref Result, ref Success);
                if (Success)
                    editorCamera.Position.set(Result);
            }
        }

        public void SetRotate(Button button)
        {
            CurrentMode = LockMode.Rotate;
            MouseLock();
        }
        public void SetScale(Button button)
        {
            CurrentMode = LockMode.Scale;
            MouseLock();
        }
        public void MouseLock()
        {
            if (editorCamera != null)
            {
                MouseLockPosition = MouseManager.MousePosition;
                Game1.self.IsMouseVisible = false;
                ZoomLock = editorCamera.ZoomDistance.get();
                MouseManager.DraggedObject = this;
            }
            else
                CurrentMode = LockMode.None;
        }
#endif
    }
}
