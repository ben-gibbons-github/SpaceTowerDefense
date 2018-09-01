using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudMineralCounter : HudItem
    {
        static Texture2D SolidSphere;
        static Texture2D OutlineSphere;
        static float AlphaChange = 0.1f;
        static float GlowAlphaChange = 0.1f;

        public int Counter = 1;
        float FullAlpha = 0;
        float ExtraGlowAlpha = 0;
        float GlowAlpha = 0;

        public override void Create(HudBox ParentBox)
        {
            if (SolidSphere == null)
            {
                SolidSphere = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudFullSphere");
                OutlineSphere = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudEmptySphere");
            }

            MoveSpeed = 0.1f;
            SetDimensions(new Vector2(0, 80), new Vector2(40, 40));

            base.Create(ParentBox);
        }

        public void SetCounter(int Counter, float X)
        {
            this.Counter = Counter;
            TargetPosition.X = X;
        }

        public override void Update(GameTime gameTime)
        {
            MoveSpeed = 1f;

            if (Counter > FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).MiningPlatformCounter / 2)
            {
                GlowAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * GlowAlphaChange;
                FullAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (FullAlpha < 0)
                    FullAlpha = 0;
                if (GlowAlpha < 0)
                    GlowAlpha = 0;
            }
            else
            {
                if (GlowAlpha == 0)
                    ExtraGlowAlpha = 2;

                GlowAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 10000f * GlowAlphaChange;
                FullAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (FullAlpha > 1)
                    FullAlpha = 1;

                float MaxGlowAlpha = FactionManager.CanBuildMiningPlatform(ParentBox.ParentShip.FactionNumber) ? 2 : 1;
                if (GlowAlpha > MaxGlowAlpha)
                    GlowAlpha = MaxGlowAlpha;
            }

            ExtraGlowAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
            if (ExtraGlowAlpha < 0)
                ExtraGlowAlpha = 0;

            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            if (FullAlpha == 1)
                Render.DrawSprite(SolidSphere, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()]);
            else
            {
                Render.DrawSprite(OutlineSphere, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()]);
                if (FullAlpha != 0)
                    Render.DrawSprite(SolidSphere, Position, Size * FullAlpha, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()]);
            }
            if (GlowAlpha > 0)
                Render.DrawSprite(GlowTexture, Position, Size * 6, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * (GlowAlpha + ExtraGlowAlpha) * (0.45f + Rand.F() * 0.1f));

            base.Draw(Position, Size);
        }
    }
}
