using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class VampireCard : UnitCard
    {
        static Texture2D StaticCastTexture;

        public override Texture2D CastTexture()
        {
            if (StaticCastTexture == null)
                StaticCastTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/OffenseAbilities/HudOffenseVampire");
            return StaticCastTexture;
        }

        public VampireCard()
        {
            Name = "Vampire";
            GhostCount = 18;
            EnergyCost = 150;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Vampire(FactionNumber);
        }
    }
}
