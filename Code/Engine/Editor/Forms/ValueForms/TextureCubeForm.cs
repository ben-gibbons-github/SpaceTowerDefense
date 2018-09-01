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
    public class TextureCubeForm : ValueForm
    {
        public TextField ValueField;
        public TextureCube Value;
        public string Path = "";
        public bool NoValue = false;

        public TextureCubeForm(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
        {
            init();
        }

        public TextureCubeForm(LinkedList<BasicEffectParameter> ReferenceValues)
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
            if (FormType == ValueFormType.Value)
                foreach (TextureCubeValue val in ReferenceValues)
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
                foreach (TextureCubeParameter val in ReferenceParameters)
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
                Value = AssetManager.Load<TextureCube>(Path);
                GameObject ParentGameObject = FormType == ValueFormType.Value ? ReferenceValues.First.Value.Parent : ReferenceParameters.First.Value.ParentValue.Parent; 

                NoValue = false;
                ValueField.TextColor = FormFormat.TextColor;

                if (FormType == ValueFormType.Value)
                    foreach (TextureCubeValue val in ReferenceValues)
                        val.set(Path);
                else
                    foreach (TextureCubeParameter val in ReferenceParameters)
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