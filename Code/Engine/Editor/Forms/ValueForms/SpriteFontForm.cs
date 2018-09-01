#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot.ValueForms
{
    public class SpriteFontForm : ValueForm
    {
        public TextField ValueField;
        public SpriteFont Value = null;
        public string Path = "";
        public bool NoValue = false;

        public SpriteFontForm(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
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
            foreach (SpriteFontValue val in ReferenceValues)
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

            ValueField.SetText(NoValue ? "" : Path);
        }

        public override void GetValueFromField()
        {

            BadRabbit.Carrot.Value.ChangeFromForm = true;
            try
            {
                Path = ValueField.Text;
                Value = AssetManager.LoadUnsafe<SpriteFont>(Path);

                NoValue = false;
                ValueField.TextColor = FormFormat.TextColor;

                foreach (SpriteFontValue val in ReferenceValues)
                    val.set(Value, Path);
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