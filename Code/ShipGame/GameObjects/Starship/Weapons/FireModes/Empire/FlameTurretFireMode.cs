using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class FlameTurretFireMode : FireMode
    {
        public FlameTurretFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "FlameTurretFire";
            FireVolume = 0.1f;

            //Ammo
            MaxROF = 100;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 3;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 1.5f;
            ModifierFactor = 1;
            LifeTime = (int)(FlameTurretCard.EngagementDistance / BulletSpeed * 1000f / 60f);
            MaxHits = 1;
            attackType = AttackType.Red;
        }


        public override Bullet getBullet()
        {
            FireVolume = 0.1f;
            return new FlameTurretBullet();
        }
    }
}
