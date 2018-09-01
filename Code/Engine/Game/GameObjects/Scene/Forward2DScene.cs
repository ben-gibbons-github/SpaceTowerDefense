using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Forward2DScene : Basic2DScene
    {
        public LinkedList<GameObject> DrawChildren;

        public override void Create()
        {
            DrawChildren = AddTag(GameObjectTag._2DForward);

            base.Create();
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (DrawCamera == null)
                return;
            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, DrawCamera.ViewMatrix);
            Render.AdditiveBlending = false;
            DrawGrid();

            foreach (GameObject g in DrawChildren)
                g.Draw2D(GameObjectTag._2DForward);

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
                objectControls.DrawControls();
#endif
            Game1.spriteBatch.End();

            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, DrawCamera.ViewMatrix);
            Render.AdditiveBlending = true;
            foreach (GameObject g in DrawChildren)
                g.Draw2D(GameObjectTag._2DForward);
            Game1.spriteBatch.End();

            base.Draw2D(DrawTag);
        }


    }
}
