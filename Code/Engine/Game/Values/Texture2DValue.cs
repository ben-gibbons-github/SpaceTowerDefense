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
    public class Texture2DValue : Value
    {
        private Texture2D Value;
        private string Path;


        public override void SetFromArgs(string[] args)
        {
            set(args[1]);
        }

        public Texture2DValue(string Name)
            : base(Name)
        {
            this.Value = null;
            this.Path = "";
        }

        public Texture2DValue(string Name, string Path)
            : base(Name)
        {
            this.Value = LoadTexture(Path);
        }

        public Texture2DValue(string Name, ValueChangeEvent Event)
            : base(Name, Event)
        {
            this.Value = null;
            this.Path = "";
        }

        public Texture2DValue(string Name, Texture2D Value)
            : base(Name)
        {
            this.Value = Value;
            this.Path = "";
        }

        public Texture2DValue(string Name, Texture2D Value, string Path)
            : base(Name)
        {
            this.Value = Value;
            this.Path = Path;
        }

        public Texture2D LoadTexture(string Path)
        {
            this.Path = Path;
            if (Parent.CanLoad && !Path.Equals(""))
                return AssetManager.Load<Texture2D>(Parent.GetParent().TextureDirectory.get() + Path);
            else
                return null;
        }

        public override void Load()
        {
            set(Path);
        }

        public Texture2D get()
        {
            return Value != null ? this.Value : Render.BlankTexture;
        }

        public string getPath()
        {
            return Path;
        }

        public void set(string Path)
        {
            this.Value = LoadTexture(Path);
            PerformEvent();
        }

        public void set(Texture2D Value)
        {
            this.Value = Value;
            PerformEvent();
        }

        public void set(Texture2D Value, string Path)
        {
            this.Value = Value;
            this.Path = Path;
            PerformEvent();
        }

#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new Texture2DForm(Values);
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
