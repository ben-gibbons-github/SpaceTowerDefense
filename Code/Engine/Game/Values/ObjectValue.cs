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
    public class ObjectValue : Value
    {
        public static Dictionary<GameObject, LinkedList<ObjectValue>> LinkedObjects = new Dictionary<GameObject, LinkedList<ObjectValue>>();

        public static void ClearObject(GameObject o)
        {
            if (LinkedObjects.ContainsKey(o))
            {
                LinkedList<ObjectValue> l = new LinkedList<ObjectValue>();

                foreach (ObjectValue v in LinkedObjects[o])
                    l.AddLast(v);
                foreach (ObjectValue v in l)
                    v.clear();

                LinkedObjects[o].Clear();
            }
        }

        private GameObject Value;
        private Type ObjectType;
        private int ID;
        private bool AllowGeneralTypes;

        public ObjectValue(string Name, Type ObjectType)
            : base(Name)
        {
            this.Value = null;
            this.ObjectType = ObjectType;
        }

        public ObjectValue(string Name, Type ObjectType, bool AllowGeneralTypes)
            : base(Name)
        {
            this.AllowGeneralTypes = AllowGeneralTypes;
            this.Value = null;
            this.ObjectType = ObjectType;
        }

        public ObjectValue(string Name, GameObject Value, Type ObjectType)
            : base(Name)
        {
            this.Value = Value;
            this.ObjectType = ObjectType;
        }

        public GameObject get()
        {
            return Value;
        }

        public Type getObjectType()
        {
            return ObjectType;
        }

        public void set(GameObject Value)
        {
            if (Value != null && (Value.GetType().Equals(ObjectType) || Value.GetType().IsSubclassOf(ObjectType)))
            {
                ValueChange();
#if EDITOR && WINDOWS
                if (GameObject.CloneDictionary != null && GameObject.CloneDictionary.ContainsKey(Value))
                    this.Value = GameObject.CloneDictionary[Value];
                else
#endif
                this.Value = Value;
                if (!LinkedObjects.ContainsKey(this.Value))
                    LinkedObjects.Add(this.Value, new LinkedList<ObjectValue>());

                LinkedObjects[this.Value].AddLast(this);

                PerformEvent();
            }
        }

        public void clear()
        {
            ValueChange();
            Value = null;
            PerformEvent();
        }

        public void set(int ID)
        {
            this.ID = ID;
            GameObject Value = Parent.ParentLevel.FindObject(ID);
            if (Value != null && (Value.GetType().Equals(ObjectType) || Value.GetType().IsSubclassOf(ObjectType)))
            {
                this.ID = ID;
                set(Value);
            }
        }

        public void set(string Name)
        {
            if (Name.Equals(""))
                clear();
            else
            {
                GameObject Value = Parent.ParentLevel.FindObject(Name);
                if (Value != null)
                    set(Value);
            }
        }

        public override void SetFromArgs(string[] args)
        {
            set(args[1]);
        }

        private void ValueChange()
        {
            if (Value != null)
            {
                LinkedObjects[Value].Remove(this);
                Value = null;
            }
        }
        #if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new ObjectForm(Values);
        }
#endif
        public override void Write(BinaryWriter Writer)
        {
            Writer.Write((Int32)(get() != null ? get().IdNumber : -1));
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

        public override void PostRead()
        {
            this.set(ID);
            base.PostRead();
        }
    }
}
