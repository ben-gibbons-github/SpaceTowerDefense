using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BasicSprite : Basic2DObject
    {
        public Texture2DValue Sprite;
        public BoolValue Additive;

        public override void Create()
        {
            Sprite = new Texture2DValue("Sprite");
            Additive = new BoolValue("Additive", AdditiveChange);

            AddTag(GameObjectTag._2DForward);
            base.Create();
        }

        private void AdditiveChange()
        {
            if (Additive.get())
            {
                RemoveTag(GameObjectTag._2DForward);
                AddTag(GameObjectTag._2DForward);
            }
            else
            {
                RemoveTag(GameObjectTag._2DForward);
                AddTag(GameObjectTag._2DForward);
            }
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            Render.DrawSprite(Sprite.get(), Position, Size, Rotation);
            base.Draw2D(DrawTag);
        }
    }
}
