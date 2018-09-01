using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FormSlider : BasicGameForm
    {
        public static Texture2D SliderBarTexture;
        public static Texture2D SliderCircleTexture;
        static int MaxChangeTime = 100;

        static FormSlider()
        {
            SliderBarTexture = AssetManager.Load<Texture2D>("_Engine/SliderBar");
            SliderCircleTexture = AssetManager.Load<Texture2D>("_Engine/SliderCircle");
        }

        public SpriteFontValue Font;
        public StringValue Text;
        public ColorValue MyColor;
        public FloatValue SizeMult;

        public FloatValue ChangeAmount;
        public FloatValue MinValue;
        public FloatValue MaxValue;

        public ObjectValue TiedObject;
        public StringValue TiedValue;

        public SliderArrow Arrow1;
        public SliderArrow Arrow2;

        public FloatValue SliderValue;
        int ChangeTime = 0;

        public override void SetValues(Vector2 Position, string Text, float Min, float Max, FloatValue SliderValue)
        {
            this.Text.set(Text);
            this.MinValue.set(Min);
            this.MaxValue.set(Max);
            this.SliderValue = SliderValue;

            base.SetValues(Position, Text, Min, Max, SliderValue);
        }

        public override void Create()
        {
            Arrow1 = new SliderArrow(this, 1, 0);
            Arrow2 = new SliderArrow(this, -1, (float)Math.PI);

            Font = new SpriteFontValue("Font", "Fonts/FormTextFont");
            Text = new StringValue("Text", "Text");
            MyColor = new ColorValue("Color", Vector4.One);
            SizeMult = new FloatValue("Size Mult", 1.5f);
            ChangeAmount = new FloatValue("Change Amount", 1);

            MinValue = new FloatValue("Min Value", -1000);
            MaxValue = new FloatValue("Max Value", 1000);

            TiedObject = new ObjectValue("Tied Object", typeof(GameObject));
            TiedValue = new StringValue("Tied Value");
            TiedObject.ChangeEvent = TieChange;
            TiedValue.ChangeEvent = TieChange;

            base.Create();

            Size.set(new Vector2(256, 32));
        }

        void TieChange()
        {
            if (TiedObject.get() != null && !TiedValue.get().Equals(""))
            {
                Value v = TiedObject.get().FindValue(TiedValue.get(), 3);
                if (v != null && v.GetType().Equals(typeof(FloatValue)))
                    SliderValue = (FloatValue)v;
            }
        }

        public override bool TriggerAsCurrent(BasicMarker m)
        {
            return true;
        }

        public override bool MarkerMove(Vector2 MoveAmount)
        {
            if (SliderValue != null && Math.Abs(MoveAmount.X) > 0.1f)
            {
                if (BasicMarker.MoveSound != null)
                    BasicMarker.MoveSound.Play(BasicMarker.MoveVolume, 0, 0);

                if (ChangeTime > MaxChangeTime)
                {
                    ChangeTime = 0;
                    SliderValue.add(ChangeAmount.get() * (MoveAmount.X > 0 ? 1 : -1));
                    if (SliderValue.get() > MaxValue.get())
                        SliderValue.set(MaxValue.get());
                    else if (SliderValue.get() < MinValue.get())
                        SliderValue.set(MinValue.get());
                }

                if (MoveAmount.X > 0)
                    Arrow1.FlashAlpha = 1;
                else
                    Arrow2.FlashAlpha = 1;

                return false;
            }

            return true;
        }

        public override void Update(GameTime gameTime)
        {
            ChangeTime += gameTime.ElapsedGameTime.Milliseconds;

            Arrow1.Update(gameTime);
            Arrow2.Update(gameTime);

            base.Update(gameTime);
        }

        public override void DrawAsForm(Vector2 Position, Vector2 Size)
        {
            Render.DrawSprite(HudItem.GlowTexture, Position, Size * 32, 0, Color.Black * (Alpha - 0.5f) * 2);

            Arrow1.Draw(Position, Size);
            Arrow2.Draw(Position, Size);

            Render.DrawShadowedText(Font.get(), Text.get(), Position + new Vector2(0, -Size.Y) - Font.get().MeasureString(Text.get()) / 2,
                Vector2.Zero, MyColor.getAsColor() * Alpha, Color.Black * Alpha);

            float MinX = Position.X - Size.X / 2 + Size.Y / 2;
            float MaxX = Position.X + Size.X / 2 - Size.Y / 2;
            float InterpolatedX = MinX;
            if (SliderValue != null)
                InterpolatedX += (MaxX - MinX) * (SliderValue.get() - MinValue.get()) / (MaxValue.get() - MinValue.get());

            Render.DrawSprite(SliderCircleTexture, Position, new Vector2(Size.Y / 2), 0, Color.Red * Alpha);
            Render.DrawSprite(SliderCircleTexture, new Vector2(InterpolatedX, Position.Y), new Vector2(Size.Y * 0.75f), 0, Color.White * Alpha);
            Render.DrawSprite(SliderBarTexture, Position, Size, 0, Color.White * Alpha);

            base.DrawAsForm(Position, Size);
        }
    }
}
