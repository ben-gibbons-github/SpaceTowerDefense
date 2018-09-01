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
    public class Vector2Form : ValueForm
    {
        public ValueHolder XHolder;
        public ValueHolder YHolder;
        public EditType editType = EditType.Basic;
        public Vector2 OldValue;

        public Vector2Form(LinkedList<Value> ReferenceValues, EditType editType)
            : base(ReferenceValues)
        {
            init(editType);
        }

        public Vector2Form(LinkedList<BasicEffectParameter> ReferenceValues, EditType editType)
            : base(ReferenceValues)
        {
            init(editType);
        }

        private void init(EditType editType)
        {
            AddForm(XHolder = new ValueHolder(this, "- X: ", GetValueFromField));
            AddForm(YHolder = new ValueHolder(this, "- Y: ", GetValueFromField));
            this.editType = editType;

            GetValueFromReferences();
        }

        public override void Create(FormHolder Parent)
        {
            Vector2 Size = Font.MeasureString(Name) + new Vector2(0, ValueForm.Buffer * 3 + YHolder.ValueField.Size.Y);

            foreach (Form f in FormChildren)
                Size.X = Math.Max(f.Size.X, Size.X);

            SetSize(Size);

            base.Create(Parent);
        }

        public override void SetPosition(Vector2 Position)
        {
            YHolder.SetPosition(Position + 
               Font.MeasureString(Name) + new Vector2(ValueForm.Buffer)
                );
            XHolder.SetPosition(Position + new Vector2(Font.MeasureString(Name).X + ValueForm.Buffer, 0));

            base.SetPosition(Position);
        }

        public override void GetValueFromReferences()
        {
            if (FormType == ValueFormType.Value)
            {
                if (editType == EditType.Basic || editType == EditType.Scalar)
                    foreach (Vector2Value val in ReferenceValues)
                    {
                        if (val == ReferenceValues.First.Value)
                        {
                            Name = val.Name;
                            XHolder.set(val.get().X, false);
                            YHolder.set(val.get().Y, false);
                        }
                        else
                        {
                            if (editType == EditType.Basic)
                            {
                                if (XHolder.Value != val.get().X)
                                    XHolder.set(true);
                                if (YHolder.Value != val.get().Y)
                                    YHolder.set(true);
                            }
                            else
                            {
                                if (XHolder.Value != val.get().X)
                                    XHolder.set(1);
                                if (YHolder.Value != val.get().Y)
                                    YHolder.set(1);
                            }
                        }
                        if (editType == EditType.Scalar)
                            OldValue = new Vector2(XHolder.Value, YHolder.Value);
                    }
                else
                {
                    OldValue = Vector2.Zero;
                    Name = ReferenceValues.First.Value.Name;

                    foreach (Vector2Value val in ReferenceValues)
                        OldValue += val.get();

                    OldValue /= ReferenceValues.Count;
                    XHolder.set(OldValue.X, false);
                    YHolder.set(OldValue.Y, false);
                }
            }
            else
                foreach (Vector2Parameter val in ReferenceParameters)
                {
                    if (val == ReferenceParameters.First.Value)
                    {
                        Name = val.Name;
                        XHolder.set(val.get().X, false);
                        YHolder.set(val.get().Y, false);
                    }
                    else
                    {
                        if (XHolder.Value != val.get().X)
                            XHolder.set(true);
                        if (YHolder.Value != val.get().Y)
                            YHolder.set(true);
                    }
                }
        }

        public override void GetValueFromField()
        {
            BadRabbit.Carrot.Value.ChangeFromForm = true;

            XHolder.get();
            YHolder.get();

            if (FormType == ValueFormType.Value)
            {

                if (editType == EditType.Basic)
                {
                    foreach (Vector2Value val in ReferenceValues)
                    {
                        if (!XHolder.NoValue)
                            val.setX(XHolder.Value);
                        if (!YHolder.NoValue)
                            val.setY(YHolder.Value);
                    }
                }
                else if (editType == EditType.Scalar)
                {
                    Vector2 NewValue = new Vector2(XHolder.Value, YHolder.Value);

                    foreach (Vector2Value val in ReferenceValues)
                    {
                        if (NewValue.X != 0)
                            val.multX(NewValue.X / OldValue.X);
                        if (NewValue.Y != 0)
                            val.multY(NewValue.Y / OldValue.Y);
                    }
                    Value.ChangeFromForm = false;

                    if (NewValue.X != 0)
                        OldValue.X = NewValue.X;
                    if (NewValue.Y != 0)
                        OldValue.Y = NewValue.Y;
                }
                else if (editType == EditType.Average)
                {
                    Vector2 NewValue = new Vector2(XHolder.Value, YHolder.Value);

                    Value.ChangeFromForm = true;
                    foreach (Vector2Value val in ReferenceValues)
                        val.add(NewValue - OldValue);
                    Value.ChangeFromForm = false;

                    OldValue = NewValue;
                }
            }
            else
                foreach (Vector2Parameter val in ReferenceParameters)
                {
                    if (!XHolder.NoValue)
                        val.setX(XHolder.Value);
                    if (!YHolder.NoValue)
                        val.setY(YHolder.Value);
                }
            BadRabbit.Carrot.Value.ChangeFromForm = false;

            base.GetValueFromField();
        }


        public override void Draw()
        {
            //XHolder.DrawSphere();
            //YHolder.DrawSphere();

            Game1.spriteBatch.DrawString(Font, Name, Position, TextColor);
            base.Draw();
        }

    }
}
#endif