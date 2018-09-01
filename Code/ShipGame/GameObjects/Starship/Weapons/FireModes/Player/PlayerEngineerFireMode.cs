using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerEngineerFireMode : FireMode
    {
        public PlayerEngineerFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "Engineer";
            FireVolume = 0.6f;

            //Ammo
            MaxROF = 1400;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 6;
            MaxClipSize = 1;

            //Creation
            LifeTime = 1000;
            BulletSpeed = 5;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 4f;
            BulletExplosionDamage = 1;
            BulletExplosionDistance = 200;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.White;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return MathHelper.ToRadians((BulletNumb - (BulletCount - 1) / 2f) * 10);
        }

        public override Vector2 getPositionPattern(int BulletNumb)
        {
            return Vector2.Zero;
        }

        public override Bullet getBullet()
        {
            return new PlayerEngineerBullet();
        }
    }
}
