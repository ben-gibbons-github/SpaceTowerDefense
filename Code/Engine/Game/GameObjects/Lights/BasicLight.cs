using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class BasicLight : Basic3DObject
    {
        public enum LightState
        {
            Alive,
            Dying,
            Dead,
            Reviving,
        }

        protected EffectValue MyEffect;
        protected EffectParameter ColorParameter;
        protected EffectParameter SpecularParameter;
        private Vector4 OldColor;
        private float OldSpecular;
        private float FlickerMult;
        protected LightState lightState = LightState.Alive;
        BoolValue Dead;
        FloatValue ChangeSpeed;

        public override void Create()
        {
            Dead = new BoolValue("Dead");
            Dead.ChangeEvent = ChangeDead;
            ChangeSpeed = new FloatValue("Change Speed", 0.01f);
            base.Create();
        }

        private void ChangeDead()
        {
#if EDITOR && WINDOWS
            if (!ParentLevel.LevelForEditing)
#endif
            {
                if (!Tags.Contains(GameObjectTag.Update))
                    AddTag(GameObjectTag.Update);

                if (Dead.get())
                {
                    lightState = LightState.Dying;
                    FlickerMult = 1;
                    if (OldColor == Vector4.Zero && ColorParameter != null)
                        OldColor = ColorParameter.GetValueVector4();
                }
                else
                {
                    lightState = LightState.Reviving;
                    FlickerMult = 0;
                }
            }
#if EDITOR && WINDOWS
            else
            {
                if (Dead.get())
                    lightState = LightState.Dead;
                else
                    lightState = LightState.Alive;
            }
#endif
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (ColorParameter != null)
                ColorParameter.SetValue(OldColor * FlickerMult);
            if (SpecularParameter != null)
                SpecularParameter.SetValue(OldSpecular * FlickerMult);

            base.Draw3D(camera, DrawTag);
        }

        public override void Update(GameTime gameTime)
        {
            switch (lightState)
            {
                case LightState.Alive:
                    RemoveTag(GameObjectTag.Update);
                    break;
                case LightState.Dead:
                    RemoveTag(GameObjectTag.Update);
                    break;
                case LightState.Dying:
                    FlickerMult -= ChangeSpeed.get() * gameTime.ElapsedGameTime.Milliseconds;
                    if (FlickerMult < 0)
                    {
                        lightState = LightState.Dead;
                        RemoveTag(GameObjectTag.Update);
                    }
                    break;
                case LightState.Reviving:
                    FlickerMult += ChangeSpeed.get() * gameTime.ElapsedGameTime.Milliseconds;
                    if (FlickerMult > 1)
                    {
                        lightState = LightState.Alive;
                        RemoveTag(GameObjectTag.Update);
                    }
                    break;
            }

            base.Update(gameTime);
        }

        public override bool TriggerEvent(EventType Event, string[] args)
        {
            if (MyEffect.get() == null)
                return false;
            else
            {
                if (SpecularParameter == null)
                {
                    SpecularParameter = MyEffect.findEffectParameter("Specular");
                    OldSpecular = SpecularParameter.GetValueSingle();
                }
                if (ColorParameter == null)
                {
                    ColorParameter = MyEffect.findEffectParameter("Color");
                    OldColor = ColorParameter.GetValueVector4();
                }
            }

            switch (Event)
            {
                case EventType.Kill:
                    Dead.set(true);
                    if (args.Count() > 0)
                    {
                        float f = Logic.ParseF(args[0]);
                        if (f != 0)
                            ChangeSpeed.set(f);
                    }
                    return true;
                case EventType.Revive:
                    Dead.set(false);
                    if (args.Count() > 0)
                    {
                        float f = Logic.ParseF(args[0]);
                        if (f != 0)
                            ChangeSpeed.set(f);
                    }
                    return true;
            }

            return base.TriggerEvent(Event, args);
        }
    }
}
