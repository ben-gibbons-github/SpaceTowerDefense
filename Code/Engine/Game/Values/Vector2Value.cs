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
    public class Vector2Value : Value
    {
        private Vector2 Value;
        public EditType editType = EditType.Basic;


        public override void SetFromArgs(string[] args)
        {
            set(new Vector2(Logic.ParseF(args[1]), Logic.ParseF(args[2])));
        }

        public Vector2Value(string Name)
            : base(Name)
        {
            this.Value = Vector2.Zero;
        }

        public Vector2Value(string Name,EditType editType)
            : base(Name)
        {
            this.Value = Vector2.Zero;
            this.editType = editType;
        }

        public Vector2Value(string Name, Vector2 Value)
            : base(Name)
        {
            this.Value = Value;
        }

        public Vector2Value(string Name, Vector2 Value,EditType editType)
            : base(Name)
        {
            this.Value = Value;
            this.editType = editType;
        }

        public Vector2Value(string Name, Vector2 Value, ValueChangeEvent e)
            : base(Name,e)
        {
            this.Value = Value;
        }


        public Vector2 get()
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

        public void set(Vector2 Value)
        {
            if (this.Value != Value)
            {
                this.Value = Value;
                PerformEvent();
            }
        }

        public void setNoPerform(Vector2 Value)
        {
            this.Value = Value;
        }

        public void mult(Vector2 Value)
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

        public void add(Vector2 Value)
        {
            this.Value += Value;
            PerformEvent();
        }

        public void addNoPerform(Vector2 Value)
        {
            this.Value += Value;
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
        
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new Vector2Form(Values, editType);
        }
#endif
        public override void Write(BinaryWriter Writer)
        {
            SaveHelper.Write(get());
            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            set(SaveHelper.ReadVector2());
            base.Read(Reader);
        }
        public static void DummyRead(BinaryReader Reader)
        {
            SaveHelper.ReadVector2();
        }
    }
}
