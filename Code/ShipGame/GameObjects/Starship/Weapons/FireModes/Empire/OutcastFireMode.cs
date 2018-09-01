using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class OutcastFireMode : FireMode
    {
        public OutcastFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "OutcastFire";
            FireVolume = 1f;
            FireDistance = 1000;

            //Ammo
            MaxROF = 4000;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 6f;
            Accuracy = 0;
            BulletCount = 5;
            ModifierFactor = 1;
            LifeTime = 3000;
            MaxHits = 1;

            //Damage
            Damage = 5f;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 300;
            BulletExplosionDamage = 0.5f;
            attackType = AttackType.Red;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return MathHelper.ToRadians((BulletNumb - (BulletCount - 1) / 2f) * 10);
        }

        public override void SetLevel(float Level)
        {
            BulletSpeed = 2 + Level / 2;
            Damage = 5f + (Level - 1) * 2.5f;
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            return new OutcastBullet();
        }
    }
}
