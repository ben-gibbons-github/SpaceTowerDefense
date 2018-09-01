using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class FormButton : BasicGameForm
    {
        public SpriteFontValue Font;
        public StringValue Text;
        public ColorValue MyColor;
        public FloatValue SizeMult;
        public EventValue ButtonEvent;

        public float FlashAlpha;
        public float ErrorAlpha;

        public EventTrigger Event;
        public string TargetMenu;

        BasicMarker LastMarker;

        public override void SetValues(Vector2 Position, string Text, EventTrigger Event)
        {
            this.Position.set(Position);
            this.Text.set(Text);
            this.Event = Event;
        }

        public override void SetValues(Vector2 Position, string Text, string TargetMenu)
        {
            this.Position.set(Position);
            this.Text.set(Text);
            this.TargetMenu = TargetMenu;
        }
        
        public override bool TriggerEvent(EventType Event, string[] args)
        {
            if (Event == EventType.SendTo)
            {
                FormSection s = (FormSection)ParentScene.FindObject(args[0], typeof(FormSection));
                if (s == null)
                    return false;

                s.Visible.set(true);
                LastMarker.SetCurrentForm(s.GetCenterForm());

                return true;
            }

            return base.TriggerEvent(Event, args);
        }

        public override bool TriggerAsCurrent(BasicMarker m)
        {
            LastMarker = m;
            if (ButtonEvent.get().Equals(""))
            {
                if (Event == null && TargetMenu == null)
                {
                    ErrorAlpha = 1;
                    return false;
                }

                if (Event != null)
                {
                    bool Result = Event(this, m);

                    if (BasicMarker.SelectSound != null)
                        BasicMarker.SelectSound.Play(BasicMarker.SelectVolume, 0, 0);

                    if (Result)
                    {
                        FlashAlpha = 1;
                        return true;
                    }
                    else
                    {
                        ErrorAlpha = 1;
                        return false;
                    }
                }
                else if (ParentFrame != null)
                {
                    if (TargetMenu.Equals(""))
                        ParentFrame.DeActivate();
                    else
                        ParentFrame.Cycle(TargetMenu);

                    FlashAlpha = 1;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                ButtonEvent.Trigger();
                return true;
            }
        }

        public override void Create()
        {
            Font = new SpriteFontValue("Font", "Fonts/FormTextFont");
            Text = new StringValue("Text", "Text");
            MyColor = new ColorValue("Color", Vector4.One);
            SizeMult = new FloatValue("Size Mult", 1.5f);
            ButtonEvent = new EventValue("Event");

            Text.ChangeEvent = TextChange;
            Font.ChangeEvent = TextChange;

            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            ErrorAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
            FlashAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;

            base.Update(gameTime);
        }

        void TextChange()
        {
            if (Size != null && Font.get() != null)
                Size.set(Font.get().MeasureString(Text.get()) * SizeMult.get());
        }

        public override void DrawAsForm(Vector2 Position, Vector2 Size)
        {
            Render.DrawSprite(HudItem.GlowTexture, Position, Size * 3, 0, Color.Black * (Alpha - 0.5f));

            Render.DrawShadowedText(Font.get(), Text.get(), Position - Font.get().MeasureString(Text.get()) / 2,
                Vector2.Zero, MyColor.getAsColor() * Alpha, Color.Black * Alpha);

            Render.DrawOutlineRect(Position - Size / 2, Position + Size / 2, 2, Color.White * Alpha);

            if (FlashAlpha > 0)
                Render.DrawSolidRect(Position - Size / 2, Position + Size / 2, Color.White * FlashAlpha * Alpha);
            if (ErrorAlpha > 0)
                Render.DrawSolidRect(Position - Size / 2, Position + Size / 2, Color.Red * ErrorAlpha * Alpha);

            base.DrawAsForm(Position, Size);
        }
    }
}
