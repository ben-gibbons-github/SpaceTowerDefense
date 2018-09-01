using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudEnergyIcon : HudItem
    {
        static Texture2D MyTexture;
        static float AlphaChange = 0.1f;

        float IconAlpha = 0;
        float GlowAlpha = 0;

        int OldEnergy;

        public override void Create(HudBox ParentBox)
        {
            if (MyTexture == null)
                MyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudEnergy");
            SetDimensions(new Vector2(110, 25), new Vector2(30, 30));
            base.Create(ParentBox);
        }

        public override void Update(GameTime gameTime)
        {
            GlowAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
            if (GlowAlpha < 0)
                GlowAlpha = 0;

            if (FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Energy > 0)
            {
                IconAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * IconAlpha;
                if (IconAlpha > 1)
                    IconAlpha = 1;
                if (FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Energy != OldEnergy)
                {
                    OldEnergy = FactionManager.GetFaction(ParentBox.ParentShip.FactionNumber).Energy;
                    GlowAlpha = 1;
                }
            }
            else
            {
                IconAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * IconAlpha;
                if (IconAlpha < 0.2f)
                    IconAlpha = 0.2f;
            }
            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            Render.DrawSprite(MyTexture, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * IconAlpha);
            Render.DrawSprite(GlowTexture, Position, Size * 3, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * GlowAlpha);

            base.Draw(Position, Size);
        }
    }
}
