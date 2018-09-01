using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class ObjectControls2D : Form
    {
        #region Fields
#if !EDITOR && WINDOWS
        public void Draw2D() { }
        public bool RayCast(GameTime gameTime) { return false; } 
#endif
#if EDITOR && WINDOWS

        private static bool Loaded = false;
        private static Texture2D MoveControl;
        private static Texture2D ScaleControl;
        private static Texture2D RotateControl;
        public Button GridButton;

        public Vector2[] ScreenPositions = new Vector2[4];
        public LockMode CurrentMode = LockMode.None;
        public Vector2 MouseLockPosition;
        public ControlMode controlMode = ControlMode.Move;
        Matrix WorldMatrix = Matrix.Identity;
        public List<Form> MyForms = new List<Form>();

        public Rectangle DrawRectangle;
#endif
        public Basic2DScene ParentScene;

        #endregion

        public ObjectControls2D(Basic2DScene ParentScene)
        {
            this.ParentScene = ParentScene;
        }

#if EDITOR && WINDOWS
        public override void Create(FormHolder Parent)
        {
            base.Create(Parent);
            Load();
            CreateForms();
        }

        private static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                ObjectControls3D.LoadIcons();

                ScaleControl = Game1.content.Load<Texture2D>("Editor/ScaleControl");
                RotateControl = Game1.content.Load<Texture2D>("Editor/RotateControl");
                MoveControl = Game1.content.Load<Texture2D>("Editor/MoveControl");
            }
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            if (CurrentMode != LockMode.None)
            {
                if (MouseManager.mouseState.LeftButton == ButtonState.Pressed)
                {
                    Vector2 AMT = MouseManager.MousePosition - MouseLockPosition;
                    switch (controlMode)
                    {
                        case ControlMode.Move:
                            ParentScene.MoveSelected(AMT);
                            break;
                        case ControlMode.Rotate:
                            ParentScene.RotateSelected(AMT.X + AMT.Y, Basic2DObject.GetAveragePosition(ParentScene.SelectedGameObjects));
                            break;
                        case ControlMode.Scale:
                            ParentScene.ScaleSelected(Vector2.One + AMT/300, Basic2DObject.GetAveragePosition(ParentScene.SelectedGameObjects));
                            break;
                    }
                }
                else
                {
                    if (controlMode == ControlMode.Move && ParentScene.UseGrid.get())
                        ParentScene.SnapSelected();
                    Game1.self.IsMouseVisible = true;
                    CurrentMode = LockMode.None;
                }
                Mouse.SetPosition((int)MouseLockPosition.X, (int)MouseLockPosition.Y);
            }

            base.Update(gameTime, Updater);
        }

        public bool RayCast(GameTime gameTime)
        {
            if (MouseManager.MouseClicked && ParentScene.SelectedGameObjects.Count > 0 && ParentScene.MyCamera.get() != null)
            {
                CurrentMode = LockMode.MoveAll;
                MouseLockPosition = MouseManager.MousePosition;
                Game1.self.IsMouseVisible = false;
            }
            return CurrentMode != LockMode.None ? true : false;
        }
        public void CreateForms()
        {
            foreach (Form form in MyForms)
                Parent.RemoveForm(form);
            MyForms.Clear();

            int ButtonX = 0;
            int ButtonY = 0;
            int Margin = -23;
            int ButtonSize = 16;

            AddForm(new Button(SetRotate, ObjectControls3D.RotateIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetScale, ObjectControls3D.ScaleIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetMove, ObjectControls3D.MoveIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));
            SetMove((Button)MyForms[2]);

            ButtonX -= Margin;
            GridButton = (Button)Parent.AddForm(new Button(ClickGrid, ObjectControls3D.GridIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));
        }

        public void SetRotate(Button button)
        {
            controlMode = ControlMode.Rotate;
            Switchselect(button);
        }

        public void SetScale(Button button)
        {
            controlMode = ControlMode.Scale;
            Switchselect(button);
        }

        public void SetMove(Button button)
        {
            controlMode = ControlMode.Move;
            Switchselect(button);
        }

        public void ClickGrid(Button button)
        {
            ParentScene.UseGrid.set(!button.Selected);
        }

        private void Switchselect(Button b)
        {
            foreach (Button but in MyForms)
                but.Selected = false;
            b.Selected = true;
        }

        new public void AddForm(Form NewForm)
        {
            MyForms.Add(NewForm);
            Parent.AddForm(NewForm);
        }

        public void DrawControls()
        {
            if (ParentScene.DrawCamera != null && ParentScene.SelectedGameObjects.Count > 0)
            {
                Vector2 Avg = Basic2DObject.GetAveragePosition(ParentScene.SelectedGameObjects);
                Texture2D tex = controlMode == ControlMode.Move ? MoveControl :
                    controlMode == ControlMode.Rotate ? RotateControl : ScaleControl;
                Render.DrawSprite(tex, Avg, new Vector2(128) / ParentScene.DrawCamera.getZoom(), 0);
            }
            base.Draw();
        }
#endif
    }
}
