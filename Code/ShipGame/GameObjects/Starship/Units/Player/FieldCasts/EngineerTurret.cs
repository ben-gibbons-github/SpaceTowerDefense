using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class EngineerTurret : UnitShip
    {
        public EngineerTurret(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyHumanExplode";
            DeathVolume = 1;
            DeathDistance = 1400;
            DeathExponenent = 1.5f;

            CollisionSound = "HeavyImpact";
            HullToughness = 15f;
            ShieldToughness = 5f;
            MaxEngagementDistance = 1200;
            MinEngagementDistance = 0;
            Acceleration = 0.05f;
            Add(new EngineerTurretGun());
            Add(UnitTag.Heavy);
            Add(UnitTag.Human);

            Acceleration = 0;
            CollisionDamage = 10;

            Mass = 1000;
            ScoreToGive = 50;
        }

        public override void Create()
        {
            StunTime = 0;
            WeaknessStunTime = 200;
            base.Create();
            Size.set(new Vector2(80));

            Weakness = AttackType.Red;
            Resistence = AttackType.Blue;
            ShieldColor = ShieldInstancer.BlueShield;
            EnergyToGive = 1000;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Melee && attackType != AttackType.Explosion && attackType != AttackType.Red)
            {
                damage -= 0.5f;
                if (attackType == AttackType.Green)
                    damage -= 0.5f * UnitLevel;
                if (attackType == AttackType.Blue)
                    damage /= 2;
            }

            base.Damage(damage, pushTime, Vector2.Zero, Damager, attackType);
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 6;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/EngineerTurret");
        }
    }
}
