using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class WinderMisslesAbility : SpecialWeapon
    {
        private static float BulletDamage = 0;
        private static float ExplosionDamage = 0;
        public static float ExplosionDistance = 400;
        private static int BulletLifeTime = 1500;
        public static float BulletMaxSpeed = 5;
        public static float BulletStartSpeed = 5;
        public static float BulletAcceleration = 0.6f;
        public static float BulletFriction = 0.985f;
        private static float BulletPushTime = 0;
        private static float BulletVelocityMult = 0.1f;
        public static int FreezeTimeBonus = 5000;

        public WinderMisslesAbility()
        {
            MaxRechargeTime = 6000;
            BulletStartSpeed = 3;
            BulletAcceleration = 0;
        }

        public override void Trigger()
        {
            if (RechargeTime >= MaxRechargeTime)
            {
                RechargeTime = 0;
                WindowMissileBullet b = new WindowMissileBullet();
                ParentShip.ParentLevel.AddObject(b);
                b.SetShipParent(ParentShip);
                b.SetPosition(ParentShip.getPosition());
                b.SetStartingPosition(b.getPosition());
                b.SetSpeed(BulletStartSpeed * Logic.ToVector2(Logic.ToAngle(ParentShip.AdjustedAimPointer - ParentShip.getPosition())));
                b.SetDamage(BulletDamage, BulletPushTime, BulletVelocityMult);
                b.SetAttackType(AttackType.Blue);
                b.SetModifierFactor(1);
                b.SetLifeTime(BulletLifeTime);
                b.SetExplosive(ExplosionDistance, ExplosionDamage);
            }

            base.Trigger();
        }
    }
}
