using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class EmpWeaponCast : OffensiveWeaponCast
    {
        public static Texture2D StaticCastTexture;

        public override Texture2D CastTexture()
        {
            if (StaticCastTexture == null)
                StaticCastTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/OffenseAbilities/HudOffenseEmpCast");
            return StaticCastTexture;
        }

        public EmpWeaponCast()
        {
            fireMode = new StingRayEmpFireMode();
            fireMode.SetLevel(3.5f);
        }
    }
}
