using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace BadRabbit.Carrot
{
    public class ModelValue : Value
    {
        private Model Value;
        private string Path;

        public ModelValue(string Name)
            : base(Name)
        {
            this.Value = null;
            this.Path = "";
        }

        public ModelValue(string Name, string Path)
            : base(Name)
        {
            this.Value = LoadModel(Path);
        }

        public ModelValue(string Name, Model Value)
            : base(Name)
        {
            this.Value = Value;
            this.Path = "";
        }

        public ModelValue(string Name, Model Value, string Path)
            : base(Name)
        {
            this.Value = Value;
            this.Path = Path;
        }

        public override void SetFromArgs(string[] args)
        {
            set(args[1]);
        }

        public Model LoadModel(string Path)
        {
            this.Path = Path;
            if (Parent.CanLoad && !Path.Equals(""))
                return AssetManager.Load<Model>(Parent.GetParent().ModelDirectory.get() + Path);
            else
                return null;
        }

        public override void Load()
        {
            set(Path);
        }

        public Model get()
        {
            return Value;
        }

        public string getPath()
        {
            return Path;
        }

        public string getFullPath()
        {
            return Parent.GetParent().ModelDirectory.get() + Path;
        }

        public void set(string Path)
        {
            this.Value = LoadModel(Path);
            PerformEvent();
        }

        public void set(Model Value)
        {
            this.Value = Value;
            PerformEvent();
        }

        public void set(Model Value, string Path)
        {
            this.Value = Value;
            this.Path = Path;
            PerformEvent();
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new ModelForm(Values);
        }
#endif
        public override void Write(BinaryWriter Writer)
        {
            Writer.Write(getPath());

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
