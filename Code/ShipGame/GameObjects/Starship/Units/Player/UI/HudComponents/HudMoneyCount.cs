using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class HudMoneyCount : HudItem
    {
        static float AlphaChange = 0.05f;
        float Alpha;

        public override void Create(HudBox ParentBox)
        {
            SetDimensions(new Vector2(-90, 25), new Vector2(90, 32));
            base.Create(ParentBox);
        }

        public override void Update(GameTime gameTime)
        {
            float TargetAlpha = FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Cells > 0? 1 : 0.2f;
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
            DigitRenderer.DrawDigits(FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Cells, 4, Position, Size, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * Alpha);
            base.Draw(Position, Size);
        }
    }
}
