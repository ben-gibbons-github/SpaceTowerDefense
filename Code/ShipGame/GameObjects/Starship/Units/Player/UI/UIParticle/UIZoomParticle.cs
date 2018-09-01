using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class UIZoomParticle : UIParticleBasic
    {
        public static float ResizeSpeed = 0.05f;

        float Size;
        float EndSize;

        public UIZoomParticle(float EndSize)
        {
            this.EndSize = EndSize;
            Size = 1 - EndSize;
            AlphaFadeOut = 0.05f;
            AlphaFadeIn = 0.05f;
        }

        public override void Update(GameTime gameTime)
        {
            Size += (EndSize * 2 - 1) * ResizeSpeed / 2;

            base.Update(gameTime);
        }

        public override void Draw()
        {
            Render.DrawOutlineRect(ParentShip.sceneView.Size / 2 - ParentShip.sceneView.Size / 2 * Size, ParentShip.sceneView.Size / 2 + ParentShip.sceneView.Size / 2 * Size, 1, Color.White * Alpha);
        }
    }
}
