#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SceneSelect : Window
    {
        public static SceneSelect self;

        public SceneSelect()
        {
            self = this;
        }

        public override void Draw()
        {
            FormChildren = GameManager.GetEditorLevel().SceneHierarchyHolder.FormChildren;
            base.Draw();
        }
        
    }
}
#endif