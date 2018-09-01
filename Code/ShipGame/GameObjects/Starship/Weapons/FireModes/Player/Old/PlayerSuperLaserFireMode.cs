using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerSuperLaserFireMode : FireMode
    {
        public PlayerSuperLaserFireMode(GunBasic Parent)
        {
            //Ammo
            MaxROF = 200;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 6;
            MaxClipSize = 1;

            //Creation
            LifeTime = 700;
            BulletSpeed = 15;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 1f;
            PushTime = 1;
            ModifierFactor = 1;
            BulletExplosionDamage = 3;
            BulletExplosionDistance = 300;
            attackType = AttackType.White;
        }

        public override void SetLevel(float Level)
        {
            Damage = 2 + Level / 2;
            base.SetLevel(Level);
        }

        public override Vector2 getPositionPattern(int BulletNumb)
        {
            return Vector2.Zero;
        }

        public override Bullet getBullet()
        {
            return new PlayerSuperLaserBullet();
        }
    }
}
