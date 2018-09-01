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
    public class Vector4Form : ValueForm
    {
        public ValueHolder XHolder;
        public ValueHolder YHolder;
        public ValueHolder ZHolder;
        public ValueHolder WHolder;

        public Vector4Form(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        public Vector4Form(LinkedList<BasicEffectParameter> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        private void init()
        {
            AddForm(XHolder = new ValueHolder(this, "- X: ", GetValueFromField));
            AddForm(YHolder = new ValueHolder(this, "- Y: ", GetValueFromField));
            AddForm(ZHolder = new ValueHolder(this, "- Z: ", GetValueFromField));
            AddForm(WHolder = new ValueHolder(this, "- W: ", GetValueFromField));

            GetValueFromReferences();
        }

        public override void Create(FormHolder Parent)
        {
            Vector2 Size = Font.MeasureString(Name) + new Vector2(0, ValueForm.Buffer * 4 + YHolder.ValueField.Size.Y + ZHolder.ValueField.Size.Y + WHolder.ValueField.Size.Y);

            foreach (Form f in FormChildren)
                Size.X = Math.Max(f.Size.X, Size.X);

            SetSize(Size);

            base.Create(Parent);
        }

        public override void SetPosition(Vector2 Position)
        {

            XHolder.SetPosition(Position + new Vector2(Font.MeasureString(Name).X + ValueForm.Buffer, 0));

            YHolder.SetPosition(Position +
   Font.MeasureString(Name) + new Vector2(ValueForm.Buffer)
    );

            ZHolder.SetPosition(Position +
   (Font.MeasureString(Name) + new Vector2(ValueForm.Buffer)) * new Vector2(1, 2)
    );

            WHolder.SetPosition(Position +
  (Font.MeasureString(Name) + new Vector2(ValueForm.Buffer)) * new Vector2(1, 3)
   );


            base.SetPosition(Position);
        }

        public override void GetValueFromReferences()
        {
            if (FormType == ValueFormType.Value)
                foreach (Vector4Value val in ReferenceValues)
                {
                    if (val == ReferenceValues.First.Value)
                    {
                        Name = val.Name;
                        XHolder.set(val.get().X, false);
                        YHolder.set(val.get().Y, false);
                        ZHolder.set(val.get().Z, false);
                        WHolder.set(val.get().Z, false);
                    }
                    else
                    {
                        if (XHolder.Value != val.get().X)
                            XHolder.set(true);
                        if (YHolder.Value != val.get().Y)
                            YHolder.set(true);
                        if (ZHolder.Value != val.get().Z)
                            ZHolder.set(true);
                        if (WHolder.Value != val.get().W)
                            WHolder.set(true);
                    }
                }
            else
                foreach (Vector4Parameter val in ReferenceParameters)
                {
                    if (val == ReferenceParameters.First.Value)
                    {
                        Name = val.Name;
                        XHolder.set(val.get().X, false);
                        YHolder.set(val.get().Y, false);
                        ZHolder.set(val.get().Z, false);
                        WHolder.set(val.get().Z, false);
                    }
                    else
                    {
                        if (XHolder.Value != val.get().X)
                            XHolder.set(true);
                        if (YHolder.Value != val.get().Y)
                            YHolder.set(true);
                        if (ZHolder.Value != val.get().Z)
                            ZHolder.set(true);
                        if (WHolder.Value != val.get().W)
                            WHolder.set(true);
                    }
                }
        }

        public override void GetValueFromField()
        {
            XHolder.get();
            YHolder.get();
            ZHolder.get();
            WHolder.get();

            BadRabbit.Carrot.Value.ChangeFromForm = true;

            if (FormType == ValueFormType.Value)
                foreach (Vector4Value val in ReferenceValues)
                {
                    if (!XHolder.NoValue)
                        val.setX(XHolder.Value);
                    if (!YHolder.NoValue)
                        val.setY(YHolder.Value);
                    if (!ZHolder.NoValue)
                        val.setZ(ZHolder.Value);
                    if (!WHolder.NoValue)
                        val.setW(WHolder.Value);
                }
            else
                foreach (Vector4Parameter val in ReferenceParameters)
                {
                    if (!XHolder.NoValue)
                        val.setX(XHolder.Value);
                    if (!YHolder.NoValue)
                        val.setY(YHolder.Value);
                    if (!ZHolder.NoValue)
                        val.setZ(ZHolder.Value);
                    if (!WHolder.NoValue)
                        val.setW(WHolder.Value);
                }
            BadRabbit.Carrot.Value.ChangeFromForm = false;

            base.GetValueFromField();
        }


        public override void Draw()
        {
            Game1.spriteBatch.DrawString(Font, Name, Position, TextColor);
            base.Draw();
        }

    }
}
#endif