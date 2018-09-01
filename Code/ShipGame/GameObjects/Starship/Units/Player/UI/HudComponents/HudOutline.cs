using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudOutline : HudBox
    {
        static Texture2D OutlineTexture;
        static Texture2D DamageTexture;

        float OutlineAlpha = 0;
        static float AlphaChange = 0.02f;

        public override void Create(PlayerShip ParentShip)
        {
            if (OutlineTexture == null)
            {
                OutlineTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudOutline");
                DamageTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudDamage");
            }

            SetDimensions(Vector2.Zero, new Vector2(1280, 720));

            base.Create(ParentShip);
        }

        public override void Update(GameTime gameTime)
        {
            if (ParentShip.viewMode == ViewMode.Ship)
            {
                OutlineAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (OutlineAlpha > 1)
                    OutlineAlpha = 1;
            }
            else
            {
                OutlineAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (OutlineAlpha < 0)
                    OutlineAlpha = 0;
            }

            base.Update(gameTime);
        }

        public override void PreDraw()
        {
            if (OutlineAlpha > 0 || ParentShip.DamageAlpha > 0)
            {
                Rectangle SceneViewRectangle = new Rectangle(0, 0, (int)ParentShip.sceneView.Size.X, (int)ParentShip.sceneView.Size.Y);
                if (OutlineAlpha > 0)
                    Game1.spriteBatch.Draw(OutlineTexture, SceneViewRectangle, TeamInfo.HudColors[ParentShip.GetTeam()] * OutlineAlpha);
                if (ParentShip.DamageAlpha > 0)
                    Game1.spriteBatch.Draw(DamageTexture, SceneViewRectangle, Color.Red * ParentShip.DamageAlpha);
            }
        }
    }
}
