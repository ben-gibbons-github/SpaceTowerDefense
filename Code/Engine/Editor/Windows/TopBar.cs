#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class TopBar : Window
    {
        public static string Status;
        public List<Button> ModeButtons = new List<Button>();

        public override void Create(Editor ParentEditor)
        {
            {
                Vector2 PlacePosition = Vector2.Zero;
                AddForm(new DropDownButton("File", Render.BasicFont, new FileWindow(), new Vector2(0, 1), PlacePosition));

                PlacePosition.X += FormChildren[FormChildren.Count() - 1].Size.X + 2;
                AddForm(new DropDownButton("Edit", Render.BasicFont, new EditWindow(), new Vector2(0, 1), PlacePosition));

                PlacePosition.X += FormChildren[FormChildren.Count() - 1].Size.X + 2;
                AddForm(new DropDownButton("Object", Render.BasicFont, new ObjectWindow(), new Vector2(0, 1), PlacePosition));

                PlacePosition.X += FormChildren[FormChildren.Count() - 1].Size.X + 2;
                AddForm(new DropDownButton("Run", Render.BasicFont, new LevelWindow(), new Vector2(0, 1), PlacePosition));
            }

            base.Create(ParentEditor);
        }

        public override void DrawChildren()
        {
            if (NetworkManager.NetworkStatus != "")
            {
                StringBuilder sb = new StringBuilder(NetworkManager.NetworkStatus);
                int s = 0;
                for (int i = 0; i < sb.Length; i++)
                {
                    s++;
                    if (s > 110 && sb[i] == ' ')
                    {
                        sb[i] = '\n';
                        s = 0;
                    }   
                }
                    Render.DrawShadowedText(sb.ToString(), new Vector2(16, 48));
            }
            base.DrawChildren();
        }
    }
}
#endif