using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class CrystalShip : UnitShip
    {
        /*
        protected static Color CrystalColor = new Color(0.75f, 0.75f, 1);
        protected static float AlphaChange = 0.05f;

        protected float StunDistance = 400;
        protected int MaxStunSearchTime = 1000;
        protected UnitBasic StunnedUnit;
        protected int StunSearchTime = 0;
        protected float Alpha = 0;
        */

        public CrystalShip(int FactionNumber)
            : base(FactionNumber)
        {
            ScoreToGive = 50;
        }
    }
}
