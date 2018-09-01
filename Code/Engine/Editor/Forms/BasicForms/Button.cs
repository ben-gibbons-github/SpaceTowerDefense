
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{

    public delegate void ClickEvent(Button button);

    public class Button : Form
    {
#if EDITOR && WINDOWS
        public Texture2D ButtonTexture;
        public Texture2D BackgroundTexture;
        public Rectangle ImageRectangle;

        public Vector2 TextPosition;

        public string Text;
        public bool HasText = false;
        public SpriteFont Font;

        public Color BorderColor = FormFormat.BorderColor;
        public Color BackgroundColor = FormFormat.BackgroundColor;
        public Color BackgroundHighlightColor = FormFormat.BackgroundHighlightColor;
        public Color TextColor = FormFormat.TextColor;

        public int ObjectMarginWidth = 4;
        public int ObjectMarginHeight = 0;
        public int DoubleClickTimer = 0;

        public bool HighLighted = false;
        public bool Selected = false;

        public object Tag;

        public ClickEvent Event;
        public ClickEvent DoubleClickEvent;

        public DropDownWindow DropWindow;
        public bool UseDoubleClick = false;

        public Button() { }

        public Button(String Text, SpriteFont Font)
        {
            SetUpWithText(Text, Font);
        }

        public Button(ClickEvent Event, String Text, SpriteFont Font)
        {
            this.Event = Event;
            SetUpWithText(Text, Font);
        }

        public Button(ClickEvent Event, Texture2D ButtonTexture, Vector2 Position)
        {
            ObjectMarginWidth = 2;
            ObjectMarginHeight = 2;

            this.Event = Event;
            this.ButtonTexture = ButtonTexture;
            SetPosition(Position);
            SetSize(new Vector2(ButtonTexture.Width + ObjectMarginWidth * 2, ButtonTexture.Height + ObjectMarginHeight * 2));
        }

        public Button(ClickEvent Event, Texture2D ButtonTexture, Vector2 IconSize, Vector2 Position)
        {
            ObjectMarginWidth = 2;
            ObjectMarginHeight = 2;

            this.Event = Event;
            this.ButtonTexture = ButtonTexture;
            SetPosition(Position);
            SetSize(new Vector2(IconSize.X + ObjectMarginWidth * 2, IconSize.Y + ObjectMarginHeight * 2));
        }


        public Button(ClickEvent Event, String Text, SpriteFont Font, Vector2 Position)
        {

            this.Event = Event;
            SetPosition(Position);
            SetUpWithText(Text, Font);
        }

        public void SetUpWithText(String Text, SpriteFont Font)
        {
            ObjectMarginWidth = 4;
            ObjectMarginHeight = 0;
            SetSize(new Vector2(ObjectMarginWidth * 2, ObjectMarginHeight * 2) + Font.MeasureString(Text));
            this.Text = Text;
            this.Font = Font;
            HasText = true;
            TextPosition = new Vector2(ObjectMarginWidth, ObjectMarginHeight) + Position;
        }

        public override void UpdateRectangles()
        {
            if (ButtonTexture != null)
            {
                ImageRectangle.X = (int)Position.X + ObjectMarginWidth;
                ImageRectangle.Y = (int)Position.Y + ObjectMarginHeight;

                ImageRectangle.Width = (int)Size.X - ObjectMarginWidth * 2;
                ImageRectangle.Height = (int)Size.Y - ObjectMarginHeight * 2;
            }

            if (HasText)
                TextPosition = new Vector2(ObjectMarginWidth, ObjectMarginHeight) + Position;

            base.UpdateRectangles();
        }

        public override void Create(FormHolder Parent)
        {

            if (BackgroundTexture == null)
                BackgroundTexture = Render.BlankTexture;

            base.Create(Parent);
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            base.Update(gameTime, Updater);
            if (ContainsMouse)
            {
                if (MouseManager.mouseState.LeftButton == ButtonState.Released)
                {
                    if (!HighLighted)
                    {
                        HighLighted = true;
                        Updater.NeedsToRedraw = true;
                    }

                    if (DoubleClickTimer > 0)
                    {
                        DoubleClickTimer = 0;
                            PerformEvent();
                    }
                }
                else
                {
                    if (MouseManager.MouseClicked)
                        MouseClick(Updater);

                    if (DoubleClickTimer > 0)
                    {
                        DoubleClickTimer--;
                        if (DoubleClickTimer < 1 && Event != null)
                            PerformEvent();
                    }
                }
            }
            else if (MouseManager.mouseState.LeftButton == ButtonState.Released && HighLighted)
            {
                HighLighted = false;
                Updater.NeedsToRedraw = true;
            }    
        }

        public void PerformEvent()
        {
            if (Event != null)
                Event(this);

            if (DropWindow != null)
                DropWindow.DestroyFlag = true;
        }

        public virtual void MouseClick(Window Updater)
        {
            if (UseDoubleClick)
            {
                if (DoubleClickTimer < 1)
                    DoubleClickTimer = 10;
            }
            else
                PerformEvent();
        }


        public override void Draw()
        {
            Game1.spriteBatch.Draw(BackgroundTexture, MyRectangleBorder, BorderColor);
            Game1.spriteBatch.Draw(BackgroundTexture, MyRectangle, HighLighted || Selected ? BackgroundHighlightColor : BackgroundColor);

            if (ButtonTexture != null)
                Game1.spriteBatch.Draw(ButtonTexture, ImageRectangle, Color.White);
            if (HasText)
                Game1.spriteBatch.DrawString(Font, Text, TextPosition, TextColor);


            base.Draw();
        }
#endif
    }

}
