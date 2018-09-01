
#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class MouseManager
    {
        private static SpriteFont Font = Render.BasicFont;
        public static Point MousePoint = new Point();
        public static MouseState mouseState;
        public static MouseState mouseStatePrevious;
        public static Vector2 MousePosition;
        public static Vector2 MouseMovement;
        public static Vector2 PreviousMousePosition;
        public static Vector2 PreviousMouseMovement;
        public static bool MouseClicked;
        public static bool RMouseClicked;
        private static bool NeedsReset = false;

#if EDITOR
        public static object DraggedObject;
        public static object PreviousDraggedObject;
        public static Vector2 DraggedMousePosition;
        public static float DraggedMouseAlpha = 0;

        public static void SetDraggedObject(GameObject o)
        {
            DraggedObject = o;
            DraggedMousePosition = MousePosition;
            DraggedMouseAlpha = 0;
        }

        public static void Draw()
        {
            if (DraggedObject == null || !DraggedObject.GetType().IsSubclassOf(typeof(GameObject)))
                return;

            DraggedMouseAlpha = Math.Max(DraggedMouseAlpha, Math.Min(1, (Vector2.Distance(MousePosition, DraggedMousePosition)) / 64));
            GameObject o = (GameObject)DraggedObject;
            Vector2 Size = Font.MeasureString(o.Name.get());

            Size += Vector2.One * 6;
            //Render.DrawSolidRect(MousePosition - Size / 2, MousePosition + Size / 2, Color.DarkGray * Alpha); ;
            Game1.spriteBatch.DrawString(Font, o.Name.get(), MousePosition - Size / 2 + Vector2.One * 3, Color.White * DraggedMouseAlpha);
            Render.DrawOutlineRect(MousePosition - Size / 2, MousePosition + Size / 2, 1, Color.Black * DraggedMouseAlpha);
        }
#endif

        public static void Update(GameTime gameTime)
        {
            mouseStatePrevious = mouseState;
            mouseState = Mouse.GetState();
            MousePosition = new Vector2(mouseState.X, mouseState.Y);
            PreviousMousePosition = new Vector2(mouseStatePrevious.X,mouseStatePrevious.Y);

            MouseClicked = mouseState.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton != ButtonState.Pressed;
            RMouseClicked = mouseState.RightButton == ButtonState.Pressed && mouseStatePrevious.RightButton != ButtonState.Pressed;

            MousePoint.X = mouseState.X;
            MousePoint.Y = mouseState.Y;

            if (NeedsReset)
            {
                Mouse.SetPosition(MasterManager.FullScreenViewport.Width / 2, MasterManager.FullScreenViewport.Height / 2);
                NeedsReset = false;
                MouseMovement = MousePosition - new Vector2(MasterManager.FullScreenViewport.Width / 2, MasterManager.FullScreenViewport.Height / 2);
            }
            else
                MouseMovement = MousePosition - PreviousMousePosition;
#if EDITOR
            PreviousDraggedObject = DraggedObject;

            if (mouseState.LeftButton == ButtonState.Released)
                DraggedObject = null;
#endif
        }

        public static void ResetMouse()
        {
            NeedsReset = true;
        }
    }
}
#endif