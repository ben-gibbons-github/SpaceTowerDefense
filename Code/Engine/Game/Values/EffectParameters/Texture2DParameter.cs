using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif

namespace BadRabbit.Carrot.EffectParameters
{
    public class Texture2DParameter : BasicEffectParameter
    {
        private Texture2D Value = null;
        private string Path = "";
       

        public Texture2DParameter(string Name)
            : base(Name)
        {
            
        }

        public Texture2DParameter(EffectParameter Param)
            : base(Param)
        {
            
        }

        public string getFullPath()
        {
            return ParentValue.Parent.GetParent().TextureDirectory.get() + Path;
        }

        public string getPath()
        {
            return Path;
        }

        public Texture2D get()
        {
            return Value != null ? this.Value : Render.BlankTexture;
        }

        public void set(string Path)
        {
            Value = LoadTexture(Path);
            Que();
        }

        public Texture2D LoadTexture(string Path)
        {
            this.Path = Path;
            if (MyParameter != null && !Path.Equals(""))
                return AssetManager.Load<Texture2D>(ParentValue.Parent.GetParent().TextureDirectory.get() + Path);
            else
                return null;
        }

        public override void UpdateParameter()
        {
            if (Value == null || Value.IsDisposed)
                Value = LoadTexture(Path);
            MyParameter.SetValue(get());
            base.UpdateParameter();
        }

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
        
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<BasicEffectParameter> Values)
        {
            return new Texture2DForm(Values);
        }
#endif
    }
}
