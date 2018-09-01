using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class HudScoreCount : HudItem
    {
        static float AlphaChange = 0.1f;
        float Alpha;

        public override void Create(HudBox ParentBox)
        {
            SetDimensions(new Vector2(-40, -20), new Vector2(220, 40));
            base.Create(ParentBox);
        }

        public override void Update(GameTime gameTime)
        {
            float TargetAlpha = FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Score > 0 ? 1 : 0.2f;
            if (Alpha < TargetAlpha)
            {
                Alpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (Alpha > TargetAlpha)
                    Alpha = TargetAlpha;
            }
            else
            {
                Alpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (Alpha < TargetAlpha)
                    Alpha = TargetAlpha;
            }
            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            DigitRenderer.DrawDigits(FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Score, 8, Position, Size, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * Alpha);
            base.Draw(Position, Size);
        }
    }
}
