using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class BeamTurret : UnitTurret
    {
        public BeamTurret(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "CrystalTurretExplode";
            ShieldToughness = 30;
            HullToughness = 60;
            MaxEngagementDistance = BeamTurretCard.EngagementDistance;
            Resistence = AttackType.Red;
            Weakness = AttackType.Green;
            ShieldColor = ShieldInstancer.BlueShield;
            ThreatLevel = 0.5f;
            RotationSpeed = 0.2f;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(BeamTurretCard.STurretSize));
            Add(new ScatterTurretGun());
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Turret2");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Red)
                damage *= 0.25f;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
