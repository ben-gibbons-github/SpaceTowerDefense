using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif
using System.IO;

namespace BadRabbit.Carrot
{
    public class Vector4Value : Value
    {
        private Vector4 Value;


        public override void SetFromArgs(string[] args)
        {
            set(new Vector4(Logic.ParseF(args[1]), Logic.ParseF(args[2]), Logic.ParseF(args[3]), Logic.ParseF(args[4])));
        }

        public Vector4Value(string Name)
            : base(Name)
        {
            this.Value = Vector4.Zero;
        }

        public Vector4Value(string Name, Vector4 Value)
            : base(Name)
        {
            this.Value = Value;
        }

        public Vector4 get()
        {
            return Value;
        }

        public float X()
        {
            return Value.X;
        }

        public float Y()
        {
            return Value.Y;
        }

        public float Z()
        {
            return Value.Z;
        }

        public float W()
        {
            return Value.W;
        }

        public void set(Vector4 Value)
        {
            if (this.Value != Value)
            {
                this.Value = Value;
                PerformEvent();
            }
        }

        public void setX(float X)
        {
            if (X != Value.X)
            {
                Value.X = X;
                PerformEvent();
            }
        }

        public void setY(float Y)
        {
            if (Y != Value.Y)
            {
                Value.Y = Y;
                PerformEvent();
            }
        }

        public void setZ(float Z)
        {
            if (Z != Value.Z)
            {
                Value.Z = Z;
                PerformEvent();
            }
        }

        public void setW(float W)
        {
            if (W != Value.W)
            {
                Value.W = W;
                PerformEvent();
            }
        }

#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new Vector4Form(Values);
        }
#endif
        public override void Write(BinaryWriter Writer)
        {
            SaveHelper.Write(get());
            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            set(SaveHelper.ReadVector4());
            base.Read(Reader);
        }
        public static void DummyRead(BinaryReader Reader)
        {
            SaveHelper.ReadVector4();
        }
    }
}
