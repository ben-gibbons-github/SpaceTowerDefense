#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class Hierarchy : Form, HierarchyParent
    {
        public GameObject HighlightedObject;
        public SceneObject ParentScene;

        public Color TextColor = FormFormat.TextColor;
        public Color SelectedTextColor = FormFormat.SelectedTextColor;
        public Color LineColor = Color.Black;
        public Color BoxColor = Color.DarkGray;
        public Color HighlightedTextColor = FormFormat.HighlightedTextColor;

        public Texture2D LineTexture = Render.BlankTexture;

        public LinkedList<GameObject> Children = new LinkedList<GameObject>();
        public SpriteFont Font;

        public int ItemHeight = 16;
        public int BoxSize = 8;
        public int PlusSize = 6;

        public Vector2 ItemPush = new Vector2(8, 0);
        public Vector2 ItemCushon = new Vector2(16, 0);

        public Rectangle LineRectangle = new Rectangle();
        public Point RelativeMouse;

        public Hierarchy(Vector2 Position, SceneObject ParentScene)
        {
            this.ParentScene = ParentScene;
            LineRectangle.Width = 1;

            Font = Render.BasicFont;
            this.SetPosition(Position);
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            RelativeMouse = Updater.RelativeMousePoint;
            base.Update(gameTime, Updater);
            if (ContainsMouse)
            {
                GameObject MouseObject = ReturnMouseOver(Updater);
                HighlightedObject = MouseObject;

                if (MouseManager.MouseClicked)
                {
                    if (MouseObject != null)
                    {
                        bool ShouldSelect = !MouseObject.HierarchyDragRectangle.Contains(Updater.RelativeMousePoint);

                        if (KeyboardManager.ControlPressed())
                        {
                            if (MouseObject.EditorSelected)
                                ParentScene.RemoveSelected(MouseObject);
                            else
                            {
                                ParentScene.AddSelected(MouseObject);
                                MouseObject.LeftClick(gameTime);
                            }
                        }
                        else if (KeyboardManager.ShiftPressed())
                        {
                            ShiftSelect(MouseObject);
                            MouseObject.LeftClick(gameTime);
                        }
                        else
                        {
                            if (ShouldSelect)
                            {
                                ParentScene.ClearSelected();
                                ParentScene.AddSelected(MouseObject);
                                MouseObject.LeftClick(gameTime);
                            }
                            MouseManager.SetDraggedObject(MouseObject);
                        }

                        WorldViewer.self.NeedsToRedraw = true;
                        ObjectProperties.self.NeedsToRedraw = true;
                    }
                }

                if (MouseManager.DraggedObject == null && MouseManager.PreviousDraggedObject != null && MouseManager.PreviousDraggedObject.GetType().IsSubclassOf(typeof(GameObject)))
                {
                    if (MouseObject != null && Updater.RelativeMousePoint.X < MouseObject.HierarchyBox.X + MouseObject.HierarchyBoxOutline.Width + 2)
                    {
                        MouseObject.HierarchyExpanded = !MouseObject.HierarchyExpanded;
                        MouseObject.ModifyCollection();
                    }
                    else if (MouseObject != null && MouseManager.PreviousDraggedObject != MouseObject)
                    {
                        GameObject g = MouseManager.PreviousDraggedObject as GameObject;
                        if (!g.HierarchyObjectIschild(MouseObject))
                        {
                            MouseObject.HierarchyExpanded = true;
                            MouseObject.AddHierarchyObject(g);
                        }
                    }
                }

                if (MouseManager.mouseState.RightButton == ButtonState.Pressed && MouseManager.mouseStatePrevious.RightButton == ButtonState.Released)
                {
                    if (MouseObject != null)
                    {
                        ParentScene.ClearSelected();
                        ParentScene.AddSelected(MouseObject);
                        MouseObject.RightClick(gameTime);

                        WorldViewer.self.NeedsToRedraw = true;
                        ObjectProperties.self.NeedsToRedraw = true;
                        HierarchyViewer.self.NeedsToRedraw = true;
                    }
                }
            }
            else
                HighlightedObject = null;
        }

        public void ShiftSelect(GameObject MouseObject)
        {
            float SmallestY = MouseObject.HierarchyDrawPosition.Y;
            float LargestY = MouseObject.HierarchyDrawPosition.Y;

            foreach (GameObject o in ParentScene.SelectedGameObjects)
            {
                if (o.EditorSelected)
                {
                    //if (o.HierarchyDrawPosition.Y > LargestY)
                      //  LargestY = o.HierarchyDrawPosition.Y;
                    if (o.HierarchyDrawPosition.Y < SmallestY)
                        SmallestY = o.HierarchyDrawPosition.Y;
                }
            }

            LinkedList<GameObject> toSelect = new LinkedList<GameObject>();
            foreach (GameObject o in Children)
                o.TestContained(SmallestY, LargestY, toSelect);

            ParentScene.ClearSelected();
            ParentScene.AddSelected(toSelect);
        }

        public virtual void Clear()
        {
            Children.Clear();
        }

        public virtual GameObject ReturnMouseOver(Window Updater)
        {
            foreach (GameObject Child in Children)
            {
                GameObject g = Child.ReturnMouseOver(Updater);
                if (g != null)
                    return g;
            }
            return null;
        }

        public LinkedList<GameObject> GetChildren()
        {
            return Children;
        }

        public void AddHierarchyObject(GameObject NewObject)
        {
            NewObject.AddToHierarchy();
            Children.AddFirst(NewObject);
            NewObject.hierarchyParent = this;
            NewObject.TopHierarchyParent = this;
            ModifyCollection();
        }

        public void RemoveHierarchyObject(GameObject KillObject)
        {
            Children.Remove(KillObject);
            ModifyCollection();
        }

        public void ModifyCollection()
        {
            UpdateHierarchy();
            HierarchyViewer.self.NeedsToRedraw = true;
        }

        public void UpdateHierarchy()
        {
            Vector2 DrawPosition = Position + ItemCushon - new Vector2(0, ItemHeight);
            float Width = 0;

            foreach (GameObject Child in Children)
            {
                Vector2 ReturnVect = Child.UpdateHierarchy(DrawPosition + new Vector2(0, ItemHeight));

                LineRectangle.Height = (int)(DrawPosition.Y + ItemHeight * 1.5);

                DrawPosition.Y = ReturnVect.Y;
                Width = Math.Max(Width, ReturnVect.X);
            }

            LineRectangle.X = (int)(Position.X + ItemCushon.X - ItemPush.X);
            LineRectangle.Y = (int)Position.Y;

            SetSize(new Vector2(Width - Position.X, DrawPosition.Y + ItemHeight));
        }

        public override void Draw()
        {
            //  Game1.spriteBatch.DrawSphere(LineTexture, MyRectangleBorder, Color.Black);
            // Game1.spriteBatch.DrawSphere(LineTexture, MyRectangle, Color.Gray);
            Game1.spriteBatch.Draw(LineTexture, LineRectangle, LineColor);


            foreach (GameObject Child in Children)
            {
                Child.DrawAsHierarchy();

            }

            base.Draw();
        }

    }
}

#endif