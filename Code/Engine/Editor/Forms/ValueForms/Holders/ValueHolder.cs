#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BadRabbit.Carrot.ValueForms;


namespace BadRabbit.Carrot
{
    public class ValueHolder : Form
    {
        public SpriteFont Font = FormFormat.NormalFont;
        public Vector2 TextPosition = Vector2.Zero;
        public Color TextColor = FormFormat.TextColor;
        public EnterEvent Event;

        public string Name;
        public TextField ValueField;
        public bool NoValue = false;
        public float Value;

        public ValueHolder(Form Parent ,string Name, EnterEvent Event)
        {
            this.Name = Name;
            this.Event = Event;

            AddForm(ValueField = new TextField(Position + new Vector2(Font.MeasureString(Name).X, 0), (int)Font.MeasureString(Value.ToString()).X, Font, Value.ToString(), Event));
            ValueField.NumbersOnly = true;
            ValueField.MaxChars = 7;
        }

        public void get()
        {
            if (ValueField.Text.Equals(""))
                NoValue = true;
            else
            {
                try
                {
                    Value = float.Parse(ValueField.Text);
                    NoValue = false;
                }
                catch (Exception e)
                {
                    NoValue = true;
#if DEBUG
                    Console.WriteLine(e.Message);
                    Exception n = e;
#endif
                }
            }
        }

        public void set(float Value, bool NoValue)
        {
            set(Value);
            set(NoValue);
        }

        public void set(float Value)
        {
            this.Value = Value;
            if (!NoValue)
                ValueField.SetText(Value.ToString());
            else
                ValueField.SetText("");
        }

        public void set(bool NoValue)
        {
            this.NoValue = NoValue;
            if (!NoValue)
                ValueField.SetText(Value.ToString());
            else
                ValueField.SetText("");
        }

        public override void SetPosition(Vector2 Position)
        {
            ValueField.SetPosition(Position + new Vector2(Font.MeasureString(Name.ToString()).X +  ValueForm.Buffer, 0));
            base.SetPosition(Position);
        }

        public override void Draw()
        {
            Game1.spriteBatch.DrawString(Font, Name, Position, TextColor);
            base.Draw();
        }
    }
}
#endif