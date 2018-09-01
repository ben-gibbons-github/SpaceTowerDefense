using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif
namespace BadRabbit.Carrot
{
    public delegate void ValueChangeEvent();

    public class Value
    {

        public static void DummyRead(byte ByteType, BinaryReader Reader)
        {
            switch (ByteType)
            {
                case 0: BoolValue.DummyRead(Reader); break;
                case 1: ColorValue.DummyRead(Reader); break;
                case 2: EffectValue.DummyRead(Reader); break;
                case 3: FloatValue.DummyRead(Reader); break;
                case 4: ModelValue.DummyRead(Reader); break;
                case 5: ObjectValue.DummyRead(Reader); break;
                case 6: SpriteFontValue.DummyRead(Reader); break;
                case 7: StringValue.DummyRead(Reader); break;
                case 8: Texture2DValue.DummyRead(Reader); break;
                case 9: TextureCubeValue.DummyRead(Reader); break;
                case 10: Vector2Value.DummyRead(Reader); break;
                case 11: Vector3Value.DummyRead(Reader); break;
                case 12: Vector4Value.DummyRead(Reader); break;
                case 13: ObjectListValue.DummyRead(Reader); break;
                case 14: IntValue.DummyRead(Reader); break;
                case 15: EventValue.DummyRead(Reader); break;
                case 16: TypeValue.DummyRead(Reader); break;
            }
        }

        public static byte ReturnByteType(Value v)
        {
            Type t = v.GetType();

            if (t.Equals(typeof(BoolValue)))
                return 0;
            if (t.Equals(typeof(ColorValue)))
                return 1;
            if (t.Equals(typeof(EffectValue)))
                return 2;
            if (t.Equals(typeof(FloatValue)))
                return 3;
            if (t.Equals(typeof(ModelValue)))
                return 4;
            if (t.Equals(typeof(ObjectValue)))
                return 5;
            if (t.Equals(typeof(SpriteFontValue)))
                return 6;
            if (t.Equals(typeof(StringValue)))
                return 7;
            if (t.Equals(typeof(Texture2DValue)))
                return 8;
            if (t.Equals(typeof(TextureCubeValue)))
                return 9;
            if (t.Equals(typeof(Vector2Value)))
                return 10;
            if (t.Equals(typeof(Vector3Value)))
                return 11;
            if (t.Equals(typeof(Vector4Value)))
                return 12;
            if (t.Equals(typeof(ObjectListValue)))
                return 13;
            if (t.Equals(typeof(IntValue)))
                return 14;
            if (t.Equals(typeof(EventValue)))
                return 15;
            if (t.Equals(typeof(TypeValue)))
                return 16;

            return 0;
        }

#if EDITOR && WINDOWS
        public static bool ChangeFromForm = false;
        public ValueForm LinkedForm;
        public bool Editable = true;
#endif

        public GameObject Parent;
        public string Name;
        public ValueChangeEvent ChangeEvent;

        public Value(string Name)
        {
            this.Parent = Level.ReferenceObject;
            this.Name = Name;

            Parent.AddValue(this);
        }

        public Value(string Name, ValueChangeEvent Event)
        {
            this.Parent = Level.ReferenceObject;
            this.Name = Name;
            this.ChangeEvent = Event;

            Parent.AddValue(this);
        }

        public virtual void SetFromArgs(string[] args)
        {

        }

        public virtual void Load()
        {

        }
#if EDITOR && WINDOWS
        public virtual Form GetForm(LinkedList<Value> Values)
        {
            return null;
        }
#endif
        public virtual void Write(BinaryWriter Writer)
        {

        }

        public virtual void Read(BinaryReader Reader)
        {

        }

        public virtual void PostRead()
        {

        }

        public virtual void Destroy()
        {

        }

        public void PerformEvent()
        {
            if (ChangeEvent != null)
                ChangeEvent();

#if EDITOR && WINDOWS
            if (Parent.GetParent().ParentLevel.LevelForEditing && Parent.EditorSelected && !ChangeFromForm)
            {
                if (LinkedForm != null)
                    LinkedForm.GetValueFromReferences();
                ObjectProperties.self.NeedsToRedraw = true;
            }
#endif
        }
    }
}
