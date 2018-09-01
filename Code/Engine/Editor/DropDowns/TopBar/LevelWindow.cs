#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class LevelWindow : DropDownWindow
    {

        public LevelWindow()
        {
            AddDropItem(new Button(Run, "Run", Font), 0, 0);
            AddDropItem(new Button(Run, "Run From Scene",Font), 0, 1);
            AddDropItem(new Button(SendToXbox, "Send to Xbox", Font), 0, 2);
        }

        public void Run(Button button)
        {
            Destroy();
            EditorManager.SwitchToPlay();
        }

        public void SendToXbox(Button button)
        {
            Destroy();
            SenderManager.Create();
        }
    }
}
#endif