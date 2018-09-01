using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class TextParticleSystemIcons
    {
        public static Texture2D EnergyTexture;
        public static Texture2D RingTexture;
        public static Texture2D CellsTexture;

        static TextParticleSystemIcons()
        {
            EnergyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/TextParticleIcons/EnergyIcon");
            RingTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/TextParticleIcons/RingIcon");
            CellsTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/TextParticleIcons/CellsIcon");
        }
    }
}
