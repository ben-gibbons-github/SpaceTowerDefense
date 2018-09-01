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
    public class DropDownWindow : Window
    {
        public Vector2 ItemSize;
        public SpriteFont Font;
        public List<DropDownWindow> WindowFormChildren = new List<DropDownWindow>();
        public DropDownButton MyButton;
        public DropDownWindow ParentDropDown;

        public Vector2 ItemPush = new Vector2(4);
        public Vector2 ItemCushon = new Vector2(4);

        public DropDownWindow()
        {
            this.BorderSize = 0;
            this.Font = Render.BasicFont;
            this.ItemSize = new Vector2(17);
            this.HasScrollbar = false;
        }

        public override void Create(Editor ParentEditor)
        {
            this.InnerColor = Color.DarkGray;

            foreach (Form form in FormChildren)
                if (form.GetType().Equals(typeof(Button)))
                {
                    Button b = (Button)form;
                    b.BackgroundColor = this.InnerColor;
                    b.BorderColor = this.InnerColor;
                }
            base.Create(ParentEditor);
        }

        public void CheckAlive()
        {
            if (!CheckMouse() && (MyButton == null || !MyButton.ContainsMouse))
                DestroyFlag = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (MouseManager.MouseClicked)
                CheckAlive();

            Form MouseForm = null;

            RelativeMousePosition = MouseManager.MousePosition - OffsetPosition;
            RelativeMousePoint = new Point((int)RelativeMousePosition.X, (int)RelativeMousePosition.Y);


            if (FormChildren != null)
            {
                float BestDistance = 100000;

                if (ContainsMouse)
                {
                    for (int i = 0; i < FormChildren.Count(); i++)
                        if (FormChildren[i] != null && FormChildren[i].Active)
                        {
                            FormChildren[i].ContainsMouse = false;

                            float Dist = Vector2.Distance(FormChildren[i].Position + new Vector2(0, FormChildren[i].Size.Y / 2), RelativeMousePosition);
                            if (Dist < BestDistance)
                            {
                                BestDistance = Dist;
                                MouseForm = FormChildren[i];
                            }
                        }

                    if (MouseForm != null)
                        MouseForm.ContainsMouse = true;
                }
                else
                    for (int i = 0; i < FormChildren.Count(); i++)
                        if (FormChildren[i] != null && FormChildren[i].Active)
                            FormChildren[i].ContainsMouse = false;

                for (int i = 0; i < FormChildren.Count(); i++)
                    if (FormChildren[i] != null && FormChildren[i].Active)
                        FormChildren[i].Update(gameTime, this);


            }
        }

        public DropDownWindow AddChild(DropDownWindow NewChild)
        {
            WindowFormChildren.Add(NewChild);
            NewChild.ParentDropDown = this;
            return NewChild;
        }

        public Form AddDropItem(Form NewForm, int Xslot, int Yslot)
        {
            AddForm(NewForm);
            NewForm.SetPosition(ItemSize * new Vector2(Xslot, Yslot) + ItemPush);

            SetDefaultSize(new Vector2(
                Math.Max(Size.X, Xslot * ItemSize.X + NewForm.Size.X + BorderSize * 2 + ItemCushon.X + ItemPush.X),
                Math.Max(Size.Y, Yslot * ItemSize.Y + NewForm.Size.Y + BorderSize * 2 + ItemCushon.Y + ItemPush.Y)));


            return NewForm;
        }

        public bool CheckMouse()
        {
            if (ContainsMouse)
                return true;
            else
                foreach (DropDownWindow Child in WindowFormChildren)
                    if (Child.CheckMouse())
                        return true;

            return false;
        }
        public override void Destroy()
        {
            if (ParentDropDown != null && ParentDropDown.WindowFormChildren.Contains(this))
                ParentDropDown.WindowFormChildren.Remove(this);

            List<DropDownWindow> Temp = new List<DropDownWindow>();

            foreach (DropDownWindow Child in WindowFormChildren)
                Temp.Add(Child);

            foreach (DropDownWindow Child in Temp)
                Child.Destroy();

            base.Destroy();
        }
    }
}
#endif