#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace BadRabbit.Carrot
{
    public delegate void EnterEvent();
    public delegate void DragEvent(GameObject Dragged);

    public class TextField : Form
    {
        public static string ClipBoardString = "";

        KeyboardState KeyBoardState;
        KeyboardState PreviousKeyBoardState;

        public EnterEvent Event;
        public DragEvent DraggedEvent;

        public Color BorderColor = FormFormat.BorderColor;
        public Color BackgroundColor = FormFormat.BackgroundColor;
        public Color BackgroundHighlightColor = FormFormat.BackgroundHighlightColor;
        public Color TextColor = FormFormat.TextColor;
        public Color HighlightColor = new Color(0.5f, 0.5f, 1);

        public int StartCursor = 0;
        public int EndCursor = 0;
        public int ClickPlace = 0;

        public int LifeTime = 0;
        public int MaxChars = 0;

        public bool IsSelected = false;
        public bool IsSelecting = false;
        public bool IsSelectedPrevious = false;
        public bool IsFlipped = true;
        public bool FirstTime = true;
        public bool NumbersOnly = false;
        public bool AlwaysHighlight = false;

        public int LeftTime = 0;
        public int RightTime = 0;
        public int DeleteTime = 0;
        public static int MaxTime = 15;

        SpriteFont Font;

        public int FlipTimer;

        public static int Bounders = 10;

        public string Text = "";

        public int StartingWidth = 0;

        public override void Create(FormHolder Parent)
        {
            base.Create(Parent);
        }

        public void SetText(string Text)
        {
            if (MaxChars > 0)
                this.Text = Text.Substring(0, Math.Min(Text.Length, MaxChars));
            else
                this.Text = Text;
        }

        public TextField(Vector2 Position, int Width, SpriteFont Font, string Text, EnterEvent Event)
        {
            StartCursor = 0;
            EndCursor = Text.Count();

            this.Text = Text;
            this.Font = Font;
            this.SetPosition(Position);
            this.Event = Event;

            StartingWidth = Width;
            SetSize(new Vector2(Width, Font.MeasureString("Ajsdfklf523%#").Y));

            MaxChars = 300;

        }

        public int FindMouseOnString(Vector2 MousePos)
        {
            int Place = 0;

            float Xpos = MyRectangle.X + 8;


            for (int i = 0; i < Text.Count(); i++)
            {
                Xpos += Font.MeasureString(Text[i].ToString()).X;

                if (MousePos.X > Xpos)
                    Place++;
            }

            return Place;
        }

        public string getSelectedText()
        {
            if (StartCursor > EndCursor)
                return this.Text.Substring(StartCursor, StartCursor - EndCursor);
            else
                return null;
        }

        public float ReturnNumber()
        {

            float Number = 0;
            int StartingPower = 0;
            string NewText = string.Empty;

            bool IsNegative = false;




            for (int i = 0; i < Text.Count(); i++)
                if (StartingPower == 0)
                {
                    Char chr = Text[i];
                    if (chr == '.')
                        StartingPower = -((Text.Count() - 1) - i);
                }

            for (int i = 0; i < Text.Count(); i++)
            {
                Char chr = Text[i];
                if (chr == '-')
                    IsNegative = true;
                else if (Char.IsDigit(chr))
                    NewText += chr;

            }

            for (int i = NewText.Count() - 1; i > -1; i--)
            {
                Char chr = NewText[i];
                Number += CompareCharToNumber(chr) * (float)Math.Pow(10, StartingPower);
                StartingPower++;
            }

            if (IsNegative)
                Number = -Number;

            return Number;

        }

        public int CompareCharToNumber(Char chr)
        {
            int Numb = 0;

            for (int i = 0; i < 10; i++)
                if (chr == i.ToString()[0])
                    Numb = i;

            return Numb;
        }

        public bool CleanString()
        {
            bool ContainsPeroid = false;

            string NewText = string.Empty;

            for (int i = 0; i < Text.Count(); i++)
            {
                Char chr = Text[i];

                if (chr == '.')
                {
                    if (!ContainsPeroid)
                    {
                        NewText += chr;
                        ContainsPeroid = true;
                    }
                }
                else if (chr == '-')
                {
                    if (i == 0)
                        NewText += chr;
                }
                else
                    NewText += chr;

            }
            Text = NewText;

            return ContainsPeroid;
        }

        public string ReturnTextFromClipBoard()
        {
            System.Windows.Forms.IDataObject iData = System.Windows.Forms.Clipboard.GetDataObject();

            if (iData.GetDataPresent(System.Windows.Forms.DataFormats.Text))
                return (String)iData.GetData(System.Windows.Forms.DataFormats.Text);
            else
                return null;
        }

        public override void UpdateRectangles()
        {
            base.UpdateRectangles();

        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            if (ContainsMouse)
                ContainsMouseAction(Updater);
            else
                DoesNotContainMouse();

            LifeTime++;

            IsSelectedPrevious = IsSelected;

            if (!IsSelected)
            {
                if (NumbersOnly)
                {
                    CleanString();
                }
            }

            if (Text.Count() > MaxChars && NumbersOnly)
            {
                int lenght = MaxChars;
                if (Text[0] == '-')
                    lenght++;
                Text = Text.Substring(0, MaxChars);
            }

            FlipTimer++;

            if (FlipTimer > 20)
            {
                FlipTimer = 0;
                IsFlipped = !IsFlipped;
            }

            if (IsSelected)
            {
                Updater.NeedsToRedraw = true;
                if (IsSelecting)
                {
                    if (Mouse.GetState().LeftButton.Equals(ButtonState.Released))
                        IsSelecting = false;
                    else
                    {
                        int NewPlace = FindMouseOnString(Updater.RelativeMousePosition);

                        if (NewPlace > ClickPlace)
                            EndCursor = NewPlace;
                        if (NewPlace < ClickPlace)
                            StartCursor = NewPlace;
                    }
                }

                PreviousKeyBoardState = KeyboardManager.keyboardStatePrevious;

                KeyBoardState = KeyboardManager.keyboardState;

                string PressedKeys = "";
                bool Changed = false;
                bool Paste = false;

                foreach (Keys Key in Enum.GetValues(typeof(Keys)))
                    if (KeyBoardState.IsKeyDown(Key) && PreviousKeyBoardState.IsKeyUp(Key))
                        if (Key.ToString().Count() < 3)
                        {
                            {
                                Changed = true;
                                PressedKeys += Key.ToString();

                                PressedKeys = PressedKeys.ToLower();
                                if (KeyBoardState.IsKeyDown(Keys.LeftShift) || KeyBoardState.IsKeyDown(Keys.RightShift))
                                    PressedKeys = PressedKeys.ToUpper();
                            }
                        }

                if ((KeyBoardState.IsKeyDown(Keys.Subtract) && PreviousKeyBoardState.IsKeyUp(Keys.Subtract)) || KeyBoardState.IsKeyDown(Keys.OemMinus)  && PreviousKeyBoardState.IsKeyUp(Keys.OemMinus))
                {
                    Changed = true;
                    PressedKeys = KeyBoardState.IsKeyDown(Keys.LeftShift) || KeyBoardState.IsKeyDown(Keys.RightShift) ? "_" : "-";
                }

                if (KeyBoardState.IsKeyDown(Keys.LeftShift) || KeyBoardState.IsKeyDown(Keys.RightShift))
                {
                    if (KeyBoardState.IsKeyDown(Keys.D9) && PreviousKeyBoardState.IsKeyUp(Keys.D9))
                    {
                        Changed = true;
                        PressedKeys = "(";
                    }

                    if (KeyBoardState.IsKeyDown(Keys.D0) && PreviousKeyBoardState.IsKeyUp(Keys.D0))
                    {
                        Changed = true;
                        PressedKeys = ")";
                    }
                }

                if (KeyBoardState.IsKeyDown(Keys.OemComma) && PreviousKeyBoardState.IsKeyUp(Keys.OemComma))
                {
                    Changed = true;
                    PressedKeys += ',';
                }

                if (KeyBoardState.IsKeyDown(Keys.OemPeriod) && PreviousKeyBoardState.IsKeyUp(Keys.OemPeriod))
                {
                    Changed = true;
                    PressedKeys += '.';
                }

                if (!NumbersOnly)
                {
                    if (KeyBoardState.IsKeyDown(Keys.Space) && PreviousKeyBoardState.IsKeyUp(Keys.Space))
                    {
                        Changed = true;
                        PressedKeys += " ";
                    }

                    if (KeyBoardState.IsKeyDown(Keys.OemQuestion) && PreviousKeyBoardState.IsKeyUp(Keys.OemQuestion))
                    {
                        Changed = true;
                        PressedKeys += "/";
                    }
                }

                if (!Paste)
                {
                    if (PressedKeys.Count() > 1)
                        PressedKeys = PressedKeys[PressedKeys.Count() - 1].ToString();

                    if (NumbersOnly && PressedKeys.Count() > 0)
                        if (!Char.IsDigit(PressedKeys[0]) && PressedKeys[0] != '.' && PressedKeys[0] != '-')
                            PressedKeys = "";
                }

                if ((KeyBoardState.IsKeyDown(Keys.LeftControl) || KeyBoardState.IsKeyDown(Keys.RightControl)) && (KeyBoardState.IsKeyDown(Keys.C) || KeyBoardState.IsKeyDown(Keys.V)))
                {
                    if (KeyBoardState.IsKeyDown(Keys.C))
                    {
                        if (getSelectedText() != null)
                            ClipBoardString = getSelectedText();
                    }
                    else
                    {
                        PressedKeys = ClipBoardString;
                    }
                }

                if (Changed && PressedKeys != "")
                {
                    FirstTime = false;

                    string NewText = "";

                    try
                    {
                        for (int i = 0; i < StartCursor; i++)
                            NewText += Text[i];
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Exception n = e;
                        Console.WriteLine(e.Message);
#endif
                    }

                    NewText += PressedKeys;

                    int count = StartCursor + PressedKeys.Count();



                    for (int i = EndCursor; i < Text.Count(); i++)
                        NewText += Text[i];

                    StartCursor = count;
                    EndCursor = count;

                    Text = NewText;
                    if (Event != null)
                        Event();
                }
                if (Text.Count() > 0)
                {
                    if (KeyBoardState.IsKeyDown(Keys.Back))
                    {
                        FirstTime = false;

                        DeleteTime++;
                        if (PreviousKeyBoardState.IsKeyUp(Keys.Back) || DeleteTime > MaxTime)
                        {
                            DeleteTime -= 3;

                            string NewText = "";

                            {

                                if (EndCursor == StartCursor)
                                    StartCursor -= 1;

                                for (int i = 0; i < StartCursor; i++)
                                    NewText += Text[i];

                                for (int i = EndCursor; i < Text.Count(); i++)
                                    NewText += Text[i];

                                EndCursor = StartCursor;
                            }
                            Text = NewText;

                            if (Event != null)
                                Event();
                        }

                    }
                    else
                        DeleteTime = 0;
                }
                else
                    DeleteTime = 0;

                if (KeyBoardState.IsKeyDown(Keys.Right))
                {
                    RightTime++;
                    if (PreviousKeyBoardState.IsKeyUp(Keys.Right) || RightTime > 20)
                    {
                        RightTime -= 3;

                        StartCursor++;
                        EndCursor++;
                        if (KeyBoardState.IsKeyDown(Keys.LeftShift) || KeyBoardState.IsKeyDown(Keys.RightShift))
                            StartCursor--;
                    }
                }
                else
                    RightTime = 0;

                if (KeyBoardState.IsKeyDown(Keys.Left))
                {
                    LeftTime++;
                    if (PreviousKeyBoardState.IsKeyUp(Keys.Left) || LeftTime > 20)
                    {
                        LeftTime -= 3;

                        StartCursor--;
                        EndCursor--;
                        if (KeyBoardState.IsKeyDown(Keys.LeftShift) || KeyBoardState.IsKeyDown(Keys.RightShift))
                            EndCursor++;
                    }
                }
                else
                    LeftTime = 0;

                if (KeyBoardState.IsKeyDown(Keys.Left) || KeyBoardState.IsKeyDown(Keys.Right))
                    if (FirstTime)
                    {

                        FirstTime = false;
                        if (KeyBoardState.IsKeyDown(Keys.Left))
                        {
                            StartCursor = 0;
                            EndCursor = 0;
                        }
                        else
                        {
                            StartCursor = Text.Count();
                            EndCursor = Text.Count();
                        }
                    }

                if (KeyBoardState.IsKeyDown(Keys.Enter) && PreviousKeyBoardState.IsKeyUp(Keys.Enter))
                {
                    IsSelected = false;
                    Event();

                }
                StartCursor = (int)MathHelper.Clamp(StartCursor, 0, Text.Count());
                EndCursor = (int)MathHelper.Clamp(EndCursor, 0, Text.Count());

            }

            base.Update(gameTime, Updater);
        }


        public int GetPlaceOnString(int Place)
        {
            float Xpos = 0;

            for (int i = 0; i < Math.Min(Text.Count(), Place); i++)
            {
                Xpos += Font.MeasureString(Text[i].ToString()).X;

            }

            return (int)Xpos;
        }


        public void ContainsMouseAction(Window Updater)
        {

            if (MouseManager.MouseClicked)
            {
                TextColor = Color.Black;

                FirstTime = false;
                IsSelecting = true;
                IsSelected = true;

                ClickPlace = FindMouseOnString(Updater.RelativeMousePosition);
                StartCursor = ClickPlace;
                EndCursor = ClickPlace;


                if (Text.Count() < 2)
                {
                    StartCursor = 0;
                    EndCursor = Text.Count();
                }
            }
            if (MouseManager.DraggedObject == null && MouseManager.PreviousDraggedObject != null && MouseManager.PreviousDraggedObject.GetType().IsSubclassOf(typeof(GameObject)))
            {
                if (DraggedEvent != null)
                    DraggedEvent((GameObject)MouseManager.PreviousDraggedObject);
            }

        }

        public void DoesNotContainMouse()
        {
            if (LifeTime > 5)
                if (MouseManager.MouseClicked)
                {
                    if (IsSelected)
                    {
                        IsSelected = false;
                    }
                }
        }

        public override void Draw()
        {

            MyRectangle.Width = Math.Max(StartingWidth, (int)Font.MeasureString(Text).X + Bounders * 2);
            MyRectangleBorder.Width = MyRectangle.Width + 2;

            Game1.spriteBatch.Draw(Render.BlankTexture, MyRectangleBorder, BorderColor);
            Game1.spriteBatch.Draw(Render.BlankTexture, MyRectangle, BackgroundColor);



            if (IsSelected)
            {

                if (StartCursor != EndCursor)
                {
                    Rectangle Rect = new Rectangle(MyRectangle.X + GetPlaceOnString(StartCursor) + Bounders, MyRectangle.Y, GetPlaceOnString(EndCursor) - GetPlaceOnString(StartCursor), MyRectangle.Height);

                    Game1.spriteBatch.Draw(Render.BlankTexture, Rect, HighlightColor);
                }
            }


            Vector2 TextDrawPos = new Vector2(MyRectangle.X + Bounders, MyRectangle.Y + MyRectangle.Height / 2);
            TextDrawPos.Y -= Font.MeasureString(Text).Y / 2;

            Game1.spriteBatch.DrawString(Font, Text, TextDrawPos, TextColor);

            if (IsSelected)
            {

                if (StartCursor == EndCursor)
                {
                    int LineX = MyRectangle.X + GetPlaceOnString(StartCursor) + Bounders + 1;
                    Game1.spriteBatch.Draw(Render.BlankTexture, new Rectangle(LineX, MyRectangle.Y, 1, MyRectangle.Height), IsFlipped ? Color.Blue : Color.Aquamarine);

                }
            }


            base.Draw();
        }


    }
}
#endif