using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Mite : UnitShip
    {
        Vector2 EMPPosition;
        int Bounces = 1;
        int BounceTime = 0;

        public Mite(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "SmallHumanExplode";
            DeathVolume = 1;
            DeathDistance = 800;
            DeathExponenent = 1.5f;

            Add(UnitTag.Light);
            ScoreToGive = 5;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(25));

            Weakness = AttackType.Blue;
            Resistence = AttackType.Green;
            ShieldColor = ShieldInstancer.GreenShield;

            RotationOffsetSpeed = new Vector3(0, 0.05f, 0);
        }

        public override bool BulletBounces(Bullet b)
        {
            if (b.attackType != Weakness)
            {
                if (BounceTime > 0)
                    return true;
                else if (Bounces > 0)
                {
                    Bounces--;
                    BounceTime = 150;
                    return true;
                }
            }
            return false;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            Bounces = 0;
            EMPPosition = Position.get();
            CanCloak = false;
            base.EMP(Damager, Level);
        }

        public override void Update(GameTime gameTime)
        {
            BounceTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (FreezeTime > 0 && StunState == AttackType.Blue)
                Damage(gameTime.ElapsedGameTime.Milliseconds / 1000f, 10, EMPPosition - Position.get(), this, AttackType.Melee);

            base.Update(gameTime);
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = 0.5f * Level;
            HullToughness = 0.25f + (Level - 1) * 0.15f;
            ShieldToughness = 0;
            Acceleration = (0.25f + (Level - 1) / 25f);
            Bounces = Level > 1.35f ? 1 : 0;

            base.SetLevel(Level, Mult);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != Weakness && attackType != AttackType.Explosion && attackType != AttackType.Melee)
                attackType = Resistence;

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override int GetIntType()
        {
            return InstanceManager.EmpireBasicIndex + 4;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Ship5");
        }
    }
}
