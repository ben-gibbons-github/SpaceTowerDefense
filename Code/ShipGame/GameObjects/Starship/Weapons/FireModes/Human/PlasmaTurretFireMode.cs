using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlasmaTurretFireMode : FireMode
    {
        public PlasmaTurretFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "PlasmaTurretFire";
            FireVolume = 0.5f;

            //Ammo
            MaxROF = 200;
            MaxBurstTime = 600;
            MaxReloadTime = 0;
            MaxBurstSize = 3;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 5;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 0.5f;
            ModifierFactor = 1;
            LifeTime = (int)(PlasmaTurretCard.EngagementDistance / BulletSpeed * 1000f / 60f);
            MaxHits = 1;
            attackType = AttackType.Red;
        }


        public override Bullet getBullet()
        {
            return new PlasmaTurretBullet();
        }
    }
}
