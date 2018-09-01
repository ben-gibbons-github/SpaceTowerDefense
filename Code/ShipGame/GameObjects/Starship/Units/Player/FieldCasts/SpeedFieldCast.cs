using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SpeedFieldCast : OffenseAbility
    {
        public static SpeedFieldCast self;

        public static Texture2D StaticCastTexture;

        public override Texture2D CastTexture()
        {
            if (StaticCastTexture == null)
                StaticCastTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/OffenseAbilities/HudOffenseSpeedCast");
            return StaticCastTexture;
        }

        static SpeedFieldCast()
        {
            self = new SpeedFieldCast();
        }

        public override bool Trigger(PlayerShip p)
        {
            if (BasicField.TestFieldClear(p.Position.get()))
            {
                BasicField f = new SpeedBoostField();
                p.ParentLevel.AddObject(f);
                f.Position.set(p.Position.get());
                f.TargetSize *= 1.5f;
                return true;
            }

            return false;
        }
    }
}
