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
    public class Texture2DForm : ValueForm
    {
        public TextField ValueField;
        public Texture2D Value = null;
        public string Path = "";
        public bool NoValue = false;

        public Texture2DForm(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        public Texture2DForm(LinkedList<BasicEffectParameter> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        private void init()
        {
            ValueField = (TextField)AddForm(new TextField(Vector2.Zero, (int)Font.MeasureString(Path.ToString()).X, Font, Path, GetValueFromField));

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
            if(FormType == ValueFormType.Value)
                foreach (Texture2DValue val in ReferenceValues)
                {
                    if (val == ReferenceValues.First.Value)
                    {
                        Name = val.Name;
                        Value = val.get();
                        Path = val.getPath();
                    }
                    else if (!val.getPath().Equals(Path))
                        NoValue = true;
                }
            else
                foreach (Texture2DParameter val in ReferenceParameters)
                {
                    if (val == ReferenceParameters.First.Value)
                    {
                        Name = val.Name;
                        Value = val.get();
                        Path = val.getPath();
                    }
                    else if (!val.getPath().Equals(Path))
                        NoValue = true;
                }

            ValueField.SetText(NoValue ? "" : Path);
        }

        public override void GetValueFromField()
        {
            BadRabbit.Carrot.Value.ChangeFromForm = true;
            try
            {
                Path = ValueField.Text;

                Value = AssetManager.LoadUnsafe<Texture2D>(GameManager.GetLevel().MyScene.TextureDirectory.get() + Path);

                NoValue = false;
                ValueField.TextColor = FormFormat.TextColor;

                if(FormType == ValueFormType.Value)
                foreach (Texture2DValue val in ReferenceValues)
                    val.set(Value, Path);
                else
                    foreach (Texture2DParameter val in ReferenceParameters)
                        val.set(Path);

            }
            catch (Exception e)
            {
                NoValue = true;
                ValueField.TextColor = Color.Red;
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