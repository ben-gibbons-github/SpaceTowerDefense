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
using BadRabbit.Carrot.EffectParameters;

namespace BadRabbit.Carrot
{
    public class EffectValue : Value
    {
        public static string[] IllegalParameters = 
        { "Time", "TextureSize", "CameraPosition",       
        "LightDistance", "LightPosition", "ShadowReference","Duration",
        "AmbientLightColor","LightOneDirection","LightTwoDirection",
        "LightOneColor","LightTwoColor"};

        public EffectHolder Holder;
        private EffectHolderType HolderType = EffectHolderType.None;
        private Effect Value;
        private string Path;
        public Dictionary<string, BasicEffectParameter> Parameters = new Dictionary<string, BasicEffectParameter>();


        public EffectValue(string Name, EffectHolderType HolderType)
            : base(Name)
        {
            this.HolderType = HolderType;
            this.Value = null;
            this.Path = "";
        }

        public EffectValue(string Name, string Path, EffectHolderType HolderType)
            : base(Name)
        {
            this.HolderType = HolderType;
            this.Value = LoadEffect(Path);
        }

        public EffectValue(string Name, Effect Value, EffectHolderType HolderType)
            : base(Name)
        {
            this.Value = Value;
            this.Path = "";
        }

        public EffectValue(string Name, Effect Value, string Path, EffectHolderType HolderType)
            : base(Name)
        {
            this.HolderType = HolderType;
            this.Value = Value;
            this.Path = Path;
        }

        public Effect LoadEffect(string Path)
        {
            this.Path = Path;
            if (Parent.CanLoad && !Path.Equals(""))
            {
                try
                {
                    return AssetManager.LoadEffect(Parent.GetParent().EffectDirectory.get() + Path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
            else
                return null;
        }

        public override void Load()
        {
            set(Path);
        }

        public Effect get()
        {
            return Value;
        }

        public EffectTechnique getTechnique(string Name)
        {
            if (Value != null)
                return Value.Techniques[Name];
            else
                return null;
        }

        public BasicEffectParameter findValueParameter(string Name)
        {
            if (Value != null)
                foreach (BasicEffectParameter param in Parameters.Values)
                    if (param.Name.ToUpper().Contains(Name.ToUpper()))
                        return param;
            return null;
        }

        public EffectParameter findEffectParameter(string Name)
        {
            if (Value != null)
                foreach (EffectParameter param in Value.Parameters)
                    if (param.Name.ToUpper().Contains(Name.ToUpper()))
                        return param;
            return null;
        }

        public void setTechnique(EffectTechnique technique)
        {
            if (Value != null)
                Value.CurrentTechnique = technique;
        }

        public string getPath()
        {
            return Path;
        }

        public string getFullPath()
        {
            return Parent.GetParent().EffectDirectory.get() + Path;
        }

        public void set(string Path)
        {
            this.Value = LoadEffect(Path);
            Perform();
        }

        public void set(Effect Value)
        {
            this.Value = Value;
            Perform();
        }

        public void set(Effect Value, string Path)
        {
            this.Value = Value;
            this.Path = Path;
            Perform();
        }

        public void Perform()
        {
            if (Value != null)
            {
                if (HolderType != EffectHolderType.None)
                    Holder = EffectHolder.ReturnHolderByType(HolderType, Value);
                Parent.AddConstructEffect(this);
            } 
            PerformEvent();
        }
#if EDITOR && WINDOWS
        public override Form GetForm(LinkedList<Value> Values)
        {
            return new EffectForm(Values);
        }
#endif
        public override void Write(BinaryWriter Writer)
        {
            Writer.Write((Int32)Parameters.Values.Count);

            foreach (BasicEffectParameter param in Parameters.Values)
            {
                Writer.Write(BasicEffectParameter.ReturnByteType(param));
                Writer.Write(param.Name);
                param.Write(Writer);
            }

            Writer.Write(getPath());

            base.Write(Writer);
        }

        public override void PostRead()
        {
            foreach (BasicEffectParameter p in Parameters.Values)
                p.PostRead();
            base.PostRead();
        }

        public override void Destroy()
        {
            foreach (BasicEffectParameter p in Parameters.Values)
                p.Destroy();
            
            base.Destroy();
        }
        

        public override void Read(BinaryReader Reader)
        {
            int ParameterCount = Reader.ReadInt32();

            for (int i = 0; i < ParameterCount; i++)
            {
                byte ParamType = Reader.ReadByte();
                string ParamName = Reader.ReadString();
                BasicEffectParameter param = null;
                if (!Parameters.ContainsKey(ParamName))
                {
                    param = BasicEffectParameter.ReturnParameter(ParamType, ParamName);
                    param.Read(Reader);
                    Add(param);
                }
                else
                {
                    Parameters[ParamName].Read(Reader);
                }
            }

            set(Reader.ReadString());

            base.Read(Reader);
        }

        public static void DummyRead(BinaryReader Reader)
        {
            int ParameterCount = Reader.ReadInt32();
            for (int i = 0; i < ParameterCount; i++)
            {
                byte ParamType = Reader.ReadByte();
                string ParamName = Reader.ReadString();
                BasicEffectParameter.ReturnParameter(ParamType, ParamName).Read(Reader);
            }
            Reader.ReadString();
        }

        public void ConstructParameters()
        {
            if (Value != null)
            {
                LinkedList<BasicEffectParameter> DestroyValues = new LinkedList<BasicEffectParameter>(Parameters.Values);

                foreach (EffectParameter param in Value.Parameters)
                    if (!IllegalParameters.Contains(param.Name) && param.RowCount < 2 && !param.Name.ToUpper().StartsWith("NOEDIT"))
                    {
                        if (!Parameters.ContainsKey(param.Name))
                            Add(BasicEffectParameter.ReturnParameter(param));
                        else
                        {
                            BasicEffectParameter p = Parameters[param.Name];
                            p.setParameter(param);
                            DestroyValues.Remove(p);
                        }
                    }
                foreach (BasicEffectParameter Parameter in DestroyValues)
                    Remove(Parameter);


#if EDITOR && WINDOWS
                if (Parent.EditorSelected)
                    Parent.ParentLevel.MyScene.UpdateSelected();
#endif
            }
        }

        private void Add(BasicEffectParameter Param)
        {
            if (Param != null)
            {
                Param.ParentValue = this;
                Parameters.Add(Param.Name, Param);
            }
        }

        private void Remove(BasicEffectParameter Param)
        {
            Parameters.Remove(Param.Name);
        }

        private void Clear()
        {
            Parameters.Clear();
        }
    }
}
