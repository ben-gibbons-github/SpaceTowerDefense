using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ImmortalityCast : OffenseAbility
    {
        static Color ParticleColor = new Color(0.2f, 0.2f, 0.2f);
        static Color ParticleColor2 = new Color(0.5f, 0.5f, 0.5f);

        public static Texture2D StaticCastTexture;

        public override Texture2D CastTexture()
        {
            if (StaticCastTexture == null)
                StaticCastTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/OffenseAbilities/HudOffenseInvShipCast");
            return StaticCastTexture;
        }

        public override bool Trigger(PlayerShip p)
        {
            Vector3 Position3 = new Vector3(p.Position.X(), 0, p.Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 2000, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 4000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 7);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 300, 7);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 500, 7);

            for (int i = 0; i < 2; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 1.5f, new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 150, 30, new Vector3(0.2f, 0.5f, 1), new Vector3(0.2f, 0.5f, 1), 0, 3);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(0.5f, 0.75f, 1), p.Size.X() * 50, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(0.5f, 0.75f, 1), 200, 5);

            p.InvTime = 10000;

            return true;
        }
    }
}
