using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class CameraControls2D : Form
    {
#if EDITOR && WINDOWS
        public Vector2 MouseLockPosition;
        public float ZoomLock = 1000;
        public LockMode CurrentMode = LockMode.None;
        private Camera2DObject editorCamera;

        public int Counter = 0;

        public Vector2 TopRightCorner;
        public List<Form> MyForms = new List<Form>();

        public Basic2DScene ParentScene;
#endif

        public CameraControls2D(Basic2DScene ParentScene)
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

            CameraControls3D.Load();

            int ButtonMargin = 0;
            int ButtonX = (int)TopRightCorner.X + ButtonMargin;
            int ButtonY = (int)TopRightCorner.Y + ButtonMargin;
            int Margin = 23;
            int ButtonSize = 16;

            ButtonX -= Margin;
            AddForm(new Button(SetRotate, CameraControls3D.RotateIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetScale, CameraControls3D.ScaleIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetMove, CameraControls3D.MoveIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(Center, CameraControls3D.CenterIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));
        
            ButtonX -= Margin;
            AddForm(new Button(Reset, CameraControls3D.ResetIcon, new Vector2(ButtonSize), new Vector2(ButtonX,ButtonY)));
        }

        new public void AddForm(Form NewForm)
        {
            MyForms.Add(NewForm);
            Parent.AddForm(NewForm);
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            if (ParentScene.MyCamera.get() != null)
                editorCamera = (Camera2DObject)ParentScene.MyCamera.get();

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
                        editorCamera.Rotation.add(MathHelper.ToRadians(MouseManager.MousePosition.Y - MouseLockPosition.Y));
                    if (CurrentMode == LockMode.MoveAll)
                        editorCamera.Position.add((MouseLockPosition - MouseManager.MousePosition) / editorCamera.MyCamera.getZoom());

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

        public void SetMove(Button button)
        {
            CurrentMode = LockMode.MoveAll;
            MouseLock();
        }

        public void SetScale(Button button)
        {
            CurrentMode = LockMode.Scale;
            MouseLock();
        }

        public void SetRotate(Button button)
        {
            CurrentMode = LockMode.Rotate;
            MouseLock();
        }

        public void Center(Button button)
        {
            if (editorCamera != null && ParentScene.SelectedGameObjects.Count > 0)
                editorCamera.Position.set(Basic2DObject.GetAveragePosition(ParentScene.SelectedGameObjects));
        }

        public void Reset(Button button)
        {
            if (editorCamera != null)
            {
                editorCamera.Rotation.set(0);
                editorCamera.ZoomDistance.set(1);
            }
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
