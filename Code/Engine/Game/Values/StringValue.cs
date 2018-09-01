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
    public class StringValue : Value
    {
        private string Value;


        public override void SetFromArgs(string[] args)
        {
            set(args[1]);
        }

        public StringValue(string Name)
            : base(Name)
        {
            this.Value = "";
        }

        public StringValue(string Name, string Value)
            : base(Name)
        {
            this.Value = Value;
        }

        public string get()
        {
            return Value;
        }

        public void set(string Value)
        {
            if (!Value.Equals(this.Value))
            {
                this.Value = Value;
                PerformEvent();
            }
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new StringForm(Values);
        }
#endif
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
        public static void DummyRead(BinaryReader Reader)
        {
            Reader.ReadString();
        }
    }
}
