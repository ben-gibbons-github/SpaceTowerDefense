using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public enum EffectHolderType
    {
        None,
        Basic3D,
        Deferred3D,
        DeferredLight,
        Lit3D,
    }

    public class EffectHolder
    {
        public static EffectHolder ReturnHolderByType(EffectHolderType HolderType, Effect Value)
        {
            switch (HolderType)
            {
                case EffectHolderType.Basic3D: return new _3DEffect().Create(Value);
                case EffectHolderType.Deferred3D: return new Deferred3DEffect().Create(Value);
                case EffectHolderType.DeferredLight: return new DeferredLightEffect().Create(Value);
                case EffectHolderType.Lit3D: return new Lit3DEffect().Create(Value);
            }
            return null;
        }

        public EffectParameterCollection Collection;
        public Effect MyEffect;


        public EffectHolder Create(string EffectPath)
        {
            return Create(AssetManager.Load<Effect>(EffectPath));
        }

        public  EffectHolder Create(Effect effect)
        {
            this.MyEffect = effect;
            this.Collection = effect.Parameters;
            SetUp();
            return this;
        }

        public virtual void SetUp()
        {

        }

        public EffectTechnique FindTechnique(string Keyword)
        {
            foreach (EffectTechnique Technique in MyEffect.Techniques)
                if (Technique.Name.ToUpper().Contains(Keyword.ToUpper()))
                    return Technique;
            return MyEffect.Techniques[0];
        }

        public EffectParameter FindParameter(string Keyword)
        {
            foreach (EffectParameter Parameter in MyEffect.Parameters)
                if (Parameter.Name.ToUpper().Contains(Keyword.ToUpper()))
                    return Parameter;
            return MyEffect.Parameters[0];
        }

        public virtual void Apply()
        {
            MyEffect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
