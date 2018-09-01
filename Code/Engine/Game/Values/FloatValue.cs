using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif
using System.IO;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class FloatValue : Value
    {
        private float Value;

        public FloatValue(string Name):base(Name)
        {
            this.Value = 0;
        }

        public FloatValue(string Name, ValueChangeEvent Event)
            : base(Name)
        {
            this.ChangeEvent = Event;
            this.Value = 0;
        }

        public FloatValue(string Name, float Value)
            : base(Name)
        {
            this.Value = Value;
        }

        public FloatValue(string Name, float Value, ValueChangeEvent Event)
            : base(Name)
        {
            this.Value = Value;
            this.ChangeEvent = Event;
        }

        public override void SetFromArgs(string[] args)
        {
            set(Logic.ParseF(args[1]));
        }

        public float get()
        {
            return this.Value;
        }

        public float getAsRadians()
        {
            return MathHelper.ToRadians(get());
        }

        public void set(float Value)
        {
            if(this.Value != Value)
            {
                this.Value = Value;
                PerformEvent();
            }
        }

        public void add(float Value)
        {
            this.Value += Value;
            PerformEvent();
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new FloatForm(Values);
        }
#endif
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

        public static void DummyRead(BinaryReader Reader)
        {
            Reader.ReadSingle();
        }
    }
}
