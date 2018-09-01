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
    public class IntParameter : BasicEffectParameter
    {
        private int Value = 0;

        public IntParameter(string Name)
            : base(Name)
        {

        }

        public IntParameter(EffectParameter Param)
            : base(Param)
        {
            this.Value = Param.GetValueInt32();
        }

        public int get()
        {
            return this.Value;
        }

        public void set(int Value)
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
            Writer.Write((Int32)get());
            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            set(Reader.ReadInt32());
            base.Read(Reader);
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<BasicEffectParameter> Values)
        {
            return new IntForm(Values);
        }
#endif
    }
}
