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
    public class DropDownButton : Button
    {
        public DropDownWindow MyWindow;
        public DropDownWindow ParentWindow;
        public Vector2 WindowOffset;

        public DropDownButton(string Text, SpriteFont Font, DropDownWindow MyWindow, Vector2 WindowOffset, Vector2 Position)
        {
            this.Position = Position;
            this.WindowOffset = WindowOffset;
            SetUpWithText(Text, Font);
            this.MyWindow = MyWindow;
            MyWindow.MyButton = this;
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            if (ContainsMouse && EditorManager.MyEditor.DropDowns.Count > 0)
                MouseClick(Updater);

            base.Update(gameTime, Updater);
        }

        public override void MouseClick(Window Updater)
        {
            if (ParentWindow != null)
                ParentWindow.AddChild(MyWindow);

            EditorManager.MyEditor.AddDropDown(MyWindow);
            MyWindow.SetPosition(Position + WindowOffset * Size + Updater.OffsetPosition);

            Updater.NeedsToRedraw = true;

            base.MouseClick(Updater);
        }

        public override void Draw()
        {
            if (ParentWindow != null)
                BorderColor = BackgroundColor;
            base.Draw();
        }


    }
}
#endif