using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ScatterBombFireMode : FireMode
    {
        public ScatterBombFireMode()
        {
            //Sound
            FireSound = "ScatterTurretFire";
            FireVolume = 0.5f;

            //Ammo
            MaxROF = 600;
            MaxBurstTime = 1200;
            MaxReloadTime = 0;
            MaxBurstSize = 3;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 4;
            LifeTime = 1000;
            Accuracy = 0;
            BulletCount = 3;
            Damage = 5f;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.Green;
            BulletExplosionDistance = 200;
            BulletExplosionDamage = 0.5f;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return MathHelper.ToRadians((BulletNumb - (BulletCount - 1) / 2f) * 5);
        }

        public override Bullet getBullet()
        {
            return new ScatterTurretBullet();
        }
    }
}
