#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class Slider : Form
    {
        public float MaxValue;
        public float MinValue;
        public float CurrentValue;
        public bool Selected = false;
        public bool Highlighted = false;

        public Rectangle SliderRectangle;
        public Rectangle SliderBorder;
        public Vector2 SliderPosition;
        public int SliderWidth = 4;

        public Color BorderColor = FormFormat.TextColor;
        public Color BackgroundColor = Color.DarkGray;
        public Color BackgroundHighlightColor = Color.LightGray;
        public Texture2D BackgroundTexture;

        public EnterEvent Event;
        public bool NoValue = false;

        public Slider(Vector2 Position, Vector2 Size, float MaxValue, float MinValue, bool NoValue, EnterEvent Event)
        {
            this.MaxValue = MaxValue;
            this.MinValue = MinValue;
            this.SetPosition(Position);
            this.SetSize(Size);
            this.Event = Event;
            BackgroundTexture = Render.BlankTexture;
            this.NoValue = NoValue;
            SetValue(CurrentValue);
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            if ((Selected || ContainsMouse) && (MouseManager.DraggedObject == null || MouseManager.DraggedObject == this))
            {
                Highlighted = true;
                if (MouseManager.mouseState.LeftButton == ButtonState.Pressed || Selected)
                {
                    MouseManager.DraggedObject = this;
                    SetValue(MinValue + (Updater.RelativeMousePosition.X - Position.X) / Size.X * (MaxValue - MinValue));
                    Selected = true;

                    if (Event != null)
                        Event();
                }
            }
            else
                Highlighted = false;
            if (Selected && MouseManager.mouseState.LeftButton != ButtonState.Pressed)
            {
                Selected = false;
                Updater.NeedsToRedraw = true;
            }

            base.Update(gameTime, Updater);
        }

        public float GetValue()
        {
            return CurrentValue;
        }

        public void SetValue(float NewValue)
        {
            NoValue = false;
            NewValue = MathHelper.Clamp(NewValue, MinValue, MaxValue);
            this.CurrentValue = NewValue;

            UpdateRectangles();
        }

        public override void SetPosition(Vector2 Position)
        {
            base.SetPosition(Position);
            UpdateRectangles();
        }

        public override void UpdateRectangles()
        {
            SliderPosition = Position + new Vector2((CurrentValue - MinValue) / (MaxValue - MinValue) * Size.X - SliderWidth / 2, 0);

            SliderRectangle.X = (int)SliderPosition.X;
            SliderRectangle.Y = (int)SliderPosition.Y;
            SliderRectangle.Width = SliderWidth;
            SliderRectangle.Height = (int)Size.Y;

            SliderBorder.X = SliderRectangle.X - 1;
            SliderBorder.Y = SliderRectangle.Y - 1;
            SliderBorder.Width = SliderRectangle.Width + 2;
            SliderBorder.Height = SliderRectangle.Height + 2;

            base.UpdateRectangles();
        }

        public override void Draw()
        {
            Game1.spriteBatch.Draw(BackgroundTexture, MyRectangleBorder, BorderColor);
            Game1.spriteBatch.Draw(BackgroundTexture, MyRectangle, BackgroundColor);

            if (!NoValue)
            {
                Game1.spriteBatch.Draw(BackgroundTexture, SliderBorder, BorderColor);
                Game1.spriteBatch.Draw(BackgroundTexture, SliderRectangle, Highlighted || Selected ? BackgroundHighlightColor : BackgroundColor);
            }

            base.Draw();
        }



    }
}
#endif