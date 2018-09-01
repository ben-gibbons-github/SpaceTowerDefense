using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SpearTurret : UnitTurret
    {
        public SpearTurret(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "CrystalTurretExplode";
            ShieldToughness = 20;
            HullToughness = 40;
            MaxEngagementDistance = SpearTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Red;
            Weakness = AttackType.Green;
            ShieldColor = ShieldInstancer.WhiteShield;
            ThreatLevel = 0.5f;
            RotationSpeed = 0.5f;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(SpearTurretCard.STurretSize));
            Add(new SpearTurretGun());
        }

        protected override void Upgrade()
        {
            MaxEngagementDistance *= 1.5f;

            base.Upgrade();
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 3;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Turret4");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Red || attackType == AttackType.Green)
                damage /= 4f;

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
