#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif

namespace BadRabbit.Carrot
{
    public class SliderHolder : Form
    {

        public SpriteFont Font = FormFormat.NormalFont;
        public Vector2 TextPosition = Vector2.Zero;
        public Color TextColor = FormFormat.TextColor;
        public EnterEvent Event;

        public string Name;
        public Slider ValueField;
        public bool NoValue = false;
        public float Value;


        public SliderHolder(Form Parent, string Name, EnterEvent Event, float min, float max)
        {
            this.Name = Name;
            this.Event = Event;

            AddForm(ValueField = new Slider(Position + new Vector2(Font.MeasureString(Name).X, 0), new Vector2(200, Font.MeasureString(Name).Y), max, min , true, Event));
            SetSize(Font.MeasureString(Name) + new Vector2(ValueField.Size.X, 0));
        }

        public void get()
        {
            NoValue = ValueField.NoValue;

            if (!ValueField.NoValue)
                Value = ValueField.GetValue();
        }

        public void set(float Value, bool NoValue)
        {
            set(Value);
            set(NoValue);
        }

        public void set(float Value)
        {
            this.Value = Value;
            ValueField.SetValue(Value);
            ValueField.NoValue = NoValue;
        }

        public void set(bool NoValue)
        {
            this.NoValue = NoValue;
            ValueField.SetValue(Value);
            ValueField.NoValue = NoValue;
        }

        public override void SetPosition(Vector2 Position)
        {
            ValueField.SetPosition(Position + new Vector2(Font.MeasureString(Name.ToString()).X + ValueForm.Buffer, 0));
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