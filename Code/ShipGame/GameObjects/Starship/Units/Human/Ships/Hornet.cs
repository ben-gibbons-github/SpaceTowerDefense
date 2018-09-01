using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Hornet : UnitShip
    {
        public Hornet(int FactionNumber)
            : base(FactionNumber)
        {
            HullToughness = 0.5f;
            ShieldToughness = 0.5f;
            MaxEngagementDistance = 500;
            MinEngagementDistance = 200;
            Acceleration = 0.3f;
            Add(new HornetGun());
            Add(UnitTag.Light);
            Mass = 2.5f;
            ScoreToGive = 12;
            Add(UnitTag.Human);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Blue)
                damage /= 4;
            if (attackType != AttackType.Melee)
                damage -= 0.1f * UnitLevel;
            if (attackType == AttackType.Red)
                damage /= UnitLevel + 1;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = 2 * Level;
            HullToughness = 0.5f + Level / 4;
            ShieldToughness = 0;
            Acceleration = 0.25f + (Level - 1) / 20;

            base.SetLevel(Level, Mult);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (TimesEMPED == 0)
            {
                FreezeTime = 1600 - 400 * UnitLevel + Level * 1000;
                StunState = AttackType.Blue;
                TimesEMPED++;
                LastDamager = Damager;
            }
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship2");
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.Green;
            Resistence = AttackType.Red;
            Size.set(new Vector2(50));
            ShieldColor = ShieldInstancer.RedShield;
        }
    }
}
