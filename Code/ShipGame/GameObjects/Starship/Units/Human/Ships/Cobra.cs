using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Cobra : UnitShip
    {
        Vector2 EMPPosition;

        public Cobra(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "SmallHumanExplode";
            DeathVolume = 1;
            DeathDistance = 800;
            DeathExponenent = 1.5f;
            Add(UnitTag.Light);
            Add(UnitTag.Human);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(40));

            Weakness = AttackType.Blue;
            Resistence = AttackType.Green;
            ShieldColor = ShieldInstancer.GreenShield;
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Melee)
                damage -= 0.05f * UnitLevel;
            if (attackType == AttackType.Green)
            {
                damage -= 1.5f + UnitLevel;
                damage /= 4;
            }
            if (attackType == AttackType.Red)
            {
                damage /= (UnitLevel + 1) / 2;
            }

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

            base.Update(gameTime);
        }

        public override void SetLevel(float Level, float Mult)
        {
            RotationSpeed = 0.1f + Level / 25f;
            CollisionDamage = (Level * 4.5f);
            HullToughness = 0.5f + (Level - 1) / 2f;
            ShieldToughness = 0;
            Acceleration = 0.35f + (Level - 1) / 10f;

            base.SetLevel(Level, Mult);
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship1");
        }
    }
}
