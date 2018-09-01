#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BadRabbit.Carrot.EffectParameters;

namespace BadRabbit.Carrot.ValueForms
{
    public class ColorForm : ValueForm
    {
        public SliderHolder RHolder;
        public SliderHolder GHolder;
        public SliderHolder BHolder;
        public SliderHolder AHolder;
        public SliderHolder IHolder;

        public Texture2D RectTexture = Render.BlankTexture;

        public Rectangle ColorRect = new Rectangle();
        public Rectangle GlowRect = new Rectangle();

        public static Texture2D GlowCircle;
        public static bool Loaded = false;

        public ColorForm(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        public ColorForm(LinkedList<BasicEffectParameter> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        private void init()
        {
            Load();
            AddForm(RHolder = new SliderHolder(this, "R", GetValueFromField, 0, 1));
            AddForm(GHolder = new SliderHolder(this, "G", GetValueFromField, 0, 1));
            AddForm(BHolder = new SliderHolder(this, "B", GetValueFromField, 0, 1));
            AddForm(AHolder = new SliderHolder(this, "A", GetValueFromField, 0, 1));
            AddForm(IHolder = new SliderHolder(this, "I", GetValueFromField, 0, 4));

            GetValueFromReferences();
        }

        

        public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                GlowCircle = AssetManager.Load<Texture2D>("editor/GlowCircle");
            }
        }

        public override void Create(FormHolder Parent)
        {
            Vector2 Size = Font.MeasureString(Name) + new Vector2(0, ValueForm.Buffer * 6 + RHolder.ValueField.Size.Y * 5);

            foreach (Form f in FormChildren)
                Size.X = Math.Max(f.Size.X, Size.X);

            SetSize(Size);

            base.Create(Parent);
        }

        public override void SetPosition(Vector2 Position)
        {
            ColorRect = new Rectangle(
                (int)(Position.X + Font.MeasureString(Name).X  + ValueForm.Buffer*2),(int)Position.Y,16,16);
            

            Vector2 FormPosition = Position;
            FormPosition.Y += Font.MeasureString(Name).Y;
            FormPosition += new Vector2( ValueForm.Buffer);

            foreach (Form f in FormChildren)
            {
                f.SetPosition(FormPosition);
                FormPosition.Y += ValueForm.Buffer + f.Size.Y;
            }

            base.SetPosition(Position);
        }

        public override void GetValueFromReferences()
        {
            if(FormType== ValueFormType.Value)
            foreach (ColorValue val in ReferenceValues)
            {
                if (val == ReferenceValues.First.Value)
                {
                    Name = val.Name;
                    RHolder.set(val.getBase().X, false);
                    GHolder.set(val.getBase().Y, false);
                    BHolder.set(val.getBase().Z, false);
                    AHolder.set(val.getBase().W, false);
                    IHolder.set(val.getMult(), false);
                }
                else
                {
                    if (RHolder.Value != val.getBase().X)
                        RHolder.set(true);
                    if (GHolder.Value != val.getBase().Y)
                        GHolder.set(true);
                    if (BHolder.Value != val.getBase().Z)
                        BHolder.set(true);
                    if (AHolder.Value != val.getBase().W)
                        AHolder.set(true);
                    if (IHolder.Value != val.getMult())
                        IHolder.set(true);
                }
            }
            else
                foreach (ColorParameter val in ReferenceParameters)
                {
                    if (val == ReferenceParameters.First.Value)
                    {
                        Name = val.Name;
                        RHolder.set(val.getBase().X, false);
                        GHolder.set(val.getBase().Y, false);
                        BHolder.set(val.getBase().Z, false);
                        AHolder.set(val.getBase().W, false);
                        IHolder.set(val.getMult(), false);
                    }
                    else
                    {
                        if (RHolder.Value != val.getBase().X)
                            RHolder.set(true);
                        if (GHolder.Value != val.getBase().Y)
                            GHolder.set(true);
                        if (BHolder.Value != val.getBase().Z)
                            BHolder.set(true);
                        if (AHolder.Value != val.getBase().W)
                            AHolder.set(true);
                        if (IHolder.Value != val.getMult())
                            IHolder.set(true);
                    }
                }
        }

        public override void GetValueFromField()
        {
            RHolder.get();
            GHolder.get();
            BHolder.get();
            AHolder.get();
            IHolder.get();


            BadRabbit.Carrot.Value.ChangeFromForm = true;

            if (FormType == ValueFormType.Value)
                foreach (ColorValue val in ReferenceValues)
                {
                    if (!RHolder.NoValue)
                        val.setR(RHolder.Value);
                    if (!GHolder.NoValue)
                        val.setG(GHolder.Value);
                    if (!BHolder.NoValue)
                        val.setB(BHolder.Value);
                    if (!AHolder.NoValue)
                        val.setA(AHolder.Value);
                    if (!IHolder.NoValue)
                        val.setIntensity(IHolder.Value);
                }
            else
                foreach (ColorParameter val in ReferenceParameters)
                {
                    if (!RHolder.NoValue)
                        val.setX(RHolder.Value);
                    if (!GHolder.NoValue)
                        val.setY(GHolder.Value);
                    if (!BHolder.NoValue)
                        val.setZ(BHolder.Value);
                    if (!AHolder.NoValue)
                        val.setW(AHolder.Value);
                    if (!IHolder.NoValue)
                        val.set(IHolder.Value);
                }

            BadRabbit.Carrot.Value.ChangeFromForm = false;

            base.GetValueFromField();
        }

        public Vector4 GetColor()
        {
            return
                new Vector4(
                    RHolder.Value,
                    GHolder.Value,
                    BHolder.Value,
                    AHolder.Value) * IHolder.Value;
        }

        public override void Draw()
        {
            Game1.spriteBatch.DrawString(Font, Name, Position, TextColor);
            Vector4 Color = GetColor();
            Game1.spriteBatch.Draw(RectTexture, ColorRect, new Color(Color));

            base.Draw();
        }


        public override void DrawAdditive()
        {
            Vector4 Color = GetColor();
            Color.X = Math.Max(0, Color.X - 1);
            Color.Y = Math.Max(0, Color.Y - 1);
            Color.Z = Math.Max(0, Color.Z - 1);

            int GlowSize = 24;
            int RectSize = 16;

            while (Color.X + Color.Y + Color.Z > 0)
            {
                GlowSize += 8;
                int Dif = GlowSize - RectSize;

                GlowRect = new Rectangle(ColorRect.X - Dif / 2, ColorRect.Y - Dif / 2, ColorRect.Width + Dif, ColorRect.Height + Dif);


                Game1.spriteBatch.Draw(GlowCircle, GlowRect, new Color(Color));
                Color.X = Math.Max(0, Color.X - 1);
                Color.Y = Math.Max(0, Color.Y - 1);
                Color.Z = Math.Max(0, Color.Z - 1);
            }

            base.DrawAdditive();
        }

    }
}
#endif