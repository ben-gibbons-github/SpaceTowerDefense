using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerRaidFireMode : FireMode
    {
        public PlayerRaidFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "Raid";
            FireVolume = 0.3f;

            //Ammo
            MaxROF = 600;
            MaxBurstTime = 800;
            MaxReloadTime = 0;
            MaxBurstSize = 3;
            MaxClipSize = 1;

            //Creation
            LifeTime = 500;
            BulletSpeed = 10;
            Accuracy = 0;
            BulletCount = 3;
            Damage = 1.5f;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.Green;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return MathHelper.ToRadians((BulletNumb - (BulletCount - 1) / 2f) * 10);
        }

        public override Bullet getBullet()
        {
            return new PlayerRaidBullet();
        }
    }
}
