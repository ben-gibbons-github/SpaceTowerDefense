using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif

namespace BadRabbit.Carrot.EffectParameters
{
    public class StringParameter : BasicEffectParameter
    {
        private string Value = "";

        public StringParameter(string Name)
            : base(Name)
        {
            this.Value = "";
        }

        public StringParameter(EffectParameter Param)
            : base(Param)
        {
            this.Value = Param.GetValueString();
        }

        public string get()
        {
            return this.Value;
        }

        public void set(string Value)
        {
            Que();
            this.Value = Value;
        }

        public override void UpdateParameter()
        {
            MyParameter.SetValue(get());
            base.UpdateParameter();
        }

        public override void Write(BinaryWriter Writer)
        {
            Writer.Write(get());
            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            set(Reader.ReadString());
            base.Read(Reader);
        }

#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<BasicEffectParameter> Values)
        {
            return new StringForm(Values);
        }
#endif
    }
}
