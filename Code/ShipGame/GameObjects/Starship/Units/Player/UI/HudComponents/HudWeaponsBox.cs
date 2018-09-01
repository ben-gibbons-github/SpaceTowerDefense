using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudWeaponsBox : HudOutlineBox
    {
        Vector2 CornerPosition = new Vector2(250, 530);
        Vector2 CenterPosition = new Vector2(640, 530);

        public override void Create(PlayerShip ParentShip)
        {
            MoveSpeed = 10;
            RealPosition = CornerPosition;
            AddItem(new HudRocketCount());
            AddItem(new HudRocketIcon());
            AddItem(new HudBombIcon());
            AddItem(new HudLT());
            AddItem(new HudRT());
            base.Create(ParentShip);
        }

        public override bool Minimize()
        {
            return base.Minimize() || (ParentShip.Dead && ParentShip.GetTeam() != WaveManager.ActiveTeam);
        }

        public override void Update(GameTime gameTime)
        {
            SetPosition(CornerPosition);

            float TargetAlpha = 0.2f;
            Faction f = FactionManager.GetFaction(ParentShip.FactionNumber);

            if (!FactionManager.Factions[ParentShip.FactionNumber].PickingCards && (f.Cells > 0 || f.Energy > 0 || f.Score > 0))
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
