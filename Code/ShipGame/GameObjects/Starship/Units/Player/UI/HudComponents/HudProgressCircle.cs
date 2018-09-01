using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudProgressCircle : HudBox
    {
        static Texture2D OuterTexture;
        static Texture2D InnerTexture;
        static Texture2D EliminatedTexture;

        static float AlphaChange = 0.05f;
        float ProgressAlpha;
        float EliminatedAlpha;

        public override void Create(PlayerShip ParentShip)
        {
            if (OuterTexture == null)
            {
                OuterTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudCircleOuter");
                InnerTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudCircleInner");
                EliminatedTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/Eliminated");
            }

            SetDimensions(new Vector2(1280 / 2, 720 / 2), new Vector2(300, 300));

            base.Create(ParentShip);
        }

        public override void Update(GameTime gameTime)
        {
            if (ParentShip.GetOffenseProgress() > 0)
            {
                ProgressAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (ProgressAlpha > 1)
                    ProgressAlpha = 1;
            }
            else
            {
                ProgressAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (ProgressAlpha < 0)
                    ProgressAlpha = 0;
            }


            if (WaveFSM.PlayerEliminatedState.LastEliminatedTeam != -1)
            {
                EliminatedAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (EliminatedAlpha > 1)
                    EliminatedAlpha = 1;
            }
            else
            {
                EliminatedAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (EliminatedAlpha < 0)
                    EliminatedAlpha = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            if (ProgressAlpha > 0)
            {
                Render.DrawSprite(OuterTexture, Position, Size * ProgressAlpha, 0, TeamInfo.HudColors[ParentShip.GetTeam()] * ProgressAlpha);
                Render.DrawSprite(InnerTexture, Position, Size * ProgressAlpha, ParentShip.GetOffenseProgress() * (float)Math.PI, TeamInfo.HudColors[ParentShip.GetTeam()] * ProgressAlpha);
            }
            if (EliminatedAlpha > 0)
            {
                Render.DrawSprite(EliminatedTexture, Position, Size * EliminatedAlpha, 0, TeamInfo.HudColors[WaveFSM.PlayerEliminatedState.LastEliminatedTeam] * EliminatedAlpha);
                
            }
            base.Draw(Position, Size);
        }
    }
}
