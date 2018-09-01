using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudBombIcon : HudItem
    {
        static Texture2D MyTexture;

        static bool Loaded = false;

        static float AlphaChange = 0.1f;
        static int MaxIconResizeTime = 2000;

        float IconAlpha = 0;
        float GlowAlpha = 0;
        float OffenseIconAlpha = 0;
        Texture2D OffenseTexture;

        bool IconResized = false;
        int IconResizeTime = 0;

        float SizeMult = 1;
        float SizeMultAcceleration = 0;
        float SizeMultGravity = 0.01f;
        float MinSizeMult = 1;
        float SizeMultBounce = 1;

        int OldBombs;

        public override void Create(HudBox ParentBox)
        {
            if (!Loaded)
            {
                Loaded = true;
                MyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudBomb");
            } 

            SetDimensions(new Vector2(-60, 0), new Vector2(100, 100));
            base.Create(ParentBox);
        }

        public override void Update(GameTime gameTime)
        {
            OffenseIconAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
            if (OffenseIconAlpha < 0)
                OffenseIconAlpha = 0;

            GlowAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
            if (GlowAlpha < 0)
                GlowAlpha = 0;

            if (!ParentBox.ParentShip.Attacking)
            {
                IconResized = false;

                if (ParentBox.ParentShip.getBigBombs() > 0)
                {
                    IconAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                    if (IconAlpha > 1)
                        IconAlpha = 1;
                    if (ParentBox.ParentShip.getBigBombs() != OldBombs)
                    {
                        OldBombs = ParentBox.ParentShip.getBigBombs();
                        GlowAlpha = 1;
                    }
                }
                else
                {
                    IconAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                    if (IconAlpha < 0.2f)
                        IconAlpha = 0.2f;
                }
            }
            else
            {
                if (IconResizeTime > 0)
                    IconResizeTime -= gameTime.ElapsedGameTime.Milliseconds;
                else if (!IconResized)
                {
                    IconResized = true;
                    IconResizeTime = MaxIconResizeTime;
                }

                SizeMult += SizeMultAcceleration * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f / 6;
                SizeMultAcceleration -= SizeMultGravity * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 3;

                if (SizeMult < MinSizeMult)
                {
                    SizeMult = MinSizeMult;
                    if (IconResizeTime > 0)
                        SizeMultAcceleration = SizeMultBounce;
                }

                IconAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * IconAlpha;
                if (IconAlpha < 0)
                    IconAlpha = 0;


                if (ParentBox.ParentShip.offenseAbility != null)
                {
                    OffenseTexture = ParentBox.ParentShip.offenseAbility.CastTexture();

                    OffenseIconAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange * 2;
                    if (OffenseIconAlpha > 1)
                        OffenseIconAlpha = 1;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            if (!FactionManager.Factions[ParentBox.ParentShip.FactionNumber].PickingCards)
            {
                if (IconAlpha > 0)
                    Render.DrawSprite(MyTexture, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * IconAlpha);
                if (GlowAlpha > 0)
                    Render.DrawSprite(GlowTexture, Position, Size * 3, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * GlowAlpha);
                if (OffenseIconAlpha > 0)
                    Render.DrawSprite(OffenseTexture, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * OffenseIconAlpha);
            }

            base.Draw(Position, Size);
        }
    }
}
