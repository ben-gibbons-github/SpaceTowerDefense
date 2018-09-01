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
    public class TypeValue : Value
    {
        private Type Value;
        private Type RequiredType;

        public TypeValue(string Name, Type RequiredType)
            : base(Name)
        {
            this.Value = null;
            this.RequiredType = RequiredType;
        }

        public Type get()
        {
            return Value;
        }

        public void set(Type Value)
        {
            if (Value != null && (Value == RequiredType || Value.IsSubclassOf(RequiredType)))
            {
                this.Value = Value;
                PerformEvent();
            }
        }

        public void clear()
        {
            Value = null;
            PerformEvent();
        }

        public void set(string Name)
        {
            if (Name.Equals(""))
                clear();
            else
            {
                Type Value = CreatorBasic.FindType(Name);
                if (Value != null)
                    set(Value);
            }
        }

        public override void SetFromArgs(string[] args)
        {
            set(args[1]);
        }

#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new TypeForm(Values);
        }
#endif
        public override void Write(BinaryWriter Writer)
        {
            Writer.Write(get() != null ? get().Name : " ");
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

        public Type getRequiredType()
        {
            return RequiredType;
        }
    }
}
