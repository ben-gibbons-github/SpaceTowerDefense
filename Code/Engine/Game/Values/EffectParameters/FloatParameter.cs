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
    public class FloatParameter : BasicEffectParameter
    {
        private float Value = 0;

        public FloatParameter(string Name)
            : base(Name)
        {

        }

        public FloatParameter(EffectParameter Param)
            : base(Param)
        {
            this.Value = Param.GetValueSingle();
        }

        public float get()
        {
            return this.Value;
        }

        public void set(float Value)
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
            Writer.Write((Single)get());
            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            set(Reader.ReadSingle());
            base.Read(Reader);
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<BasicEffectParameter> Values)
        {
            return new FloatForm(Values);
        }
#endif
    }
}
