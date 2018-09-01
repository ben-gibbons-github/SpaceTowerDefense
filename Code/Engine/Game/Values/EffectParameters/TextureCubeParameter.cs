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
    public class TextureCubeParameter : BasicEffectParameter
    {
        private TextureCubeReference RefValue = null;
        private TextureCube Value = null;
        private string Path = "";

        public TextureCubeParameter(string Name)
            : base(Name)
        {

        }

        public TextureCubeParameter(EffectParameter Param)
            : base(Param)
        {

        }

        public string getPath()
        {
            return Path;
        }

        public TextureCube get()
        {
            return RefValue == null || RefValue.get() == null ? Value : RefValue.get();
        }

        public void set(string Path)
        {
            Load(Path);
            Que();
        }

        public override void UpdateParameter()
        {
            MyParameter.SetValue(get());
            base.UpdateParameter();
        }

        public void Load(string Path)
        {
            this.Path = Path;
            if (!Path.Equals("") && ParentValue != null)
            {
                DiscardReferenceValue();
                Value = AssetManager.Load<TextureCube>(Path);

                if (Value == null)
                {
                    RefValue = ParentValue.Parent.GetParent().getTextureCube(Path);
                    if (RefValue == null)
                        RefValue = ParentValue.Parent.ParentLevel.getTextureCube(Path);

                    if (RefValue != null)
                        RefValue.AddReference(this);
                }
            }
        }

        private void DiscardReferenceValue()
        {
            if (RefValue != null)
            {
                RefValue.RemoveReference(this);
                RefValue = null;
            }
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

        public override void PostRead()
        {
            if (get() == null)
                set(Path);
            base.PostRead();
        }
        
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<BasicEffectParameter> Values)
        {
            return new TextureCubeForm(Values);
        }
#endif
    }
}
