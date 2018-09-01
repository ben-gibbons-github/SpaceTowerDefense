using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class TechTreeGroup
    {
        public static Vector2 CellBorders = new Vector2(150);
        public static Vector2 CellSize = new Vector2(128);

        Vector2 BumperCursor;
        Vector2 Cursor;
        TechTreeMenu ParentMenu;
        Vector2 TargetPosition;
        Vector2 DrawOffset;
        float Alpha;
        FactionCard HighlightedCard;
        List<FactionCard> Cards;
        LinkedListNode<TechTreeGroup> MyNode;

        public Vector2 Position, Size;
        private static float OffsetSpeed = 0.1f;

        public TechTreeGroup(List<FactionCard> Cards)
        {
            this.Cards = Cards;
        }

        public void SetNode(LinkedListNode<TechTreeGroup> MyNode, TechTreeMenu ParentMenu)
        {
            this.ParentMenu = ParentMenu;
            this.MyNode = MyNode;

            Size.Y = CellBorders.Y;
            Size.X = Math.Max(Size.X, Cards.Count * CellBorders.X);
        }

        public void SwitchTo()
        {
            Vector2 TargetOffset = -new Vector2((Cursor.X - 2) * CellBorders.X, -CellBorders.Y / 2);
            DrawOffset = TargetOffset;

            foreach (FactionCard c in Cards)
                c.MenuReset();
        }

        public void Update(GameTime gameTime, BasicController MyController, bool Highlighted)
        {
            if (Highlighted && !ParentMenu.Closing)
            {
                TargetPosition = ParentMenu.Position - TechTreeMenu.NegativeOffset - new Vector2(0, Size.Y / 2);
                Position += (TargetPosition - Position) * gameTime.ElapsedGameTime.Milliseconds * PlayerMenu.CloseOpenSpeed * 2;
                Alpha = Math.Min(1, Alpha + gameTime.ElapsedGameTime.Milliseconds * PlayerMenu.CloseOpenSpeed * 2);
                if (Alpha > 0.5f)
                {
                    Vector2 CursorPrevious = Cursor;
                    bool ExtraSticks = FactionManager.GetFaction(ParentMenu.ParentShip.FactionNumber).PickingCards;
                    Cursor += MyController.MenuStick(ExtraSticks, ExtraSticks, true, false, false);

                    if (ExtraSticks)
                    {
                        Vector2 BumperCursorPrevious = BumperCursor;
                        BumperCursor = MyController.MenuStick(false, false, false, true, true);
                        if (Math.Abs(BumperCursor.X) > 0.1f && Math.Abs(BumperCursor.X) < 0.1f)
                        {
                            Faction f= FactionManager.GetFaction(ParentMenu.ParentShip.FactionNumber);
                            if (BumperCursor.X > 0)
                            {
                                f.CardPickPosition++;
                                if (f.CardPickPosition == f.Cards.Count)
                                    ParentMenu.ParentShip.ReadyMenu();
                            }
                            else
                                f.CardPickPosition = Math.Max(0, f.CardPickPosition - 1);
                        }
                    }

                    if (Cursor.Y != 0 && ParentMenu.MyFaction.PickingCards)
                    {
                        if (Cursor.Y < 0)
                        {
                            TechTreeGroup PreviousGroup = MyNode.Previous != null ? MyNode.Previous.Value : MyNode.List.Last.Value;
                            if (PreviousGroup.Cards != null && PreviousGroup.Cards.Count > 0)
                            {
                                PreviousGroup.Cursor.X = Cursor.X;
                                PreviousGroup.Position = Position - new Vector2(0, PreviousGroup.Size.Y);
                                ParentMenu.SetGroup(PreviousGroup);
                                PreviousGroup.SwitchTo();
                            }
                        }
                        else if (Cursor.Y > 0)
                        {
                            TechTreeGroup NextGroup = MyNode.Next != null ? MyNode.Next.Value : MyNode.List.First.Value;
                            if (NextGroup.Cards != null && NextGroup.Cards.Count > 0)
                            {
                                NextGroup.Cursor.X = Cursor.X;
                                NextGroup.Position = Position + new Vector2(0, Size.Y);
                                ParentMenu.SetGroup(NextGroup);
                                NextGroup.SwitchTo();
                            }
                        }
                        Cursor.Y = 0;
                    }
                    Cursor.X = MathHelper.Clamp(Cursor.X, 0, Cards.Count - 1);

                    int i = 0;
                    foreach (FactionCard c in Cards)
                        c.MenuUpdate(gameTime, (int)Cursor.X == i, 1 - Math.Abs(Cursor.X - i++) / 2.25f);
                    
                    Vector2 TargetOffset = -new Vector2((Cursor.X - 2) * CellBorders.X, -CellBorders.Y / 2f);
                    DrawOffset += (TargetOffset - DrawOffset) * OffsetSpeed;
                    
                    HighlightedCard = Cards[(int)Cursor.X];
                    if (MyController.AButton() && !MyController.AButtonPrevious())
                    {
                    }
                    HighlightedCard.Update3D(ParentMenu.ParentShip);
                }
            }
            else
                Alpha = Math.Max(0, Alpha - gameTime.ElapsedGameTime.Milliseconds * PlayerMenu.CloseOpenSpeed);
        }

        private void Draw(Vector2 Position)
        {
            if (Alpha < 0.01f)
                return;
            int x = 0;
            foreach (FactionCard c in Cards)
                c.DrawTechTree(Position + DrawOffset + new Vector2(x++ * CellBorders.X, 0), Alpha * ParentMenu.Alpha, ParentMenu.ParentShip);
        }

        public void Draw3D(Camera3D DrawCamera)
        {
            if (HighlightedCard != null)
                HighlightedCard.Draw3D(DrawCamera, ParentMenu.ParentShip);
        }

        public void DrawAll()
        {
            TechTreeGroup PreviousGroup = MyNode.Previous != null ? MyNode.Previous.Value : MyNode.List.Last.Value;
            TechTreeGroup NextGroup = MyNode.Next != null ? MyNode.Next.Value : MyNode.List.First.Value;

            if (ParentMenu.MyFaction.PickingCards)
                PreviousGroup.Draw(Position - new Vector2(0, PreviousGroup.Size.Y));

            Draw(Position);

            if (ParentMenu.MyFaction.PickingCards)
                NextGroup.Draw(Position + new Vector2(0, Size.Y));
        }
    }
}
