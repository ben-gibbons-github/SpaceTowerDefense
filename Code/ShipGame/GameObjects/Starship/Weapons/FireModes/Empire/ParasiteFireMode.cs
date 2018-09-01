﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ParasiteFireMode : FireMode
    {
        public ParasiteFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "HornetFire";
            FireVolume = 0.5f;
            FireDistance = 600;

            //Ammo
            MaxROF = 4000;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 4f;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 1300;
            MaxHits = 1;

            //Damage
            Damage = 2f;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 300;
            BulletExplosionDamage = 0.5f;
            attackType = AttackType.Red;
        }

        public override void SetLevel(float Level)
        {
            BulletSpeed = 3 + Level / 2;
            Damage = 2f + (Level - 1);
            base.SetLevel(Level);
        }


        public override Bullet getBullet()
        {
            return new ParasiteBullet();
        }
    }
}
