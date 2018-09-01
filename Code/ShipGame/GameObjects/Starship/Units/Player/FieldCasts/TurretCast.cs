using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class TurretCast : OffenseAbility
    {
        public static Texture2D StaticCastTexture;

        public override Texture2D CastTexture()
        {
            if (StaticCastTexture == null)
                StaticCastTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/OffenseAbilities/HudOffenseTurretCast");
            return StaticCastTexture;
        }

        public override bool Trigger(PlayerShip p)
        {
            if (p.CanPlaceTurret(80))
            {
                p.PlaceTurret(new EngineerTurret(p.FactionNumber));
                return true;
            }
            else 
                return false;
        }
    }
}
