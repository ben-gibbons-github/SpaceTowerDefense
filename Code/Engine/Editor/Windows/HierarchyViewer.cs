#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class HierarchyViewer : Window
    {
        public static HierarchyViewer self;

        public HierarchyViewer()
        {
            self = this;
        }

        public override void Draw()
        {
            if (GameManager.GetEditorLevel() != null && GameManager.GetEditorLevel().MyScene != null)
                FormChildren = GameManager.GetEditorLevel().MyScene.HierarchyHolder.FormChildren;
            else
                FormChildren = null;
            base.Draw();
        } 
    }
}
#endif