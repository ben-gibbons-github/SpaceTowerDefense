using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SpikeTurretCard : TurretCard
    {
        public static float STurretSize = 80;
        public static float EngagementDistance = 80;

        public SpikeTurretCard()
        {
            cardType = CardType.Empire;

            Name = "Spike Turret";
            Caption = "Destroys enemy units on impact.\nSTRONG VS. MELEE";
            StrongVs = "Light";

            CardCellsCost = 300;
            CardCellsCostIncrease = 100;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override float GetTurretFragility()
        {
            return 0.5f;
        }

        public override float GetBuildingAvoidence()
        {
            return 1f;
        }

        public override float GetTurretAgression()
        {
            return 1.25f;
        }

        protected override float GetTurretWeight()
        {
            return 0.75f;
        }

        public override float GetBaseAvoidence()
        {
            return -0.25f;
        }

        public override Microsoft.Xna.Framework.Color GetColor()
        {
            return new Color(1, 0.25f, 0.25f);
        }
        
        public override string GetImagePath()
        {
            return "Empire/Turret5";
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new SpikeTurret(FactionNumber);
        }
    }
}
