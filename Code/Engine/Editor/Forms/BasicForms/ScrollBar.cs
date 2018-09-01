#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public enum Orientation
    {
        Horizontal,
        Vertical,
    }

    public class ScrollBar : Form
    {
        public bool isBeingDragged = false;
        public Orientation orientation;
        public Window parent;
        public Rectangle self = new Rectangle(0, 0, 16, 16);
        public int size = 16;
        public float difference = 1;
        public int pixelToCover = 400;
        public Point RelativePointOfMouseContact;

        public ScrollBar(Window parent, Orientation orientation)
        {
            this.parent = parent;
            this.orientation = orientation;
            if (orientation == Orientation.Horizontal)
                MyRectangle = new Rectangle(0, parent.MyRectangle.Height - size - parent.BorderSize * 2, parent.MyRectangle.Width - parent.BorderSize * 2 - size, size);
            if (orientation == Orientation.Vertical)
                MyRectangle = new Rectangle(parent.MyRectangle.Width - size - parent.BorderSize * 2, 0, size, parent.MyRectangle.Height - parent.BorderSize * 2 - size);
        }

        public void Resize()
        {
            if (orientation == Orientation.Horizontal)
                MyRectangle = new Rectangle(0, parent.MyRectangle.Height - size - parent.BorderSize * 2, parent.MyRectangle.Width - parent.BorderSize * 2 - size, size);
            if (orientation == Orientation.Vertical)
                MyRectangle = new Rectangle(parent.MyRectangle.Width - size - parent.BorderSize * 2, 0, size, parent.MyRectangle.Height - parent.BorderSize * 2 - size);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Window Updater)
        {
            if (orientation == Orientation.Horizontal)
                pixelToCover = Updater.GetBounds().Width;
            else if (orientation == Orientation.Vertical)
                pixelToCover = Updater.GetBounds().Height;
            Console.WriteLine(pixelToCover);
            //Scrollbar needs real mouse coordinates
            if (Updater.ContainsMouse)
                ContainsMouse = self.Contains(new Point((int)-Offset.X + Updater.RelativeMousePoint.X,
                    (int)-Offset.Y + Updater.RelativeMousePoint.Y));
            else
                ContainsMouse = false;
            //
            if (orientation == Orientation.Horizontal && pixelToCover + size < MyRectangle.Width)
            {
                difference = 1;
                self.Width = MyRectangle.Width - pixelToCover - size;
            }
            else
            {
                self.Width = size;
                difference = pixelToCover / MyRectangle.Width;
            }
            if (orientation == Orientation.Vertical && pixelToCover + size < MyRectangle.Height)
            {
                difference = 1;
                self.Height = MyRectangle.Height - pixelToCover - size;
            }
            else
            {
                self.Height = size;
                difference = pixelToCover / MyRectangle.Height;
            }
            if (ContainsMouse && MouseManager.MouseClicked == true)
            {
                isBeingDragged = true;
                RelativePointOfMouseContact = new Point(Mouse.GetState().X - (int)self.X, Mouse.GetState().Y - (int)self.Y);
                MouseManager.DraggedObject = this;
            }
            else if (MouseManager.mouseState.LeftButton == ButtonState.Released)
            {
                isBeingDragged = false;
            }
            if (isBeingDragged)
            {
                if (orientation == Orientation.Horizontal)
                    self.X = Mouse.GetState().X - RelativePointOfMouseContact.X;
                if (orientation == Orientation.Vertical)
                    self.Y = Mouse.GetState().Y - RelativePointOfMouseContact.Y;

                Updater.NeedsToRedraw = true;
            }
            //Clamp position in bounds
            if (self.Y < MyRectangle.Top)
                self.Y = MyRectangle.Top;
            if (self.Y + self.Height > MyRectangle.Bottom)
                self.Y = MyRectangle.Bottom - self.Height;
            if (self.X < MyRectangle.Left)
                self.X = MyRectangle.Left;
            else if (self.X + self.Width > MyRectangle.Right)
                self.X = MyRectangle.Right - self.Width;
            //base.Update(gameTime, Updater);
        }

        public override void Draw()
        {
            Game1.spriteBatch.Draw(Render.BlankTexture, new Vector2((int)(parent.ScrollBarHorizontal.self.X * parent.ScrollBarHorizontal.difference) + MyRectangle.X,
                (int)(parent.ScrollBarVertical.self.Y * parent.ScrollBarVertical.difference) + MyRectangle.Y), MyRectangle, Color.DarkGray);
            Game1.spriteBatch.Draw(Render.BlankTexture, new Vector2((int)(parent.ScrollBarHorizontal.self.X * parent.ScrollBarHorizontal.difference) + self.X,
                (int)(parent.ScrollBarVertical.self.Y * parent.ScrollBarVertical.difference) + self.Y), self, new Color(90, 90, 90));
            base.Draw();
        }
    }
}
#endif