using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if EDITOR && WINDOWS
using BadRabbit.Carrot.ValueForms;
#endif
using System.IO;

namespace BadRabbit.Carrot.EffectParameters
{
    public class BasicEffectParameter
    {
#if EDITOR && WINDOWS
        public ValueForm LinkedForm;
#endif

        public static BasicEffectParameter ReturnParameter(byte Value, string Name)
        {
            switch (Value)
            {
                case 0: return new FloatParameter(Name);
                case 1: return new Vector2Parameter(Name);
                case 2: return new Vector3Parameter(Name);
                case 3: return new Vector4Parameter(Name);
                case 4: return new BoolParameter(Name);
                case 5: return new StringParameter(Name);
                case 6: return new Texture2DParameter(Name);
                case 7: return new TextureCubeParameter(Name);
                case 8: return new ColorParameter(Name);
                case 9: return new IntParameter(Name);
            }
            return null;
        }

        public static BasicEffectParameter ReturnParameter(EffectParameter param)
        {
            switch (param.ParameterType)
            {
                case EffectParameterType.Int32:
                    return new IntParameter(param);
                case EffectParameterType.Single:
                    {
                        switch (param.ColumnCount)
                        {
                            case 1: return new FloatParameter(param);
                            case 2: return new Vector2Parameter(param);
                            case 3: return new Vector3Parameter(param);
                            case 4:
                                {
                                    if (param.Name.ToUpper().Contains("COLOR"))
                                        return new ColorParameter(param);
                                    else
                                        return new Vector4Parameter(param);
                                }
                        }
                        return null;
                    } 

                case EffectParameterType.Bool: return new BoolParameter(param);
                case EffectParameterType.String: return new StringParameter(param);
                case EffectParameterType.Texture:
                case EffectParameterType.Texture2D: return new Texture2DParameter(param);
                case EffectParameterType.TextureCube: return new TextureCubeParameter(param); 
            }
            return null;
        }

        public static byte ReturnByteType(BasicEffectParameter Parameter)
        {
            Type t = Parameter.GetType();

            if (t.Equals(typeof(FloatParameter)))
                return 0;
            if (t.Equals(typeof(Vector2Parameter)))
                return 1;
            if (t.Equals(typeof(Vector3Parameter)))
                return 2;
            if (t.Equals(typeof(Vector4Parameter)))
                return 3;
            if (t.Equals(typeof(BoolParameter)))
                return 4;
            if (t.Equals(typeof(StringParameter)))
                return 5;
            if (t.Equals(typeof(Texture2DParameter)))
                return 6;
            if (t.Equals(typeof(TextureCubeParameter)))
                return 7;
            if (t.Equals(typeof(ColorParameter)))
                return 8;
            if (t.Equals(typeof(IntParameter)))
                return 9;
            return 0;
        }


        public EffectParameter MyParameter;
        public EffectValue ParentValue;
        public string Name;

        public BasicEffectParameter(EffectParameter MyParameter)
        {
            this.MyParameter = MyParameter;
            this.Name = MyParameter.Name;
        }

        public BasicEffectParameter(string Name)
        {
            this.Name = Name;
        }

        public void setParameter(EffectParameter MyParameter)
        {
            this.MyParameter = MyParameter;
            UpdateParameter();
        }

        public virtual void UpdateParameter()
        {
#if EDITOR
            Render.EffectUpdateCalls++;
#endif
        }

        public void Que()
        {
            if (MyParameter != null)
                ParentValue.Parent.Add(this);
        }

        public virtual void Load()
        {


        }

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

        public virtual Form GetForm(LinkedList<BasicEffectParameter> Values)
        {
            return null;
        }
    }
}
