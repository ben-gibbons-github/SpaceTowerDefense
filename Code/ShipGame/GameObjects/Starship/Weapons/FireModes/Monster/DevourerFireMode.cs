using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class DevourerFireMode : FireMode
    {
        public DevourerFireMode()
        {
            //Sound
            FireSound = null;

            //Ammo
            MaxROF = 0;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 2.5f;
            Accuracy = 0;
            BulletCount = 12;
            ModifierFactor = 1;
            LifeTime = 1500;
            MaxHits = 1;
            Ammo = 1;

            //Damage
            Damage = 1.5f;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0;
            attackType = AttackType.Blue;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return (float)(Math.PI * BulletNumb * 2 / BulletCount);
        }

        public override Bullet getBullet()
        {
            return new DevourerBullet();
        }
    }
}
