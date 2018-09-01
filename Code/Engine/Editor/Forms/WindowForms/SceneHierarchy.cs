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
    public class SceneHierarchy : Form
    {
        public Level ParentLevel;
        public LinkedList<SceneObject> SceneChildren = new LinkedList<SceneObject>();

        public int ItemSize = 64;
        public int ItemSeperation = 32;

        private SpriteFont Font = FormFormat.NormalFont;

        public SceneHierarchy(Vector2 Position, Level ParentLevel)
        {
            this.ParentLevel = ParentLevel;
            SetPosition(Position);
        }

        public override void Update(GameTime gameTime, Window Updater)
        {

            if (this.ContainsMouse)
            {
                SceneObject s = ReturnMouseOver(Updater);
                if(s!=null)
                {
                    if (MouseManager.MouseClicked)
                    {
                        s.LeftClick(gameTime);
                        MouseManager.SetDraggedObject(s);
                    }
                    if (MouseManager.RMouseClicked)
                        s.RightClick(gameTime);
                    
                }
            }
            base.Update(gameTime, Updater);
        }

        public SceneObject ReturnMouseOver(Window Updater)
        {
            foreach (SceneObject Child in SceneChildren)
            {
                SceneObject s = Child.ReturnMouseOverThumbnail(Updater);
                if (s != null)
                    return s;
            }
            return null;
        }

        public void Add(SceneObject Child)
        {
            if (Child.ParentSceneHierarchy != null)
                Child.ParentSceneHierarchy.Remove(Child);
            if (!SceneChildren.Contains(Child))
                SceneChildren.AddFirst(Child);

            Child.ParentSceneHierarchy = this;
            ModifyCollection();
        }

        public void Remove(SceneObject Child)
        {
            if (SceneChildren.Contains(Child))
                SceneChildren.Remove(Child);

            ModifyCollection();
        }

        private void ModifyCollection()
        {
            Vector2 DrawPosition = Position;
            foreach (SceneObject Child in SceneChildren)
                DrawPosition = Child.UpdateThumbnail(DrawPosition);

            SetSize(new Vector2(DrawPosition.X + ItemSeperation + ItemSize, 80));
        }

        public override void Draw()
        {
            foreach (SceneObject Child in SceneChildren)
                Child.DrawThumbnail();

            base.Draw();
        }
    }
}
#endif