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
    public class IntForm : ValueForm
    {
        public TextField ValueField;
        public int Value;
        public bool NoValue = false;

        public IntForm(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        public IntForm(LinkedList<BasicEffectParameter> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        private void init()
        {
            ValueField = (TextField)AddForm(new TextField(Vector2.Zero, (int)Font.MeasureString(Value.ToString()).X, Font, Value.ToString(), GetValueFromField));
            ValueField.NumbersOnly = true;
            ValueField.MaxChars = 7;

            GetValueFromReferences();
        }

        public override void Create(FormHolder Parent)
        {
            SetSize(ValueField.Size + new Vector2(Font.MeasureString(Name).X, 0));

            base.Create(Parent);
        }

        public override void SetPosition(Vector2 Position)
        {
            ValueField.SetPosition(Position + new Vector2(Font.MeasureString(Name.ToString()).X + Buffer, 0));
            base.SetPosition(Position);
        }

        public override void GetValueFromReferences()
        {
            if (FormType == ValueFormType.Value)
                foreach (IntValue val in ReferenceValues)
                {
                    if (val == ReferenceValues.First.Value)
                    {
                        Name = val.Name;
                        Value = val.get();
                    }
                    else if (val.get() != Value)
                        NoValue = true;
                }
            else
                foreach (IntParameter val in ReferenceParameters)
                {
                    if (val == ReferenceParameters.First.Value)
                    {
                        Name = val.Name;
                        Value = val.get();
                    }
                    else if (val.get() != Value)
                        NoValue = true;
                }
            ValueField.SetText(NoValue ? "" : Value.ToString());
        }

        public override void GetValueFromField()
        {

            BadRabbit.Carrot.Value.ChangeFromForm = true;
            try
            {
                Value = int.Parse(ValueField.Text);
                NoValue = false;
                if (FormType == ValueFormType.Value)
                    foreach (IntValue val in ReferenceValues)
                        val.set(Value);
                else
                    foreach (IntParameter val in ReferenceParameters)
                        val.set(Value);
            }
            catch (Exception e)
            {
                NoValue = true;
#if DEBUG
                Console.WriteLine(e.Message);
                Exception n = e;
#endif
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