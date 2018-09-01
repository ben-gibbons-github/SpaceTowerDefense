using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudScoreBox : HudOutlineBox
    {
        public override void Create(PlayerShip ParentShip)
        {
            SetPosition(new Vector2(250, 190));
            AddItem(new HudScoreCount());
            AddItem(new HudMoneyCount());
            AddItem(new HudEnergyCount());
            AddItem(new HudEnergyIcon());
            AddItem(new HudMoneyIcon());
            base.Create(ParentShip);
        }

        public override void Update(GameTime gameTime)
        {
            float TargetAlpha = 0.2f;
            Faction f = FactionManager.GetFaction(ParentShip.FactionNumber);

            if (f.Cells > 0 || f.Energy > 0 || f.Score > 0)
                TargetAlpha = 1;
            if (Alpha > TargetAlpha)
            {
                Alpha -= gameTime.ElapsedGameTime.Milliseconds * AlphaChange * 60 / 1000f;
                if (Alpha < TargetAlpha)
                    Alpha = TargetAlpha;
            }
            else
            {
                Alpha += gameTime.ElapsedGameTime.Milliseconds * AlphaChange * 60 / 1000f;
                if (Alpha > TargetAlpha)
                    Alpha = TargetAlpha;
            }

            base.Update(gameTime);
        }
    }
}
