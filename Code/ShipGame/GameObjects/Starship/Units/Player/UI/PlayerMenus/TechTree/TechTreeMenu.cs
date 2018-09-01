using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class TechTreeMenu : PlayerMenu
    {
        public Faction MyFaction;
        private LinkedList<TechTreeGroup> Groups;
        private TechTreeGroup CurrentGroup;
        bool CardPickingMode = true;

        public static Vector2 NegativeOffset = TechTreeGroup.CellBorders * new Vector2(2.5f, 0);

        public TechTreeMenu(PlayerShip ParentShip)
            : base(ParentShip)
        {
            MyFaction = FactionManager.GetFaction(ParentShip.FactionNumber);

            Groups = new LinkedList<TechTreeGroup>();
            AddGroup(new TechTreeGroup(FactionCard.SortedTurretDeck[0]));
            AddGroup(new TechTreeGroup(FactionCard.SortedTurretDeck[1]));
            AddGroup(new TechTreeGroup(FactionCard.SortedTurretDeck[2]));
        }

        public void SetForGameSelection()
        {
            Groups.Clear();
            TechTreeGroup group = new TechTreeGroup(FactionManager.GetFaction(ParentShip.FactionNumber).Cards);
            group.SetNode(Groups.AddLast(group), this);
            CurrentGroup = group;
            CardPickingMode = false;
        }

        private void AddGroup(TechTreeGroup group)
        {
            group.SetNode(Groups.AddLast(group), this);

            if (CurrentGroup == null)
                CurrentGroup = group;
        }

        public override void SetMenu(Vector2 Position)
        {
            foreach (TechTreeGroup g in Groups)
                g.Position = Position - NegativeOffset - new Vector2(0, g.Size.Y);
            base.SetMenu(Position);
        }

        public void SetGroup(TechTreeGroup CurrentGroup)
        {
            this.CurrentGroup = CurrentGroup;
        }

        public override void Update(GameTime gameTime, BasicController MyController)
        {
            LinkedListNode<TechTreeGroup> NextNode = Groups.First;
            do
            {
                if (NextNode != null)
                {
                    NextNode.Value.Update(gameTime, MyController, NextNode.Value == CurrentGroup);
                    NextNode = NextNode.Next;
                }
                else
                    break;
            }
            while (NextNode != null);


            base.Update(gameTime, MyController);
        }

        public override void Draw3D(Camera3D DrawCamera)
        {
            CurrentGroup.Draw3D(DrawCamera);
            base.Draw3D(DrawCamera);
        }

        public override void Draw()
        {
            CurrentGroup.DrawAll();
            if (CardPickingMode)
                Render.DrawShadowedText("Position: " + FactionManager.GetFaction(ParentShip.FactionNumber).CardPickPosition.ToString(), new Vector2(100));
        }
    }
}
