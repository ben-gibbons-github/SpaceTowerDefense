using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HornetCard : UnitCard
    {
        static Texture2D StaticCastTexture;

        public override Texture2D CastTexture()
        {
            if (StaticCastTexture == null)
                StaticCastTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/OffenseAbilities/HudOffenseHornet");
            return StaticCastTexture;
        }

        public HornetCard()
        {
            Name = "Hornet";
            GhostCount = 22;
            EnergyCost = 150;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new Hornet(FactionNumber);
        }
    }
}
