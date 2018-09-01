using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Starfield : GameObject
    {
        IntValue Seed;
        IntValue StarCount;
        
        FloatValue MinSize;
        FloatValue MaxSize;

        FloatValue MinRadius;
        FloatValue MaxRadius;

        ColorValue MinColor;
        ColorValue MaxColor;

        Star[] Stars;
        Random r;


        public override void  Create()
        {
            AddTag(GameObjectTag.Update);
            
            Seed = new IntValue("Seed");
            Seed.ChangeEvent = StarChange;
            StarCount = new IntValue("Star Count");
            StarCount.ChangeEvent = StarChange;

            MinSize = new FloatValue("Min Size", 500);
            MinSize.ChangeEvent = StarChange;
            MaxSize = new FloatValue("Max Size", 2500);
            MaxSize.ChangeEvent = StarChange;

            MinRadius = new FloatValue("Min Radius", 10000);
            MinRadius.ChangeEvent = StarChange;
            MaxRadius = new FloatValue("Max Radius", 30000);
            MaxRadius.ChangeEvent = StarChange;

            MinColor = new ColorValue("Min Color", Vector4.One);
            MinColor.ChangeEvent = StarChange;
            MaxColor = new ColorValue("Max Color", Vector4.One);
            MaxColor.ChangeEvent = StarChange;

 	        base.Create();
            StarCount.set(100);
        }

        void StarChange()
        {
            r = new Random(Seed.get());
            Stars = new Star[StarCount.get()];
            for (int i = 0; i < StarCount.get(); i++)
                Stars[i] = new Star(Logic.RLerp(MinRadius.get(), MaxRadius.get(), r) * Vector3.Normalize(Rand.V3()), 
                    Logic.RLerp(MinSize.get(), MaxSize.get(), r), new Color(Logic.RLerp(MinColor.get(), MaxColor.get(), r)));
        }

        public override void  Update(GameTime gameTime)
        {
            return;
            for (int i = 0; i < Stars.Length; i++)
            {
                ParticleManager.CreateParticle(Stars[i].Position, Vector3.Zero, Stars[i].color, Stars[i].Size, 1);
            }

 	        base.Update(gameTime);
        }

        class Star
        {
            public Vector3 Position;
            public float Size;
            public Color color;

            public Star(Vector3 Position, float Size, Color color)
            {
                this.Position = Position;
                this.Size = Size;
                this.color = color;
            }
        }
    }
}
