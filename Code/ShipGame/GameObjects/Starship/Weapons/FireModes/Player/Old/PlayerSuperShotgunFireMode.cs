using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerSuperShotgunFireMode : FireMode
    {
        public PlayerSuperShotgunFireMode(GunBasic Parent)
        {
            //Ammo
            MaxROF = 300;
            MaxBurstTime = 1000;
            MaxReloadTime = 0;
            MaxBurstSize = 3;
            MaxClipSize = 1;

            //Creation
            LifeTime = 500;
            BulletSpeed = 15;
            Accuracy = 0;
            BulletCount = 5;
            Damage = 3.5f;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.Blue;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return MathHelper.ToRadians((BulletNumb - (BulletCount - 1) / 2f) * 10);
        }

        public override Bullet getBullet()
        {
            return new PlayerSuperShotGunBullet();
        }
    }
}
