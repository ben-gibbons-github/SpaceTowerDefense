using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class UnitCard : FactionCard
    {
        public int GhostCount = 15;
        public int EnergyCost = 150;

        public virtual Texture2D CastTexture()
        {
            return null;   
        }

        public UnitCard()
        {

        }
    }
}
