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
    public class BoolForm : ValueForm
    {
        public Button ValueField;
        public bool Value;
        public bool NoValue = false;

        public static bool Loaded = false;
        public static Texture2D CheckedTexture;
        public static Texture2D UnCheckedTexture;

        public BoolForm(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }
        public BoolForm(LinkedList<BasicEffectParameter> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        private void init()
        {
            Load();

            AddForm(ValueField = new Button(Click, NoValue || !Value ? UnCheckedTexture : CheckedTexture, Vector2.Zero));

            GetValueFromReferences();

        }

        private void Click(Button b)
        {
            if (NoValue)
            {
                NoValue = false;
                Value = true;
            }
            else
                Value = !Value;

            ValueField.ButtonTexture = NoValue || !Value ? UnCheckedTexture : CheckedTexture;

            GetValueFromField();
        }

        public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                UnCheckedTexture = AssetManager.Load<Texture2D>("Editor/UnCheckedBox");
                CheckedTexture = AssetManager.Load<Texture2D>("Editor/CheckedBox");
            }
        }

        public override void Create(FormHolder Parent)
        {
            SetSize(ValueField.Size + new Vector2(Font.MeasureString(Name).X, 0));

            base.Create(Parent);
        }

        public override void SetPosition(Vector2 Position)
        {
            ValueField.SetPosition(Position + new Vector2(Font.MeasureString(Name.ToString()).X + Buffer, -2));
            base.SetPosition(Position);
        }

        public override void GetValueFromReferences()
        {
            if (FormType == ValueFormType.Value)
                foreach (BoolValue val in ReferenceValues)
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
                foreach (BoolParameter val in ReferenceParameters)
                {
                    if (val == ReferenceParameters.First.Value)
                    {
                        Name = val.Name;
                        Value = val.get();
                    }
                    else if (val.get() != Value)
                        NoValue = true;
                }
            ValueField.ButtonTexture = NoValue || !Value ? UnCheckedTexture : CheckedTexture;
        }

        public override void GetValueFromField()
        {
            BadRabbit.Carrot.Value.ChangeFromForm = true;
            if (!NoValue)
            {
                if(FormType == ValueFormType.Value)
                foreach (BoolValue val in ReferenceValues)
                    val.set(Value);
                else
                    foreach (BoolParameter val in ReferenceParameters)
                        val.set(Value);
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