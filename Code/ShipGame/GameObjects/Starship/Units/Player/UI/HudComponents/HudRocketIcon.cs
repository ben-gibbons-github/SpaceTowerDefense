using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudRocketIcon : HudItem
    {
        static Texture2D MyTexture;
        static Texture2D OffenseTexture;

        static float AlphaChange = 0.1f;
        static bool Loaded = false;

        float IconAlpha = 0;
        float GlowAlpha = 0;

        int OldBombs;
        int OldCasts;

        public override void Create(HudBox ParentBox)
        {
            if (!Loaded)
            {
                Loaded = true;
                MyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudRocket");
            }

            OffenseTexture = MyTexture;
            SetDimensions(new Vector2(80, 0), new Vector2(60, 60));
            base.Create(ParentBox);
        }

        public override void Update(GameTime gameTime)
        {
            GlowAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
            if (GlowAlpha < 0)
                GlowAlpha = 0;

            if (!ParentBox.ParentShip.Attacking)
            {
                if (ParentBox.ParentShip.getSmallBombs() > 0)
                {
                    IconAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                    if (IconAlpha > 1)
                        IconAlpha = 1;
                    if (ParentBox.ParentShip.getSmallBombs() != OldBombs)
                    {
                        OldBombs = ParentBox.ParentShip.getSmallBombs();
                        GlowAlpha = 1;
                    }
                }
                else
                {
                    IconAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * IconAlpha;
                    if (IconAlpha < 0.2f)
                        IconAlpha = 0.2f;
                }
            }
            else
            {
                if (ParentBox.ParentShip.getUnitCasts() > 0)
                {
                    GhostCast g = (GhostCast)ParentBox.ParentShip.CurrentSpecialWeapon;
                    OffenseTexture = g.MyCard.CastTexture();

                    IconAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                    if (IconAlpha > 1)
                        IconAlpha = 1;
                    if (ParentBox.ParentShip.getUnitCasts() != OldCasts)
                    {
                        OldCasts = ParentBox.ParentShip.getUnitCasts();
                        GlowAlpha = 1;
                    }
                }
                else
                {
                    IconAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * IconAlpha;
                    if (IconAlpha < 0.2f)
                        IconAlpha = 0.2f;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            if (ParentBox.ParentShip.Attacking)
                Render.DrawSprite(OffenseTexture, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * IconAlpha);
            else
                Render.DrawSprite(MyTexture, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * IconAlpha);

            Render.DrawSprite(GlowTexture, Position, Size * 3, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * GlowAlpha);

            base.Draw(Position, Size);
        }
    }
}
