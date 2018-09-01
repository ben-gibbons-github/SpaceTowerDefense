using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CobraCard : UnitCard
    {
        static Texture2D StaticCastTexture;

        public override Texture2D CastTexture()
        {
            if (StaticCastTexture == null)
                StaticCastTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/OffenseAbilities/HudOffenseCobra");
            return StaticCastTexture;
        }

        public CobraCard()
        {
            Name = "Cobra";
            GhostCount = 30;
            EnergyCost = 150;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Cobra(FactionNumber);
        }
    }
}
