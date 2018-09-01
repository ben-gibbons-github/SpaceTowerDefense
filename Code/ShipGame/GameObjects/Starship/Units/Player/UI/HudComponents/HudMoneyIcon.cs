using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudMoneyIcon : HudItem
    {
        static Texture2D MyTexture;
        static float AlphaChange = 0.01f;

        float IconAlpha = 0;
        float GlowAlpha = 0;

        int OldCells;

        public override void Create(HudBox ParentBox)
        {
            if (MyTexture == null)
                MyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudMoney");
            SetDimensions(new Vector2(-15, 25), new Vector2(30, 30));

            base.Create(ParentBox);
        }

        public override void Update(GameTime gameTime)
        {
            GlowAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
            if (GlowAlpha < 0)
                GlowAlpha = 0;

            float TargetAlpha = FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Cells > 0 ? 1 : 0.2f;
            if (IconAlpha < TargetAlpha)
            {
                IconAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (IconAlpha > TargetAlpha)
                    IconAlpha = TargetAlpha;
            }
            else
            {
                IconAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (IconAlpha < TargetAlpha)
                    IconAlpha = TargetAlpha;
            }

            if (FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Cells != OldCells)
            {
                OldCells = FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Cells;
                GlowAlpha = 1;
            }
            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            Render.DrawSprite(MyTexture, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * IconAlpha);
            Render.DrawSprite(GlowTexture, Position, Size * 8, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * GlowAlpha);

            base.Draw(Position, Size);
        }
    }
}
