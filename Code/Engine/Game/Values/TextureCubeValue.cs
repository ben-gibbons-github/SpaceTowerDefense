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
    public class TextureCubeValue : Value
    {
        private TextureCubeReference Value;
        private string Path;


        public override void SetFromArgs(string[] args)
        {
            set(args[1]);
        }

        public TextureCubeValue(string Name)
            : base(Name)
        {
            this.Value = null;
            this.Path = "";
        }

        public TextureCubeValue(string Name, string Path)
            : base(Name)
        {
            this.Value = LoadTexture(Path);
        }

        public TextureCubeValue(string Name, TextureCube Value)
            : base(Name)
        {
            this.Value = Value != null ? new TextureCubeReference(Value) : null;
            this.Path = "";
        }

        public TextureCubeValue(string Name, TextureCube Value, string Path)
            : base(Name)
        {
            this.Value = Value != null ? new TextureCubeReference(Value) : null;
            this.Path = Path;
        }

        public TextureCubeReference LoadTexture(string Path)
        {
            this.Path = Path;
            if (Parent.CanLoad && !Path.Equals(""))
            {
                TextureCube tc = AssetManager.Load<TextureCube>(Path);
                return tc != null ? new TextureCubeReference(tc) : null;
            }
            else
                return null;
        }

        public override void Load()
        {
            set(Path);
        }

        public TextureCube get()
        {
            return Value.get();
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

        public void set(TextureCubeReference Value)
        {
            this.Value = Value;
            PerformEvent();
        }

        public void set(TextureCubeReference Value, string Path)
        {
            this.Value = Value;
            this.Path = Path;
            PerformEvent();
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new TextureCubeForm(Values);
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

        public override void Destroy()
        {
            base.Destroy();
        }

        public static void DummyRead(BinaryReader Reader)
        {
            Reader.ReadString();
        }
    }
}
