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
    public class ValueForm : Form
    {
        public enum ValueFormType
        {
            Value,
            Parameter
        }

        public ValueFormType FormType = ValueFormType.Value;
        public LinkedList<Value> ReferenceValues = new LinkedList<Value>();
        public LinkedList<BasicEffectParameter> ReferenceParameters = new LinkedList<BasicEffectParameter>();
        public string Name;
        public Color TextColor = FormFormat.TextColor;
        public SpriteFont Font = FormFormat.NormalFont;

        public static int Buffer = 10;

        public ValueForm(LinkedList<Value> ReferenceValues)
        {
            this.ReferenceValues = ReferenceValues;
            foreach (Value v in ReferenceValues)
                v.LinkedForm = this;

            FormType = ValueFormType.Value;
        }

        public ValueForm(LinkedList<BasicEffectParameter> ReferenceParameters)
        {
            this.ReferenceParameters = ReferenceParameters;
            foreach (BasicEffectParameter v in ReferenceParameters)
                v.LinkedForm = this;

            FormType = ValueFormType.Parameter;
        }

        public virtual void GetValueFromReferences()
        {

        }

        public virtual void GetValueFromField()
        {

        }
    }
}
#endif