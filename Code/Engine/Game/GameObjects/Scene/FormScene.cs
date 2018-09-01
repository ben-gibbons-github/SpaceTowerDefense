using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FormScene : Basic2DScene
    {
        public LinkedList<GameObject> DrawChildren;
        public LinkedList<GameObject> OverDrawViewsChildren;

        public override void Create()
        {
            AddTag(GameObjectTag.Form);
            DrawChildren = AddTag(GameObjectTag._2DForward);
            OverDrawViewsChildren = AddTag(GameObjectTag.OverDrawViews);
            base.Create();
        }

        public override void PlayerJoinedEvent(PlayerProfile p)
        {
            PlayerMarker m = new PlayerMarker(p);

            foreach (GameObject o in GetList(GameObjectTag.Form))
            {
                BasicGameForm f = (BasicGameForm)o;
                if (f.StartingForm.get())
                {
                    m.SetCurrentForm(f);
                }
            }

            Add(m);
            base.PlayerJoinedEvent(p);
        }

        public override void PlayerQuitEvent(PlayerProfile p)
        {
            GameObject ToDestroy = null;
            foreach(GameObject g in Children)
                if (g.GetType().Equals(typeof(PlayerMarker)) || g.GetType().IsSubclassOf(typeof(PlayerMarker)))
                {
                    PlayerMarker m = (PlayerMarker)g;
                    if (m.MyPlayer == p)
                    {
                        ToDestroy = m;
                        break;
                    }
                }
            if (ToDestroy != null)
                ToDestroy.Destroy();

            base.PlayerQuitEvent(p);
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

            MasterManager.SetViewportToFullscreen();

            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            foreach (GameObject o in OverDrawViewsChildren)
                o.Draw2D(GameObjectTag.OverDrawViews);
            Game1.spriteBatch.End();

        }
    }
}
