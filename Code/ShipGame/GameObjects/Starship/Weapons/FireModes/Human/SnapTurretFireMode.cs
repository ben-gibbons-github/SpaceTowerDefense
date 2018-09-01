using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SnapTurretFireMode : FireMode
    {
        public SnapTurretFireMode(GunBasic Parent)
        {
            //Ammo
            MaxROF = 400;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 8;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 1;
            ModifierFactor = 1;
            LifeTime = (int)(SnapTurretCard.EngagementDistance / BulletSpeed * 1000f / 60f);
            LifeTime = -1;
            MaxHits = 1;
            BulletExplosionDamage = 1;
            BulletExplosionDistance = 150;
            attackType = AttackType.Blue;
        }

        public override Bullet getBullet()
        {
            return new ShutDownBullet();
        }
    }
}
