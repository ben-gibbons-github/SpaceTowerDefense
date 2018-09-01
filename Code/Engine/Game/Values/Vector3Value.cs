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
    public enum EditType
    {
        Basic,
        Average,
        Scalar
    }

    public class Vector3Value : Value
    {
        private Vector3 Value;
        public EditType editType = EditType.Basic;

        public override void SetFromArgs(string[] args)
        {
            set(new Vector3(Logic.ParseF(args[1]), Logic.ParseF(args[2]), Logic.ParseF(args[3])));
        }

        public Vector3Value(string Name)
            : base(Name)
        {
            this.Value = Vector3.Zero;
        }

        public Vector3Value(string Name, ValueChangeEvent Event)
            : base(Name, Event)
        {
            this.Value = Vector3.Zero;
        }

        public Vector3Value(string Name, EditType editType, ValueChangeEvent Event)
            : base(Name, Event)
        {
            this.editType = editType;
            this.Value = Vector3.Zero;
        }

        public Vector3Value(string Name, Vector3 Value)
            : base(Name)
        {
            this.Value = Value;
        }

        public Vector3Value(string Name, Vector3 Value, ValueChangeEvent Event)
            : base(Name, Event)
        {
            this.Value = Value;
        }

        public Vector3Value(string Name, Vector3 Value, EditType editType, ValueChangeEvent Event)
            : base(Name, Event)
        {
            this.editType = editType;
            this.Value = Value;
        }

        public Vector3 get()
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

        public Vector3 getAsRadians()
        {
            return new Vector3(
                MathHelper.ToRadians(Value.X),
                MathHelper.ToRadians(Value.Y),
                MathHelper.ToRadians(Value.Z)
                );
        }

        public void set(Vector3 Value)
        {
            if (this.Value != Value)
            {
                this.Value = Value;
                PerformEvent();
            }
        }

        public void add(Vector3 Value)
        {
            this.Value += Value;
            PerformEvent();
        }

        public void mult(Vector3 Value)
        {
            this.Value *= Value;
            PerformEvent();
        }

        public void multX(float Value)
        {
            this.Value.X *= Value;
            PerformEvent();
        }

        public void multY(float Value)
        {
            this.Value.Y *= Value;
            PerformEvent();
        }

        public void multZ(float Value)
        {
            this.Value.Z *= Value;
            PerformEvent();
        }

        public void addX(float Value)
        {
            this.Value.X += Value;
            PerformEvent();
        }

        public void addY(float Value)
        {
            this.Value.Y += Value;
            PerformEvent();
        }

        public void addZ(float Value)
        {
            this.Value.Z += Value;
            PerformEvent();
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
        
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new Vector3Form(Values, editType);
        }
#endif
        public override void Write(BinaryWriter Writer)
        {
            SaveHelper.Write(get());
            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            set(SaveHelper.ReadVector3());
            base.Read(Reader);
        }
        public static void DummyRead(BinaryReader Reader)
        {
            SaveHelper.ReadVector3();
        }
    }
}
