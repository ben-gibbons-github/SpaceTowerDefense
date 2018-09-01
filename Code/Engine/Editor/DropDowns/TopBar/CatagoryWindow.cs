#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CatagoryWindow : DropDownWindow
    {
        string MyCatagory;
        public CatagoryWindow(string MyCatagory, ClickEvent AddObject)
        {
            this.MyCatagory = MyCatagory;
            int YPos = 0;

            foreach (CreatorBasic Creator in CreatorBasic.AllCreators)
                if ((Creator.Createable && Creator.Catagory != null) && Creator.Catagory.Equals(MyCatagory))
                {
                    Button b = (Button)AddDropItem(new Button(AddObject, Creator.MyType.Name, Font), 0, YPos++);
                    b.Tag = Creator;
                }
        }
    }
}
#endif