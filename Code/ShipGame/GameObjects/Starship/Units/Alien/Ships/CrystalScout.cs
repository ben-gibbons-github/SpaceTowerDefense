using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CrystalScout : CrystalShip
    {
        Vector2 EMPPosition;

        public CrystalScout(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "SmallCrystalExplode";
            DeathVolume = 1;
            DeathDistance = 800;
            DeathExponenent = 1.5f;

            Add(UnitTag.Light);
            ScoreToGive = 50;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(50));

            Weakness = AttackType.Blue;
            Resistence = AttackType.Green;
            ShieldColor = ShieldInstancer.WhiteShield;
            Mass = 5;
            RotationOffsetSpeed = new Vector3(0, 0.05f, 0);
        }

        public override int GetUnitWeight()
        {
            return 2;
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Explosion && attackType != AttackType.Melee)
            {
                damage /= 3;
                if (attackType != AttackType.White && attackType != AttackType.Blue)
                {
                    damage -= 0.05f * UnitLevel;
                    damage /= 4;
                }

                if (attackType == Resistence)
                    return;

                base.Damage(damage, pushTime, Vector2.Zero, Damager, attackType);
            }
            else
                base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            EMPPosition = Position.get();
            CanCloak = false;
            base.EMP(Damager, Level);
        }

        public override void Update(GameTime gameTime)
        {
            if (FreezeTime > 0 && StunState == AttackType.Blue)
                Damage(gameTime.ElapsedGameTime.Milliseconds / 1000f, 10, EMPPosition - Position.get(), LastDamager, AttackType.Melee);
            else if (StunState != AttackType.Melee)
                FreezeTime = -1;

            base.Update(gameTime);
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = (6f * Mult * (1 + Level)) + 6;
            HullToughness = 0.5f + (Level - 1) / 2f;
            ShieldToughness = 0.5f + (Level - 1) / 2f;
            Acceleration = (0.35f + (Level - 1) / 20f) * 0.75f;

            base.SetLevel(Level, Mult);
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienBasicIndex + 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Ship1");
        }
    }
}
