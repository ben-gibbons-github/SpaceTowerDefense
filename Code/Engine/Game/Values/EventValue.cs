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
    public class EventValue : Value
    {
        enum ParseStage
        {
            Object,
            Function,
            Arguments
        }

        private string Value;
        private bool NeedsParsing = false;
        EventCall MyEvent;

        public EventValue(string Name)
            : base(Name)
        {
            this.Value = "";
        }

        public EventValue(string Name, string Value)
            : base(Name)
        {
            this.Value = Value;
            NeedsParsing = true;
        }

        public string get()
        {
            return Value;
        }

        public override void SetFromArgs(string[] args)
        {
            set(args[1]);
        }

        private void Parse()
        {
            MyEvent = null;
            ParseStage stage = ParseStage.Object;
            StringBuilder ObjectBuilder = new StringBuilder();
            StringBuilder FunctionBuilder = new StringBuilder();
            StringBuilder[] ArgBuilder = new StringBuilder[16];
            int ArgCount = 0;

            for (int i = 0; i < Value.Count(); i++)
            {
                switch (Value[i])
                {
                    case '.':
                        stage = ParseStage.Function;
                        break;

                    case '(':
                        stage = ParseStage.Arguments;
                        break;

                    case ',':
                        if (stage == ParseStage.Arguments)
                            ArgCount++;
                        break;

                    case ')':
                        if (ArgBuilder[ArgCount] != null)
                            ArgCount++;
                        string[] args = new string[ArgCount];

                        for (int a = 0; a < ArgCount; a++)
                        {
                            if (ArgBuilder[a] != null)
                            {
                                args[a] = ArgBuilder[a].ToString().ToLower();
                                ArgBuilder[a] = new StringBuilder();
                            }
                        }

                        ArgCount = 0;

                        EventCall e = new EventCall();

                        e.args = args;

                        string ObjectString = ObjectBuilder.ToString();
                        if (!ObjectString.Equals("this"))
                        {
                            e.MyObject = Parent.ParentLevel.FindObject(ObjectString);
                            if (e.MyObject == null)
                                e.MyObject = Parent.ParentScene;
                        }
                        else
                            e.MyObject = Parent;

                        string FunctionString = FunctionBuilder.ToString().ToLower();
                        foreach (EventType t in EventCall.AllEvents)
                            if (t.ToString().ToLower().Equals(FunctionString))
                            {
                                e.Function = t;
                                break;
                            }

                        ObjectBuilder = new StringBuilder();
                        FunctionBuilder = new StringBuilder();

                        if (MyEvent == null)
                            MyEvent = e;
                        else
                            MyEvent.Add(e);

                        stage = ParseStage.Object;
                        break;

                    default:
                        switch (stage)
                        {
                            case ParseStage.Object:
                                if (Value[i] != ' ')
                                    ObjectBuilder.Append(Value[i]);
                                break;

                            case ParseStage.Function:
                                if (Value[i] != ' ')
                                    FunctionBuilder.Append(Value[i]);
                                break;

                            case ParseStage.Arguments:
                                if (ArgBuilder[ArgCount] == null)
                                    ArgBuilder[ArgCount] = new StringBuilder();
                                ArgBuilder[ArgCount].Append(Value[i]);
                                break;
                        }
                        break;
                }
            }
            NeedsParsing = false;
        }

        public void Trigger() 
        {
            if (NeedsParsing)
                Parse();

            if (MyEvent != null)
                MyEvent.Trigger(Parent);
        }

        public void set(string Value)
        {
            if (!Value.Equals(this.Value))
            {
                this.Value = Value;
                PerformEvent();
                NeedsParsing = true;
            }
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new EventForm(Values);
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
