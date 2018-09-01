#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public class Window : FormHolder
    {
        public Editor ParentEditor;

        public Rectangle MyRectangle;
        public Rectangle InnerRectangle;
        public Rectangle MyRectangleBorder;
        public Rectangle InnerRectangleBorder;

        public Color OuterColor = FormFormat.WindowOuterColor;
        public Color InnerColor = FormFormat.WindowInnerColor;
        public Color BorderColor = FormFormat.WindowBorderColor;
        public Color TextColor = FormFormat.TextColor;

        public Vector2 Position;
        public Vector2 Size;
        public Vector2 DefaultSize;
        public Vector2 UpperLeftMargin;
        public Vector2 LowerRightMargin;
        public Vector2 OffsetPosition;

        public int BorderSize = 1;

        public Texture2D OuterWindowTexture;
        public Texture2D InnerWindowTexture;

        
        public List<Direction> Anchors = new List<Direction>();

        public RenderTarget2D MyRenderTarget;

        public bool ContainsMouse = false;
        public bool MouseOverForm = false;
        public bool NeedsToRedraw = false;
        public bool DestroyFlag = false;

        public Vector2 RelativeMousePosition;
        public Point RelativeMousePoint;

        public ScrollBar ScrollBarHorizontal;
        public ScrollBar ScrollBarVertical;
        public bool HasScrollbar = true;

        public virtual void Create(Editor ParentEditor)
        {
            this.ParentEditor = ParentEditor;

            if (OuterWindowTexture == null)
                OuterWindowTexture = Render.BlankTexture;
            if (InnerWindowTexture == null)
                InnerWindowTexture = Render.BlankTexture;
            ScrollBarHorizontal = new ScrollBar(this, Orientation.Horizontal);
            ScrollBarVertical = new ScrollBar(this, Orientation.Vertical);
            UpdateRectangles();
        }

        public virtual void Destroy()
        {
            if (ParentEditor != null)
                ParentEditor.RemoveWindow(this);
        }


        public virtual void OnDestroy()
        {

        }

        public virtual void AddAnchors(params Direction[] NewAnchors)
        {
            foreach (Direction Anchor in NewAnchors)
                Anchors.Add(Anchor);
        }

        public virtual void SetDefaultSize(Vector2 Size)
        {
            this.DefaultSize = Size;
            this.Size = Size;
            UpdateRectangles();
        }

        public virtual void SetMargins(Vector2 UpperLeftMargin, Vector2 LowerRightMargin)
        {
            this.LowerRightMargin = LowerRightMargin;
            this.UpperLeftMargin = UpperLeftMargin;
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

        public virtual Rectangle GetBounds()
        {
            int left = 0;
            int right = 0;
            int bottom = 0;
            int top = 0;
            if (FormChildren != null)
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

        public virtual void UpdateRectangles()
        {
            OffsetPosition = Position + new Vector2(BorderSize);

            MyRectangle.X = (int)Position.X;
            MyRectangle.Y = (int)Position.Y;
            MyRectangleBorder.X = (int)Position.X - 1;
            MyRectangleBorder.Y = (int)Position.Y - 1;

            InnerRectangle.X = (int)Position.X + BorderSize;
            InnerRectangle.Y = (int)Position.Y + BorderSize;
            InnerRectangleBorder.X = (int)Position.X + BorderSize - 1;
            InnerRectangleBorder.Y = (int)Position.Y + BorderSize - 1;


            MyRectangle.Width = (int)Size.X;
            MyRectangle.Height = (int)Size.Y;
            MyRectangleBorder.Width = (int)Size.X + 2;
            MyRectangleBorder.Height = (int)Size.Y + 2;

            InnerRectangle.Width = (int)Size.X - BorderSize * 2;
            InnerRectangle.Height = (int)Size.Y - BorderSize * 2;
            InnerRectangleBorder.Width = (int)Size.X - BorderSize * 2 + 2;
            InnerRectangleBorder.Height = (int)Size.Y - BorderSize * 2 + 2;

            if (HasScrollbar)
            {
                if (ScrollBarHorizontal != null)
                    ScrollBarHorizontal.Resize();
                if (ScrollBarVertical != null)
                    ScrollBarVertical.Resize();
            }

            if ((MyRenderTarget == null || (MyRenderTarget.Bounds != InnerRectangle)) && InnerRectangle.Width > 0 && InnerRectangle.Height > 0)
                UpdateRenderTarget();
        }

        public virtual void UpdateRenderTarget()
        {
            DisposeRenderTarget();
            MyRenderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, (int)InnerRectangle.Width, (int)InnerRectangle.Height);
            NeedsToRedraw = true;
        }

        public void DisposeRenderTarget()
        {
            if (MyRenderTarget != null)
            {
                MyRenderTarget.Dispose();
                MyRenderTarget = null;
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (HasScrollbar)
            {
                Rectangle bounds = GetBounds();
                if (bounds.Right > MyRectangle.Width)
                {
                    ScrollBarHorizontal.SetActive(true);
                    ScrollBarHorizontal.pixelToCover = bounds.Right - MyRectangle.Width;
                }
                else
                {
                    ScrollBarHorizontal.SetActive(false);
                    ScrollBarHorizontal.self.X = 0;
                }
                if (bounds.Bottom > MyRectangle.Height)
                {
                    ScrollBarVertical.SetActive(true);
                    ScrollBarVertical.pixelToCover = bounds.Bottom - MyRectangle.Height;
                }
                else
                {
                    ScrollBarVertical.SetActive(false);
                    ScrollBarVertical.self.Y = 0;
                }
            }
            //added scrollbar code
            Vector2 offset = new Vector2((int)(ScrollBarHorizontal.self.X * ScrollBarHorizontal.difference),
                    (int)(ScrollBarVertical.self.Y * ScrollBarVertical.difference));
            RelativeMousePosition = MouseManager.MousePosition - OffsetPosition + offset;
            RelativeMousePoint = new Point((int)RelativeMousePosition.X, (int)RelativeMousePosition.Y);

            if (HasScrollbar)
            {
                if (ScrollBarHorizontal.Active)
                {
                    ScrollBarHorizontal.Update(gameTime, this);
                    ScrollBarHorizontal.Offset = offset;
                }
                if (ScrollBarVertical.Active)
                {
                    ScrollBarVertical.Update(gameTime, this);
                    ScrollBarVertical.Offset = offset;
                }
            }

            MouseOverForm = false;

            if (FormChildren != null)
            {
                for (int i = 0; i < FormChildren.Count(); i++)
                    if (FormChildren[i] != null && FormChildren[i].Active)
                    {
                        FormChildren[i].Update(gameTime, this);
                        FormChildren[i].Offset.X = offset.X;
                        FormChildren[i].Offset.Y = offset.Y;
                    }
            }
        }

        public virtual void PreDraw()
        {

        }

        public virtual void Draw()
        {
            if (MyRenderTarget.IsContentLost)
                return;

            Game1.graphics.GraphicsDevice.SetRenderTarget(MyRenderTarget);
            Game1.graphics.GraphicsDevice.Clear(InnerColor);

            NeedsToRedraw = false;

            DrawWindowContents();

            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);
        }

        public virtual void DrawWindowContents()
        {
            Matrix scrollbarMatrix = Matrix.CreateTranslation(new Vector3(-ScrollBarHorizontal.self.X * ScrollBarHorizontal.difference, -ScrollBarVertical.self.Y * ScrollBarVertical.difference, 0));
            
            Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, scrollbarMatrix);
            DrawChildren();
            Game1.spriteBatch.End();

            Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, scrollbarMatrix);
            DrawAdditiveChildren();
            Game1.spriteBatch.End();
        }

        public virtual void DrawChildren()
        {
            if (FormChildren != null)
                for (int i = 0; i < FormChildren.Count(); i++)
                    if (FormChildren[i] != null && FormChildren[i].Active)
                        FormChildren[i].Draw();
            if (HasScrollbar)
            {
                if (ScrollBarHorizontal.Active)
                    ScrollBarHorizontal.Draw();
                if (ScrollBarVertical.Active)
                    ScrollBarVertical.Draw();
            }
        }

        public virtual void DrawAdditiveChildren()
        {
            if (FormChildren != null)
                for (int i = 0; i < FormChildren.Count(); i++)
                    if (FormChildren[i] != null && FormChildren[i].Active)
                        FormChildren[i].DrawAdditive();
        }

        public void DrawToScreen()
        {
            if (MyRenderTarget == null)
                return;
            Game1.spriteBatch.Draw(MyRenderTarget, Position + new Vector2(BorderSize, BorderSize), Color.White);
        }
    }
}
#endif