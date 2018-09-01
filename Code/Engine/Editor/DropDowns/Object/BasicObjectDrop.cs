#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BasicObjectDrop : DropDownWindow
    {
        public GameObject MyObject;
        public BasicObjectDrop(GameObject MyObject) 
        {
            int i =0;
            foreach(string strng in MyObject.RightClickActions.Keys)
            {
                Button b;
                AddDropItem(b = new Button(MyObject.RightClickActions[strng], strng, Font), 0, i++);
                b.DropWindow = this;
            }

            SetPosition(MouseManager.MousePosition);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //if (MouseManager.MouseClicked)
              //  DestroyFlag = true;
            base.Update(gameTime);
        }
       

    }
}
#endif