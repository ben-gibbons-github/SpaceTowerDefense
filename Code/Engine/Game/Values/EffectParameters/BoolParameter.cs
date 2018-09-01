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
    public class BoolParameter : BasicEffectParameter
    {
        private bool Value = false;

        public BoolParameter(string Name)
            : base(Name)
        {

        }

        public BoolParameter(EffectParameter Param)
            : base(Param)
        {
            this.Value = Param.GetValueBoolean();
        }

        public bool get()
        {
            return this.Value;
        }

        public void set(bool Value)
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
            set(Reader.ReadBoolean());
            base.Read(Reader);
        }

#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<BasicEffectParameter> Values)
        {
            return new BoolForm(Values);
        }
#endif
    }
}
