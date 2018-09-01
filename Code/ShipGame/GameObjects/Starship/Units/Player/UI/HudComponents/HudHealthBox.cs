using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudHealthBox : HudBox
    {
        static Texture2D MyTexture;
        static Texture2D SliceTexture;

        Vector2 TargetSliceSize;
        int CurrentMaxCounter = 0;

        public override void Create(PlayerShip ParentShip)
        {
            if (MyTexture == null)
            {
                MyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudHealthBox");
                SliceTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudHealthSlice");
            }

            SetDimensions(new Vector2(650, 150), new Vector2(600, 110));
            TargetSliceSize = new Vector2(350 / 600f, 30 / 110f);

            base.Create(ParentShip);
        }

        public override void Update(GameTime gameTime)
        {
            int c = FactionManager.GetFaction(ParentShip.FactionNumber).MaxMiningPlatformCounter / 2;
            if (c != CurrentMaxCounter)
            {
                CurrentMaxCounter = c;

                AddItem(new HudMineralCounter());

                int Counter = 0;
                foreach (HudMineralCounter m in Children)
                {
                    Counter++;
                    m.SetCounter(Counter, (-Children.Count / 2f + Counter - 0.5f) * 80);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            Render.DrawSprite(MyTexture, Position, Size, 0, TeamInfo.HudColors[ParentShip.GetTeam()]);
            Vector2 SliceSize = TargetSliceSize * Size;
            Game1.spriteBatch.Draw(SliceTexture, Position - SliceSize / 2, new Rectangle(0, 0, (int)(
                SliceTexture.Width * Math.Floor(ParentShip.HealthMult() * 50) / 50f), SliceTexture.Height), TeamInfo.HudColors[ParentShip.GetTeam()],
                0, Vector2.Zero, SliceSize / new Vector2(SliceTexture.Width,SliceTexture.Height), SpriteEffects.None,0);
            base.Draw(Position, Size);
        }
    }
}
