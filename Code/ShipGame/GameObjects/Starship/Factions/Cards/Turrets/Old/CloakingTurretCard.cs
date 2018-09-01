using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CloakingTurretCard : TurretCard
    {
        public static float STurretSize = 50;
        public static float EngagementDistance = 150;

        public CloakingTurretCard()
        {
            cardType = CardType.Rebels;

            Name = "Cloaking Turret";
            CardCellsCost = 150;
            CardCellsCostIncrease = 150;
            TurretSize = STurretSize;
            Radius = EngagementDistance;
        }

        public override UnitBasic GetUnit(int FactionNumber)
        {
            return new CloakingTurret(FactionNumber);
        }
    }
}
