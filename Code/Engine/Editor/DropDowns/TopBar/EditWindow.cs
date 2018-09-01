#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class EditWindow : DropDownWindow
    {
        public EditWindow()
        {
            AddDropItem(new Button(New, "Undo", Font), 0, 0);
            AddDropItem(new Button(New, "Redo", Font), 0, 1);
            AddDropItem(new Button(New, "Cut", Font), 0, 2);
            AddDropItem(new Button(New, "Copy", Font), 0, 3);
            AddDropItem(new Button(New, "Paste", Font), 0, 4);
            AddDropItem(new Button(New, "Settings", Font), 0, 5);
        }

        public void New(Button button)
        {

        }
    }
}
#endif