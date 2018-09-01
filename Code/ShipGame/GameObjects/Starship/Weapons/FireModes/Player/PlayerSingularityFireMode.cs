using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerSingularityFireMode : FireMode
    {
        public PlayerSingularityFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "Singularity";
            FireVolume = 0.4f;

            //Ammo
            MaxROF = 200;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 6;
            MaxClipSize = 1;

            //Creation
            LifeTime = 800;
            BulletSpeed = 10;
            Accuracy = 0;
            BulletCount = 3;
            Damage = 1f;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.White;
        }

        public override void SetLevel(float Level)
        {
            Damage = 2 + Level / 2;
            base.SetLevel(Level);
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
            return new PlayerSingularityBullet();
        }
    }
}
