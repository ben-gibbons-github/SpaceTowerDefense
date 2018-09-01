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
    public class SpriteFontValue : Value
    {
        private SpriteFont Value;
        private string Path;

        public override void SetFromArgs(string[] args)
        {
            set(args[1]);
        }

        public SpriteFontValue(string Name)
            : base(Name)
        {
            this.Value = null;
            this.Path = "";
        }

        public SpriteFontValue(string Name, string Path)
            : base(Name)
        {
            this.Value = LoadSpriteFont(Path);
        }

        public SpriteFontValue(string Name, SpriteFont Value)
            : base(Name)
        {
            this.Value = Value;
            this.Path = "";
        }

        public SpriteFontValue(string Name, SpriteFont Value, string Path)
            : base(Name)
        {
            this.Value = Value;
            this.Path = Path;
        }

        public SpriteFont LoadSpriteFont(string Path)
        {
            this.Path = Path;
            if (Parent.CanLoad && !Path.Equals(""))
                return AssetManager.Load<SpriteFont>(Path);
            else
                return null;
        }

        public override void Load()
        {
            set(Path);
        }

        public SpriteFont get()
        {
            return Value;
        }

        public string getPath()
        {
            return Path;
        }

        public void set(string Path)
        {
            this.Value = LoadSpriteFont(Path);
            PerformEvent();
        }

        public void set(SpriteFont Value)
        {
            this.Value = Value;
            PerformEvent();
        }

        public void set(SpriteFont Value, string Path)
        {
            this.Value = Value;
            this.Path = Path;
            PerformEvent();
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new SpriteFontForm(Values);
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
