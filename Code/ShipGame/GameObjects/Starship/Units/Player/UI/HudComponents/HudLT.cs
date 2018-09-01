using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudLT : HudItem
    {
        static Texture2D MyTexture;

        static bool Loaded = false;

        static float AlphaChange = 0.1f;

        float LTAlpha = 0;

        public override void Create(HudBox ParentBox)
        {
            if (!Loaded)
            {
                Loaded = true;
                MyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/Keys/LT");
            }

            SetDimensions(new Vector2(-60, -120), new Vector2(120, 120));
            base.Create(ParentBox);
        }

        public override void Update(GameTime gameTime)
        {
            if (ParentBox.ParentShip.WeaponBoxCenterTime > 0)// && Vector2.Distance(ParentBox.RealPosition, ParentBox.TargetPosition) < 10)
            {
                LTAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (LTAlpha > 1)
                    LTAlpha = 1;
            }
            else
            {
                LTAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (LTAlpha < 0)
                    LTAlpha = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            if (LTAlpha > 0)
                Render.DrawSprite(MyTexture, Position, Size, 0, TeamInfo.HudColors[ParentBox.ParentShip.GetTeam()] * LTAlpha);

            base.Draw(Position, Size);
        }
    }
}
