using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SplinterTurretFireMode : FireMode
    {
        public SplinterTurretFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "SplinterTurretFire";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 300;
            MaxBurstTime = 4000;
            MaxReloadTime = 0;
            MaxBurstSize = 3;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 8;
            LifeTime = (int)(SplinterTurretCard.EngagementDistance / BulletSpeed * 1000f / 60f);
            Accuracy = 0;
            BulletCount = 1;
            Damage = 1f;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.Red;
            BulletExplosionDistance = 150;
            BulletExplosionDamage = 0.25f;
        }

        public override Bullet getBullet()
        {
            SplinterTurretBeam b = new SplinterTurretBeam();
            
            return b;
        }
    }
}
