#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;


namespace BadRabbit.Carrot
{
    public class Editor
    {
        public List<Window> Children = new List<Window>();
        public List<Window> DropDowns = new List<Window>();
        public int OldWidth;
        public int OldHeight;
        int WindowWidth;
        int WindowHeight;

        private bool OldWindowIsActive = true;

        public Editor()
        {

        }

        public void Create()
        {
            EditorFormat.InitWindows(this);
            EditorResize();
        }

        public void Destroy()
        {
            foreach (Window w in Children)
                w.DisposeRenderTarget();
        }

        public Window AddWindow(Window NewWindow)
        {
            if (!Children.Contains(NewWindow))
            {
                Children.Add(NewWindow);
                NewWindow.Create(this);
            }

            return NewWindow;
        }

        public Window AddDropDown(Window NewWindow)
        {
            for (int i = 0; i < DropDowns.Count(); i++)
            {
                DropDownWindow d = (DropDownWindow)DropDowns[i];
                d.CheckAlive();
            }

            if (!Children.Contains(NewWindow))
            {
                Children.Add(NewWindow);
                NewWindow.Create(this);
                DropDowns.Add(NewWindow);
            }
            
            return NewWindow;
        }

        public void RemoveWindow(Window RemoveWindow)
        {
            RemoveWindow.OnDestroy();
            if (Children.Contains(RemoveWindow))
                Children.Remove(RemoveWindow);
            if (DropDowns.Contains(RemoveWindow))
                DropDowns.Remove(RemoveWindow);
        }


        public void EditorResize()
        {
            OldHeight = WindowHeight;
            OldWidth = WindowWidth;

            foreach (Window Child in Children)
            {
                Vector2 UpperLeft = Child.Position;
                Vector2 LowerRight = Child.Position + Child.Size;

                if (Child.Anchors.Contains(Direction.RIGHT))
                {
                    LowerRight.X = WindowWidth - Child.LowerRightMargin.X;
                    if (Child.Anchors.Contains(Direction.LEFT))
                        UpperLeft.X = Child.UpperLeftMargin.X;
                    else
                        UpperLeft.X = WindowWidth - Child.DefaultSize.X - Child.LowerRightMargin.X;
                }
                else if (Child.Anchors.Contains(Direction.LEFT))
                {
                    UpperLeft.X = Child.UpperLeftMargin.X;
                    LowerRight.X = Child.DefaultSize.X + Child.UpperLeftMargin.X;
                }

                if (Child.Anchors.Contains(Direction.DOWN))
                {
                    LowerRight.Y = WindowHeight - Child.LowerRightMargin.Y;
                    if (Child.Anchors.Contains(Direction.UP))
                        UpperLeft.Y = Child.UpperLeftMargin.Y;
                    else
                        UpperLeft.Y = WindowHeight - Child.DefaultSize.Y - Child.LowerRightMargin.Y;
                }
                else if (Child.Anchors.Contains(Direction.UP))
                {
                    UpperLeft.Y = Child.UpperLeftMargin.Y;
                    LowerRight.Y = Child.DefaultSize.Y + Child.UpperLeftMargin.Y;
                }
                Child.SetPosition(UpperLeft);
                Child.SetSize(LowerRight - UpperLeft);
            }
        }

        public void Update(GameTime gameTime)
        {   
#if WINDOWS
            bool WindowIsActive = Game1.self.IsActive;

            if (WindowIsActive && !this.OldWindowIsActive)
                foreach (Window w in Children)
                    w.NeedsToRedraw = true;

            OldWindowIsActive = WindowIsActive;
#endif

            WindowWidth = Game1.self.Window.ClientBounds.Width;
            WindowHeight = Game1.self.Window.ClientBounds.Height;

            if (WindowWidth > 100 && WindowHeight > 100)
                if (WindowHeight != OldHeight || WindowWidth != OldWidth)
                    EditorResize();

            Window MouseWindow = null;



            for (int i = 0; i < DropDowns.Count(); i++)
                if (MouseWindow == null && DropDowns[i] != null && DropDowns[i].MyRectangle.Contains(MouseManager.MousePoint))
                {
                    MouseWindow = DropDowns[i];
                }

            for (int i = 0; i < Children.Count(); i++)
                if (MouseWindow == null && Children[i] != null && Children[i].MyRectangle.Contains(MouseManager.MousePoint))
                {
                    MouseWindow = Children[i];
                }

            for (int i = 0; i < Children.Count(); i++)
                if (Children[i] != null)
                {
                    if (MouseWindow != Children[i])
                        Children[i].ContainsMouse = false;
                    else
                        Children[i].ContainsMouse = true;
                    Children[i].Update(gameTime);
                }

            GameManager.GetEditorLevel().Update(gameTime);

            Cleanup();
        }

        public virtual void Cleanup()
        {
            for (int i = 0; i < Children.Count(); i++)
                if (Children[i] != null && Children[i].DestroyFlag)
                {
                    Children[i].DestroyFlag = false;
                    Children[i].Destroy();
                }

        }


        public void Draw()
        {
            for (int i = 0; i < Children.Count(); i++)
                if (Children[i] != null && Children[i].MyRenderTarget != null && (Children[i].ContainsMouse || Children[i].NeedsToRedraw))
                    Children[i].PreDraw();


            for (int i = 0; i < Children.Count(); i++)
                if (Children[i] != null && Children[i].MyRenderTarget != null && (Children[i].ContainsMouse || Children[i].NeedsToRedraw))
                    Children[i].Draw();

            Game1.spriteBatch.Begin();

            for (int i = 0; i < Children.Count(); i++)
                if (Children[i] != null)
                    Children[i].DrawToScreen();

            MouseManager.Draw();

            Game1.spriteBatch.End();
        }
        public void Write(BinaryWriter Writer)
        {
            
        }

        public void Read(BinaryReader Reader)
        {
            
        }
    }
}
#endif