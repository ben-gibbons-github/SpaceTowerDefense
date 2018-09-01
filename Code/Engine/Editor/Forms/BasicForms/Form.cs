
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace BadRabbit.Carrot
{
    public class Form
#if EDITOR && WINDOWS
       : FormHolder
#endif
    {
#if EDITOR && WINDOWS
        public FormHolder Parent;

        public Rectangle MyRectangle;
        public Rectangle MyRectangleBorder;
        public Vector2 Size;
        public bool Active = true;
        public bool ContainsMouse = false;
        public bool WriteAble = true;
        public Vector2 Offset = new Vector2();
        public Vector2 Position;

        public bool StackRight = false;

        public virtual void Create(FormHolder Parent)
        {
            this.Parent = Parent;
        }

        public virtual void Remove()
        {

        }

        public virtual void SetActive(bool Active)
        {
            this.Active = Active;
        }

        public virtual void SetWriteable(bool WriteAble)
        {
            this.WriteAble = WriteAble;
            foreach (Form Child in FormChildren)
                Child.SetWriteable(WriteAble);
        }

        public virtual void SetSize(Vector2 Size)
        {
            this.Size = Size;
            UpdateRectangles();
        }

        public virtual void SetPosition(Vector2 Position)
        {
            this.Position = Position;
            UpdateRectangles();
        }

        public virtual void SetPositionFromScreen(Vector2 ScreenSize)
        {
            UpdateRectangles();
        }

        public virtual void UpdateRectangles()
        {
            MyRectangle.X = (int)Position.X;
            MyRectangle.Y = (int)Position.Y;
            MyRectangle.Width = (int)Size.X;
            MyRectangle.Height = (int)Size.Y;

            MyRectangleBorder.X = (int)Position.X - 1;
            MyRectangleBorder.Y = (int)Position.Y - 1;
            MyRectangleBorder.Width = (int)Size.X + 2;
            MyRectangleBorder.Height = (int)Size.Y + 2;
        }

        public virtual void Update(GameTime gameTime, Window Updater)
        {
            if (Updater.ContainsMouse && !Updater.ScrollBarHorizontal.ContainsMouse && !Updater.ScrollBarVertical.ContainsMouse)
            {
                if (ContainsMouse = MyRectangle.Contains(Updater.RelativeMousePoint))
                    Updater.MouseOverForm = true;
            }
            else
                ContainsMouse = false;

            foreach (Form Child in FormChildren)
                Child.Update(gameTime, Updater);
        }

        public Vector2 GetScrollPosition()
        {
            return Position + Offset;
        }

        public virtual Rectangle GetBounds()
        {
            int left = MyRectangle.Left;
            int right = MyRectangle.Right;
            int bottom = MyRectangle.Bottom;
            int top = MyRectangle.Top;
            for (int i = 0; i < FormChildren.Count; i++)
            {
                Rectangle childRect = FormChildren[i].GetBounds();
                if (childRect.Left < left)
                    left = childRect.Left;
                if (childRect.Right > right)
                    right = childRect.Right;
                if (childRect.Top < top)
                    top = childRect.Top;
                if (childRect.Bottom > bottom)
                    bottom = childRect.Bottom;
            }
            return new Rectangle(left, top, right - left, bottom - top);
        }

        public virtual void Draw()
        {
            foreach (Form Child in FormChildren)
                Child.Draw();
        }

        public virtual void DrawAdditive()
        {
            foreach (Form Child in FormChildren)
                Child.DrawAdditive();
        }

        public virtual void Read(BinaryReader Reader)
        {
            foreach (Form Child in FormChildren)
                Child.Read(Reader);
        }

        public virtual void Write(BinaryWriter Writer)
        {
            foreach (Form Child in FormChildren)
                Child.Write(Writer);

        }

        public virtual void Read(Queue<object> ReadQueue)
        {
            foreach (Form Child in FormChildren)
                Child.Read(ReadQueue);
        }

        public virtual void Write(Queue<object> WriteQueue)
        {
            foreach (Form Child in FormChildren)
                Child.Write(WriteQueue);

        }
#endif
    }
}
