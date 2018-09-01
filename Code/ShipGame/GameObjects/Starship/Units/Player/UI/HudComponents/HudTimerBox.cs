using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudTimerBox : HudBox
    {
        SpriteFont TimerFont;

        public override void Create(PlayerShip ParentShip)
        {
            if (TimerFont == null)
                TimerFont = AssetManager.Load<SpriteFont>("Fonts/ShipGame/TimerFont");

            SetDimensions(new Vector2(1000, 150), Vector2.Zero);
            base.Create(ParentShip);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            string Text = FactionManager.Factions[ParentShip.FactionNumber].BestSurvivedWave.ToString() + "<>" + 
                FactionManager.Factions[ParentShip.FactionNumber].SurvivedMinutes.ToString() + ":" +
                FactionManager.Factions[ParentShip.FactionNumber].SurvivedSeconds.ToString() + ":" +
                FactionManager.Factions[ParentShip.FactionNumber].SurvivedMilliSeconds.ToString() + "\nMining Rings:" +
                FactionManager.Factions[ParentShip.FactionNumber].MiningPlatformCount.ToString();
            Vector2 TextSize = TimerFont.MeasureString(Text);
            Render.DrawShadowedText(TimerFont, Text, Position - TextSize / 2, Vector2.One, TeamInfo.HudColors[ParentShip.GetTeam()], Color.Black);
        }
    }
}
