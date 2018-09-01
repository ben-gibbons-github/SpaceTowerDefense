
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
    public class ObjectListValue : Value
    {
        public static Dictionary<GameObject, LinkedList<ObjectListValue>> LinkedObjects = new Dictionary<GameObject, LinkedList<ObjectListValue>>();

        public static void ClearObject(GameObject o)
        {
            if (LinkedObjects.ContainsKey(o))
            {
                LinkedList<ObjectListValue> removeList = new LinkedList<ObjectListValue>();

                foreach (ObjectListValue v in LinkedObjects[o])
                    removeList.AddLast(v);

                foreach(ObjectListValue v in removeList)
                    v.remove(o);

                LinkedObjects[o].Clear();
            }
        }

        public LinkedList<GameObject> Value = new LinkedList<GameObject>();
        private LinkedList<int> IDs = new LinkedList<int>();
        private Type ObjectType;

        public ObjectListValue(string Name, Type ObjectType)
            : base(Name)
        {
            this.ObjectType = ObjectType;
        }

        public ObjectListValue(string Name, Type ObjectType, params GameObject[] Values)
            : base(Name)
        {
            foreach (GameObject o in Values)
                add(o);
            this.ObjectType = ObjectType;
        }

        public Type getObjectType()
        {
            return ObjectType;
        }

        public void add(int ID)
        {
            GameObject o = Parent.ParentLevel.FindObject(ID);
            if (o != null && o.GetType().Equals(ObjectType)
#if EDITOR && WINDOWS
                && (GameObject.CloneDictionary == null || !GameObject.CloneDictionary.ContainsKey(o) || GameObject.CloneDictionary[o] != null)
#endif
                )
                add(o);
            else if(!IDs.Contains(ID))
                IDs.AddLast(ID);
            PerformEvent();
        }

        public void add(GameObject o)
        {
            if (o != null && o.GetType().Equals(ObjectType))
            {
#if EDITOR && WINDOWS
                if (GameObject.CloneDictionary != null && GameObject.CloneDictionary.ContainsKey(o))
                    o = GameObject.CloneDictionary[o];
#endif
                this.Value.AddLast(o);
                if (!LinkedObjects.ContainsKey(o))
                    LinkedObjects.Add(o, new LinkedList<ObjectListValue>());

                LinkedObjects[o].AddLast(this);

                PerformEvent();
            }
        }

        public void remove(GameObject o)
        {
            if (Value.Contains(o))
            {
                LinkedObjects[o].Remove(this);
                Value.Remove(o);
                PerformEvent();
            }
        }

        private void clear()
        {
            Value = null;
            PerformEvent();
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return null;
        }
#endif
        public override void Write(BinaryWriter Writer)
        {
            Writer.Write((Int32)Value.Count);
            foreach (GameObject o in Value)
                Writer.Write((Int32)o.IdNumber);

            base.Write(Writer);
        }

        public override void Read(BinaryReader Reader)
        {
            int ChildCount = Reader.ReadInt32();
            for (int i = 0; i < ChildCount; i++)
                add(Reader.ReadInt32());
            base.Read(Reader);
        }

        public static void DummyRead(BinaryReader Reader)
        {
            int Count = Reader.ReadInt32();
            for (int i = 0; i < Count; i++)
                Reader.ReadInt32();
        }

        public override void PostRead()
        {
            foreach (int i in IDs)
                add(i);
            base.PostRead();
        }
    }
}