using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif

namespace BadRabbit.Carrot.EffectParameters
{
    public class Vector2Parameter : BasicEffectParameter
    {
        private Vector2 Value = Vector2.Zero;

        public Vector2Parameter(string Name)
            : base(Name)
        {

        }

        public Vector2Parameter(EffectParameter Param)
            : base(Param)
        {
            this.Value = Param.GetValueVector2();
        }

        public Vector2 get()
        {
            return this.Value;
        }

        public void set(Vector2 Value)
        {
            this.Value = Value;
            Que();
        }

        public void setX(float X)
        {
            this.Value.X = X;
            Que();
        }

        public void setY(float Y)
        {
            this.Value.Y = Y;
            Que();
        }

        public override void UpdateParameter()
        {
            MyParameter.SetValue(get());
            base.UpdateParameter();
        }

        public override void Write(BinaryWriter Writer)
        {
            SaveHelper.Write(Value);
            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            set(SaveHelper.ReadVector2());
            base.Read(Reader);
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<BasicEffectParameter> Values)
        {
            return new Vector2Form(Values, EditType.Basic);
        }
#endif
    }
}
