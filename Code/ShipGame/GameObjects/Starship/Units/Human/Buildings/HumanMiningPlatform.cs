using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class HumanMiningPlatform : MiningPlatform
    {
        public static Texture2D PlaceholderHumanMiningPlatformTexture;

        public HumanMiningPlatform(int FactionNumber) :
            base(FactionNumber)
        {

        }

        public override int GetIntType()
        {
            return 1 + InstanceManager.WorldIndex;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/PlanetRing");
        }
    }
}
