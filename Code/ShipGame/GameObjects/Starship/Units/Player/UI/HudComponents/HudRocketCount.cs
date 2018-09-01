using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class HudRocketCount : HudItem
    {
        static float AlphaChange = 0.05f;
        float Alpha;

        public override void Create(HudBox ParentBox)
        {
            SetDimensions(new Vector2(-20, 0), new Vector2(50, 70));
            base.Create(ParentBox);
        }

        public override void Update(GameTime gameTime)
        {
            float TargetAlpha = 
                ParentBox.ParentShip.Attacking ? (ParentBox.ParentShip.getUnitCasts() > 0 ? 1 : 0.2f)
                : (ParentBox.ParentShip.getSmallBombs() > 0 ? 1 : 0.2f);

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
            DigitRenderer.DrawDigits(
                ParentBox.ParentShip.Attacking ? ParentBox.ParentShip.getUnitCasts() : ParentBox.ParentShip.getSmallBombs(),
                1, Position, Size, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * Alpha);
            base.Draw(Position, Size);
        }
    }
}
