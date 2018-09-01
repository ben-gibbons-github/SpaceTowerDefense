using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudItem
    {
        public static Texture2D GlowTexture;
        public static Texture2D DigitTexture;

        protected Vector2 TargetPosition;
        Vector2 TargetSize;

        Vector2 RealPosition;
        Vector2 RealSize;

        protected float MoveSpeed = 0;

        public HudBox ParentBox;

        static HudItem()
        {
            GlowTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudGlow");
            DigitTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/Digits");
        }

        public virtual void Create(HudBox ParentBox)
        {
            this.ParentBox = ParentBox;
        }

        protected void SetDimensions(Vector2 TargetPosition, Vector2 TargetSize)
        {
            this.TargetPosition = TargetPosition;
            this.TargetSize = TargetSize;
            RealPosition = TargetPosition;
            if (MoveSpeed == 0)
                RealSize = TargetSize;
        }

        public virtual void PreDraw(Vector2 ParentPosition, Vector2 ParentSize)
        {
            Vector2 ProjectedSize = RealSize / HudBox.ScreenSize * ParentBox.ParentShip.sceneView.Size;
            Vector2 ProjectedPosition = RealPosition / HudBox.ScreenSize * ParentBox.ParentShip.sceneView.Size;
            ProjectedSize *= ParentBox.SizeMult;
            Draw(ParentPosition + Vector2.Normalize(RealPosition) * ProjectedPosition.Length(),
                Vector2.Normalize(RealSize) * ProjectedSize.Length());
        }

        public virtual void Update(GameTime gameTime)
        {
            if (MoveSpeed > 0)
            {
                float MoveAmount = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * MoveSpeed;

                if (Vector2.Distance(RealPosition, TargetPosition) < MoveAmount * 1.5f)
                    RealPosition = TargetPosition;
                else
                    RealPosition += Vector2.Normalize(TargetPosition - RealPosition) * MoveAmount;

                if (Vector2.Distance(RealSize, TargetSize) < MoveAmount * 1.5f)
                    RealSize = TargetSize;
                else
                    RealSize += Vector2.Normalize(TargetSize - RealSize) * MoveAmount;
            }
        }

        public virtual void Draw(Vector2 Position, Vector2 Size)
        {

        }
    }
}
