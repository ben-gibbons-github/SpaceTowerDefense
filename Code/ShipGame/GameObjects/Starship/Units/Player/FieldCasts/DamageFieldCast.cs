using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class DamageFieldCast : OffenseAbility
    {
        public static DamageFieldCast self;

        static DamageFieldCast()
        {
            self = new DamageFieldCast();
        }

        public static Texture2D StaticCastTexture;

        public override Texture2D CastTexture()
        {
            if (StaticCastTexture == null)
                StaticCastTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/OffenseAbilities/HudOffenseDamageCast");
            return StaticCastTexture;
        }

        public override bool Trigger(PlayerShip p)
        {
            if (BasicField.TestFieldClear(p.Position.get()))
            {
                BasicField f = new DamageBoostField();
                p.ParentLevel.AddObject(f);
                f.Position.set(p.Position.get());
                f.TargetSize *= 1.5f;
                return true;
            }

            return false;
        }
    }
}
