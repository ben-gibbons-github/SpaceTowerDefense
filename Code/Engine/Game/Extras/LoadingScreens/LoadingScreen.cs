using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class LoadingScreen
    {
        public virtual void Draw(int Completed, int Max, float Alpha)
        {
            Game1.spriteBatch.Begin();
            Render.DrawShadowedText(Completed.ToString() + '/' + Max.ToString(), new Vector2(100), Color.White * Alpha);
            Game1.spriteBatch.End();
        }
    }
}
