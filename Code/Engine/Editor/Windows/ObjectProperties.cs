#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ObjectProperties:Window
    {
        public static ObjectProperties self;

        public ObjectProperties()
        {
            self = this;
        }

        public override void Draw()
        {
            if (GameManager.GetLevel() != null &&
            GameManager.GetLevel().MyScene != null
            && GameManager.GetLevel().MyScene.ObjectPropertiesHolder != null)
                FormChildren = GameManager.GetLevel().MyScene.ObjectPropertiesHolder.FormChildren;
            else
                FormChildren = null;
            base.Draw();
        }

    }
}
#endif