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
    public class IntValue : Value
    {
        private int Value;

        public IntValue(string Name)
            : base(Name)
        {
            this.Value = 0;
        }

        public IntValue(string Name, ValueChangeEvent Event)
            : base(Name)
        {
            this.ChangeEvent = Event;
            this.Value = 0;
        }

        public IntValue(string Name, int Value)
            : base(Name)
        {
            this.Value = Value;
        }

        public IntValue(string Name, int Value, ValueChangeEvent Event)
            : base(Name)
        {
            this.Value = Value;
            this.ChangeEvent = Event;
        }

        public override void SetFromArgs(string[] args)
        {
            set(Logic.ParseI(args[1]));
        }

        public int get()
        {
            return this.Value;
        }

        public void set(int Value)
        {
            if (this.Value != Value)
            {
                this.Value = Value;
                PerformEvent();
            }
        }

        public void add(int Value)
        {
            this.Value += Value;
            PerformEvent();
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new IntForm(Values);
        }
#endif
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

        public static void DummyRead(BinaryReader Reader)
        {
            Reader.ReadInt32();
        }
    }
}
