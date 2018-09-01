using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudBox
    {
        public static Vector2 ScreenSize = new Vector2(1280, 720);

        public PlayerShip ParentShip;

        protected LinkedList<HudItem> Children = new LinkedList<HudItem>();

        public Vector2 TargetPosition; // in absolute screen space
        public Vector2 RealPosition;
        Vector2 TargetSize;
        public float SizeMult = 0;

        protected float MoveSpeed = 0;
            
        public virtual void Create(PlayerShip ParentShip)
        {
            this.ParentShip = ParentShip;
        }

        public void AddItem(HudItem c)
        {
            Children.AddLast(c);
            c.Create(this);
        }

        protected void SetDimensions(Vector2 TargetPosition, Vector2 TargetSize)
        {
            this.TargetPosition = TargetPosition;
            this.TargetSize = TargetSize;
            if (MoveSpeed == 0)
                RealPosition = TargetPosition;
        }

        public virtual bool Minimize()
        {
            return FactionManager.Factions[ParentShip.FactionNumber].PickingCards;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Minimize())
            {
                SizeMult -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.01f;
                if (SizeMult < 0)
                    SizeMult = 0;
            }
            else
            {
                SizeMult += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.01f;
                if (SizeMult > 1)
                    SizeMult = 1;
            }

            if (MoveSpeed > 0)
            {
                float MoveAmount = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * MoveSpeed;

                if (Vector2.Distance(RealPosition, TargetPosition) < MoveAmount * 1.5f)
                    RealPosition = TargetPosition;
                else
                    RealPosition += Vector2.Normalize(TargetPosition - RealPosition) * MoveAmount;
            }

            foreach (HudItem i in Children)
                i.Update(gameTime);
        }

        public virtual void PreDraw()
        {
            Vector2 ProjectedSize = TargetSize / ScreenSize * ParentShip.sceneView.Size * SizeMult;
            Draw((RealPosition) / ScreenSize * ParentShip.sceneView.Size, 
                Vector2.Normalize(TargetSize) * ProjectedSize.Length());
        }

        public virtual void Draw(Vector2 Position, Vector2 Size)
        {
            foreach (HudItem c in Children)
            {
                c.PreDraw(Position, Size);
            }
        }
    }
}
