using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif
using System.IO;

namespace BadRabbit.Carrot
{
    public class BoolValue : Value
    {
        private bool Value;

        public BoolValue(string Name)
            : base(Name)
        {
            this.Value = false;
        }

        public BoolValue(string Name, ValueChangeEvent Event)
            : base(Name, Event)
        {
            this.Value = false;
        }

        public BoolValue(string Name, bool Value)
            : base(Name)
        {
            this.Value = Value;
        }

        public bool get()
        {
            return Value;
        }

        public override void SetFromArgs(string[] args)
        {
            switch (args[1])
            {
                case "true":
                    set(true);
                    return;
                case "t":
                    set(true);
                    return;
                case "1":
                    set(true);
                    return;
                default:
                    set(false);
                    return;
            }
        }

        public void set(bool Value)
        {
            if (Value != this.Value)
            {
                this.Value = Value;
                PerformEvent();
            }
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new BoolForm(Values);
        }
#endif
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

        public static void DummyRead(BinaryReader Reader)
        {
            Reader.ReadBoolean();
        }
    }
}
