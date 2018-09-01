using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CrystalFighter : CrystalShip
    {
        public CrystalFighter(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "MediumCrystalExplode";
            DeathVolume = 1;
            DeathDistance = 1000;
            DeathExponenent = 1.5f;

            CollisionSound = "HeavyImpact";
            HullToughness = 0.5f;
            ShieldToughness = 0.5f;
            MaxEngagementDistance = 1000;
            MinEngagementDistance = 200;
            Acceleration = 0.2f;
            Add(new CrystalFighterGun());
            Add(UnitTag.Light);
            Mass = 10;
            ScoreToGive = 50;
        }

        public override int GetUnitWeight()
        {
            return 3;
        }

        public override void Update(GameTime gameTime)
        {
            NoShootTime = -1;
            if (StunState != AttackType.Melee)
                FreezeTime = -1;

            base.Update(gameTime);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Melee && attackType != AttackType.Green && attackType != AttackType.White)
            {
                damage -= 0.1f * UnitLevel;
                if (attackType == AttackType.Red)
                    damage -= 0.1f * UnitLevel;
            }
            if (attackType != AttackType.Explosion && attackType != AttackType.Melee)
            {
                damage /= 6;
                if (attackType != AttackType.White)
                    damage /= 2;
                if (attackType == Resistence)
                    return;

                base.Damage(damage, pushTime, Vector2.Zero, Damager, attackType);
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = 1;
            HullToughness = 0.5f + Level / 4;
            ShieldToughness = 0.75f + Level / 4;
            Acceleration = 0.15f + (Level - 1) / 15f;

            base.SetLevel(Level, Mult);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (ShieldDamage < ShieldToughness)
            {
                if (TimesEMPED == 0)
                {
                    FreezeTime = 1600 - 400 * UnitLevel + 1000 * Level;
                    StunState = AttackType.Blue;
                    TimesEMPED++;
                    LastDamager = Damager;
                }
            }
            else
            {
                if (Level > 0)
                {
                    if (TimesEMPED == 0)
                    {
                        FreezeTime = 1600 - 400 * UnitLevel;
                        StunState = AttackType.Blue;
                        TimesEMPED++;
                        LastDamager = Damager;
                    }
                }
                else
                    ShieldFlash(1);
            }
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienBasicIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Ship2");
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.Green;
            Resistence = AttackType.Red;
            Size.set(new Vector2(60));
            ShieldColor = ShieldInstancer.WhiteShield;
            RotationOffsetSpeed = new Vector3(0, 0, 0.05f);
        }
    }
}
