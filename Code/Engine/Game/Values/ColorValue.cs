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
    public class ColorValue : Value
    {
        private Vector4 Value;
        private float Mult = 1;

        public ColorValue()
            : base("Color")
        {
            this.Value = Vector4.One;
        }

        public ColorValue(string Name)
            : base(Name)
        {
            this.Value = Vector4.One;
        }

        public ColorValue(string Name, Vector4 Value)
            : base(Name)
        {
            this.Value = Value;
        }

        public ColorValue(string Name, Vector4 Value, float Mult)
            : base(Name)
        {
            this.Value = Value;
            this.Mult = Mult;
        }

        public override void SetFromArgs(string[] args)
        {
            if (args[4].Equals(""))
                set(new Vector4(Logic.ParseF(args[1]), Logic.ParseF(args[2]), Logic.ParseF(args[3]), 1));
            else
                set(new Vector4(Logic.ParseF(args[1]), Logic.ParseF(args[2]), Logic.ParseF(args[3]), Logic.ParseF(args[4])));

            base.SetFromArgs(args);
        }

        public Vector4 get()
        {
            return Value * Mult;
        }

        public Vector4 getBase()
        {
            return Value;
        }

        public float R()
        {
            return Value.X * Mult;
        }

        public float G()
        {
            return Value.Y * Mult;
        }

        public float B()
        {
            return Value.Z * Mult;
        }

        public float A()
        {
            return Value.W * Mult;
        }


        public float getMult()
        {
            return Mult;
        }

        public void set(Vector4 Value)
        {
            if (Value != this.Value)
            {
                this.Value = Value;
                PerformEvent();
            }
        }

        public void set(Vector4 Value, float Mult)
        {
            if (Value != this.Value || Mult != this.Mult)
            {
                this.Value = Value;
                this.Mult = Mult;
                PerformEvent();
            }
        }

        public void setR(float Value)
        {
            if (Value != this.Value.X)
            {
                this.Value.X = Value;
                PerformEvent();
            }
        }

        public void setG(float Value)
        {
            if (Value != this.Value.Y)
            {
                this.Value.Y = Value;
                PerformEvent();
            }
        }

        public void setB(float Value)
        {
            if (Value != this.Value.Z)
            {
                this.Value.Z = Value;
                PerformEvent();
            }
        }

        public void setA(float Value)
        {
            if (Value != this.Value.W)
            {
                this.Value.W = Value;
                PerformEvent();
            }
        }

        public void setIntensity(float Value)
        {
            if (this.Mult != Value)
            {
                this.Mult = Value;
                PerformEvent();
            }
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new ColorForm(Values);
        }
#endif

        public override void Write(BinaryWriter Writer)
        {
            SaveHelper.Write(getBase());
            Writer.Write((Single)getMult());

            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            set(SaveHelper.ReadVector4(), Reader.ReadSingle());

            base.Read(Reader);
        }


        public static void DummyRead(BinaryReader Reader)
        {
            SaveHelper.ReadVector4();
            Reader.ReadSingle();
        }

        public Color getAsColor()
        {
            return new Color(Value * Mult);
        }
    }
}
