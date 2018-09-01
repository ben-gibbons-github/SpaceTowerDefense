using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerSnipeFireMode : FireMode
    {
        public PlayerSnipeFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "Snipe";
            FireVolume = 0.4f;

            //Ammo
            MaxROF = 400;
            MaxBurstTime = 1500;
            MaxReloadTime = 0;
            MaxBurstSize = 2;
            MaxClipSize = 1;

            //Creation
            LifeTime = 1200;
            BulletSpeed = 8;
            Accuracy = 0;
            BulletCount = 10;
            Damage = 1.5f;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.Blue;
        }

        public override Vector2 getPositionPattern(int BulletNumb)
        {
            return new Vector2((BulletNumb - BulletCount / 2f - 0.5f) * 10, 0);
        }

        public override Bullet getBullet()
        {
            return new PlayerSnipeBullet();
        }
    }
}
