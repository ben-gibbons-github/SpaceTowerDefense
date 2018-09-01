using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class FormSection : Basic2DObject
    {
        BasicGameForm[] OldChildren;
        Vector2 CenterPosition;
        BasicGameForm CenterForm;

        public BoolValue Visible;
        public FloatValue SizeChange;
        public float SizeMult = 0;


        public override void Create()
        {
            Visible = new BoolValue("Visible");
            SizeChange = new FloatValue("Size Change", 0.01f);
            base.Create();
        }

        public override void UpdateEditor(GameTime gameTime)
        {
            ChangeChildren();
            base.UpdateEditor(gameTime);
        }

        public override void CreateInGame()
        {
            ChangeChildren();
            base.CreateInGame();
        }

        void ChangeChildren()
        {
            if (OldChildren != null)
                foreach (BasicGameForm f in OldChildren)
                    f.SetSection(null);

            int childCount = 0;

            foreach (GameObject f in HierarchyChildren)
                if (f.GetType().IsSubclassOf(typeof(BasicGameForm)))
                {
                    BasicGameForm b = (BasicGameForm)f;
                    b.SetSection(this);
                    childCount++;
                }

            OldChildren = new BasicGameForm[childCount];
            CenterPosition = Vector2.Zero;

            int i = 0;
            foreach (GameObject o in HierarchyChildren)
                if (o.GetType().IsSubclassOf(typeof(BasicGameForm)))
                {
                    BasicGameForm f = (BasicGameForm)o;
                    OldChildren[i++] = f;
                    CenterPosition += f.Position.get();
                }

            CenterPosition /= childCount;

            float BestDistance = 10000;
            foreach (BasicGameForm f in OldChildren)
            {
                float d = Vector2.Distance(f.Position.get(), CenterPosition);
                if (d < BestDistance)
                {
                    BestDistance = d;
                    CenterForm = f;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Visible.get())
            {
                SizeMult += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * SizeChange.get();
                if (SizeMult > 1)
                    SizeMult = 1;
            }
            else
            {
                SizeMult -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * SizeChange.get();
                if (SizeMult < 0)
                    SizeMult = 0;
            }

            base.Update(gameTime);
        }

        public BasicGameForm GetCenterForm()
        {
            return CenterForm;
        }
    }
}
