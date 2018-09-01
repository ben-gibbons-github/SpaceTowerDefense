#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ObjectWindow : DropDownWindow
    {
        public ObjectWindow()
        {
            int YPos = 0;
            foreach (CreatorBasic Creator in CreatorBasic.AllCreators)
                if (Creator.Createable && (Creator.Catagory == null || Creator.Catagory.Equals("")))
                {
                    Button b = (Button)AddDropItem(new Button(AddObject, Creator.MyType.Name, Font), 0, YPos++);
                    b.Tag = Creator;
                }

            foreach (string Catagory in CreatorBasic.Catagories)
            {
                DropDownButton b =
                   (DropDownButton)AddDropItem(new DropDownButton(Catagory, Font, new CatagoryWindow(Catagory, AddObject), new Vector2(1, 0), Vector2.Zero), 0, YPos++);
                b.ParentWindow = this;
            }

            foreach (Form Child in FormChildren)
                if (Child.GetType().Equals(typeof(DropDownButton)))
                {
                    DropDownButton b = (DropDownButton)Child;
                    b.WindowOffset = new Vector2((Size.X - 10) / Child.Size.X, 0);
                }
        }

        public void AddObject(Button button)
        {
            CreatorBasic c = (CreatorBasic)button.Tag;
            CreatorBasic.LastCreator = c;

            GameObject g = c.ReturnObject();

            GameManager.GetEditorLevel().AddObject(g);

            Destroy();
        }
    }
}
#endif