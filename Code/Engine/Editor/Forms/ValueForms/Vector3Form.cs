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
    public class Vector3Form : ValueForm
    {
        public ValueHolder XHolder;
        public ValueHolder YHolder;
        public ValueHolder ZHolder;
        public EditType editType = EditType.Basic;
        public Vector3 OldValue;

        public Vector3Form(LinkedList<Value> ReferenceValues, EditType editType)
            : base(ReferenceValues)
        {
            init(editType);
        }

        public Vector3Form(LinkedList<BasicEffectParameter> ReferenceValues, EditType editType)
            : base(ReferenceValues)
        {
            init(editType);
        }

        private void init(EditType editType)
        {
            AddForm(XHolder = new ValueHolder(this, "- X: ", GetValueFromField));
            AddForm(YHolder = new ValueHolder(this, "- Y: ", GetValueFromField));
            AddForm(ZHolder = new ValueHolder(this, "- Z: ", GetValueFromField));
            this.editType = editType;

            GetValueFromReferences();
        }

        public override void Create(FormHolder Parent)
        {
            Vector2 Size = Font.MeasureString(Name) + new Vector2(0, ValueForm.Buffer * 4 + YHolder.ValueField.Size.Y + ZHolder.ValueField.Size.Y);

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
   (Font.MeasureString(Name) + new Vector2(ValueForm.Buffer)) * new Vector2(1,2)
    );

            base.SetPosition(Position);
        }

        public override void GetValueFromReferences()
        {
            if (FormType == ValueFormType.Value)
            {
                if (editType == EditType.Basic)
                    foreach (Vector3Value val in ReferenceValues)
                    {
                        if (val == ReferenceValues.First.Value)
                        {
                            Name = val.Name;
                            XHolder.set(val.get().X, false);
                            YHolder.set(val.get().Y, false);
                            ZHolder.set(val.get().Z, false);
                        }
                        else
                        {
                            if (XHolder.Value != val.get().X)
                                XHolder.set(true);
                            if (YHolder.Value != val.get().Y)
                                YHolder.set(true);
                            if (ZHolder.Value != val.get().Z)
                                ZHolder.set(true);
                        }
                    }
                if (editType == EditType.Scalar)
                    foreach (Vector3Value val in ReferenceValues)
                    {
                        if (val == ReferenceValues.First.Value)
                        {
                            Name = val.Name;
                            XHolder.set(val.get().X, false);
                            YHolder.set(val.get().Y, false);
                            ZHolder.set(val.get().Z, false);
                        }
                        else
                        {
                            if (XHolder.Value != val.get().X)
                                XHolder.set(1);
                            if (YHolder.Value != val.get().Y)
                                YHolder.set(1);
                            if (ZHolder.Value != val.get().Z)
                                ZHolder.set(1);
                        }
                        OldValue = new Vector3(XHolder.Value, YHolder.Value, ZHolder.Value);
                    }
                if (editType == EditType.Average)
                {
                    OldValue = Vector3.Zero;
                    Name = ReferenceValues.First.Value.Name;

                    foreach (Vector3Value val in ReferenceValues)
                        OldValue += val.get();

                    OldValue /= ReferenceValues.Count;
                    XHolder.set(OldValue.X, false);
                    YHolder.set(OldValue.Y, false);
                    ZHolder.set(OldValue.Z, false);
                }
            }
            else
                foreach (Vector3Parameter val in ReferenceParameters)
                {
                    if (val == ReferenceParameters.First.Value)
                    {
                        Name = val.Name;
                        XHolder.set(val.get().X, false);
                        YHolder.set(val.get().Y, false);
                        ZHolder.set(val.get().Z, false);
                    }
                    else
                    {
                        if (XHolder.Value != val.get().X)
                            XHolder.set(true);
                        if (YHolder.Value != val.get().Y)
                            YHolder.set(true);
                        if (ZHolder.Value != val.get().Z)
                            ZHolder.set(true);
                    }
                }
        }

        public override void GetValueFromField()
        {
            BadRabbit.Carrot.Value.ChangeFromForm = true;

            if (FormType == ValueFormType.Value)
            {
                if (editType == EditType.Basic)
                {
                    XHolder.get();
                    YHolder.get();
                    ZHolder.get();

                    foreach (Vector3Value val in ReferenceValues)
                    {
                        if (!XHolder.NoValue)
                            val.setX(XHolder.Value);
                        if (!YHolder.NoValue)
                            val.setY(YHolder.Value);
                        if (!ZHolder.NoValue)
                            val.setZ(ZHolder.Value);
                    }
                }

                if (editType == EditType.Scalar)
                {
                    XHolder.get();
                    YHolder.get();
                    ZHolder.get();

                    Vector3 NewValue = new Vector3(XHolder.Value, YHolder.Value, ZHolder.Value);

                    foreach (Vector3Value val in ReferenceValues)
                    {
                        if (NewValue.X != 0)
                            val.multX(NewValue.X / OldValue.X);
                        if (NewValue.Y != 0)
                            val.multY(NewValue.Y / OldValue.Y);
                        if (NewValue.Z != 0)
                            val.multZ(NewValue.Z / OldValue.Z);
                    }
                    Value.ChangeFromForm = false;

                    if (NewValue.X != 0)
                        OldValue.X = NewValue.X;
                    if (NewValue.Y != 0)
                        OldValue.Y = NewValue.Y;
                    if (NewValue.Z != 0)
                        OldValue.Z = NewValue.Z;
                }

                if (editType == EditType.Average)
                {
                    XHolder.get();
                    YHolder.get();
                    ZHolder.get();
                    Vector3 NewValue = new Vector3(XHolder.Value, YHolder.Value, ZHolder.Value);

                    Value.ChangeFromForm = true;
                    foreach (Vector3Value val in ReferenceValues)
                        val.add(NewValue - OldValue);
                    Value.ChangeFromForm = false;

                    OldValue = NewValue;
                }

            }
            else
            {
                XHolder.get();
                YHolder.get();
                ZHolder.get();

                foreach (Vector3Parameter val in ReferenceParameters)
                {
                    if (!XHolder.NoValue)
                        val.setX(XHolder.Value);
                    if (!YHolder.NoValue)
                        val.setY(YHolder.Value);
                    if (!ZHolder.NoValue)
                        val.setZ(ZHolder.Value);
                }
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