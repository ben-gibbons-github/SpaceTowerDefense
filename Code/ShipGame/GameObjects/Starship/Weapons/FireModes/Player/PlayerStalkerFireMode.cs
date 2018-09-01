using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerStalkerFireMode : FireMode
    {
        public PlayerStalkerFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "Stalker";
            FireVolume = 0.5f;

            //Ammo
            MaxROF = 60;
            MaxBurstTime = 600;
            MaxReloadTime = 0;
            MaxBurstSize = 4;
            MaxClipSize = 1;

            //Creation
            LifeTime = 350;
            BulletSpeed = 15;
            Accuracy = 0;
            BulletCount = 6;
            Damage = 1.5f;
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
            return new PlayerStalkerBullet();
        }
    }
}
