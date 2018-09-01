using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CrystalBattleCruiser : CrystalShip
    {
        public CrystalBattleCruiser(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyCrystalExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            CollisionSound = "HeavyImpact";
            HullToughness = 2;
            ShieldToughness = 2;
            Acceleration = 0.075f;
            Add(UnitTag.Medium);
            Add(new CrystalBattleCruiserGun());
            MaxEngagementDistance = 1200;
            MinEngagementDistance = 200;
            Mass = 10;
            ScoreToGive = 50;
        }

        public override void Update(GameTime gameTime)
        {
            NoShootTime = -1;
            if (StunState != AttackType.Melee)
                FreezeTime = -1;
            base.Update(gameTime);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = 1 + Level / 8;
            HullToughness = 0.5f + Level / 2;
            ShieldToughness = 1 + Level / 2;
            Acceleration = 0.1f + Level / 20f;

            base.SetLevel(Level, Mult);
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.Red;
            Resistence = AttackType.Blue;
            Size.set(new Vector2(120));
            ShieldColor = ShieldInstancer.WhiteShield;
            RotationOffsetSpeed = new Vector3(0, 0, 0.05f);
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienBasicIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Ship3");
        }

        public override int GetUnitWeight()
        {
            return 4;
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Melee && attackType != AttackType.Red && 
                attackType != AttackType.Explosion && attackType != AttackType.White)
            {
                damage -= 0.05f * UnitLevel;
                if (attackType == AttackType.Green)
                    damage -= 0.05f * UnitLevel;
            }
            if (attackType != AttackType.Explosion && attackType != AttackType.Melee)
            {
                damage /= 6;
                if (attackType != AttackType.White)
                    damage /= 3;
                if (attackType == Resistence)
                    return;

                base.Damage(damage, pushTime, Vector2.Zero, Damager, attackType);
            }
            else
                base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
