using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudFactionFeed : HudBox
    {
        static string RespawnString = "200 Energy : Hold LT to Respawn";
        static float StringWidth;
        static float AlphaChange = 0.1f;

        float PayToRespawnAlpha = 0;

        public override void Create(PlayerShip ParentShip)
        {
            StringWidth = FactionEvent.FeedFont.MeasureString(RespawnString).X;
            SetDimensions(new Vector2(1280 / 2, 600), Vector2.One);
            base.Create(ParentShip);
        }

        public override void Update(GameTime gameTime)
        {
            if (ParentShip.Dead && ParentShip.Attacking && FactionManager.CanAfford(ParentShip.FactionNumber,0 , PlayerShip.GetRespawnCost()))
            {
                PayToRespawnAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (PayToRespawnAlpha > 1)
                    PayToRespawnAlpha = 1;
            }
            else
            {
                PayToRespawnAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (PayToRespawnAlpha < 0)
                    PayToRespawnAlpha = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            if (PayToRespawnAlpha > 0)
            {
                Render.DrawShadowedText(FactionEvent.FeedFont, RespawnString, Position - new Vector2(StringWidth / 2, 100),
                    Vector2.Zero, Color.White * PayToRespawnAlpha, Color.Black * PayToRespawnAlpha);
            }
            FactionManager.GetFaction(ParentShip.FactionNumber).Draw(Position);
        }
    }
}
