using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class ShieldBreakerTurretCard : TurretCard
    {
        public static float STurretSize = 150;
        public static float EngagementDistance = 600;

        public ShieldBreakerTurretCard()
        {
            Name = "Shield Breaker Turret";
            CardCellsCost = 800;
            CardCellsCostIncrease = 800;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new ShieldBreakerTurret(FactionNumber);
        }
    }
}
