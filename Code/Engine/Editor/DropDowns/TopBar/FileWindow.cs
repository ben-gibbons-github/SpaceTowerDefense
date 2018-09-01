#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FileWindow : DropDownWindow
    {
        public FileWindow()
        {
            AddDropItem(new Button(New, "New", Font), 0, 0);
            AddDropItem(new Button(Open, "Open", Font), 0, 1);
            AddDropItem(new Button(Save, "Save", Font), 0, 2);
            AddDropItem(new Button(SaveAs, "Save as", Font), 0, 3);
            AddDropItem(new Button(Exit, "Exit", Font), 0, 4);
        }

        public void New(Button button)
        {
            EditorManager.NewEditorLevel();
            Destroy();
        }

        public void Open(Button button)
        {
#if WINDOWS
            DialogManager.Load();
#endif
            Destroy();
        }

        public void SaveAs(Button button)
        {
#if WINDOWS
            DialogManager.SaveAs();
#endif
            Destroy();
        }

        public void Save(Button button)
        {
#if WINDOWS
            DialogManager.Save();
#endif
            Destroy();
        }

        public void Exit(Button button)
        {
            if (DialogManager.SaveAs())
                Game1.self.Exit();
        }
    }
}
#endif