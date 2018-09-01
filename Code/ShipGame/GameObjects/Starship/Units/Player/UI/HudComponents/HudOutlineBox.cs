using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HudOutlineBox : HudBox
    {
        protected static float AlphaChange = 0.1f;
        static Texture2D MyTexture;

        protected float Alpha = 0;

        public override void Create(PlayerShip ParentShip)
        {
            if (MyTexture == null)
                MyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudBox");

            base.Create(ParentShip);
        }

        public void SetPosition(Vector2 Position)
        {
            SetDimensions(Position, new Vector2(280, 140));
        }

        public override void Draw(Vector2 Position, Vector2 Size)
        {
            Render.DrawSprite(MyTexture, Position, Size, 0, TeamInfo.HudColors[ParentShip.GetTeam()] * Alpha);
            base.Draw(Position, Size);
        }
    }
}
