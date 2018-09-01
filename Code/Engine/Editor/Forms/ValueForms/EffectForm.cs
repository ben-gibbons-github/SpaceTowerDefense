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
    public class EffectForm : ValueForm
    {
        public TextField ValueField;
        public Effect Value = null;
        public string Path = "";
        public bool NoValue = false;
        //private LinkedList<ValueForm> ChildForms;

        public EffectForm(LinkedList<Value> ReferenceValues)
            : base(ReferenceValues)
        {
            ValueField = (TextField)AddForm(new TextField(Vector2.Zero, (int)Font.MeasureString(Path.ToString()).X, Font, Path, GetValueFromField));

            GetValueFromReferences();
        }

        public override void Create(FormHolder Parent)
        {
            SetSize(ValueField.Size + new Vector2(Font.MeasureString(Name).X, 0));

            if (!NoValue)
            {
                LinkedList<Form> Forms = new LinkedList<Form>();
                Dictionary<string, LinkedList<BasicEffectParameter>> Values = new Dictionary<string, LinkedList<BasicEffectParameter>>();

                foreach (EffectValue e in ReferenceValues)
                    foreach (BasicEffectParameter v in e.Parameters.Values)
                    {
                        LinkedList<BasicEffectParameter> Vlist = null;
                        if (!Values.Keys.Contains(v.Name))
                            Values.Add(v.Name, Vlist = new LinkedList<BasicEffectParameter>());
                        else
                            Vlist = Values[v.Name];

                        Vlist.AddLast(v);
                    }

                foreach (string s in Values.Keys)
                       Parent.AddForm(Values[s].First.Value.GetForm(Values[s]));
                    
            }

            base.Create(Parent);
        }

        public override void SetPosition(Vector2 Position)
        {
            ValueField.SetPosition(Position + new Vector2(Font.MeasureString(Name.ToString()).X + Buffer, 0));
            base.SetPosition(Position);
        }

        public override void GetValueFromReferences()
        {
            foreach (EffectValue val in ReferenceValues)
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
                Value = AssetManager.LoadEffect(GameManager.GetLevel().MyScene.EffectDirectory.get() + Path);

                NoValue = false;
                ValueField.TextColor = FormFormat.TextColor;
            }
            catch (Exception e)
            {
                NoValue = true;
                ValueField.TextColor = Color.Red;
#if DEBUG
                Exception n = e;
                Console.WriteLine(e.Message);
#endif
            }

            if(!NoValue)
                foreach (EffectValue val in ReferenceValues)
                    val.set(Value, Path);

            
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