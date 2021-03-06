﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerRocketFireMode : FireMode
    {
        public PlayerRocketFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "Rocket";
            FireVolume = 0.5f;

            //Ammo
            MaxROF = 400;
            MaxBurstTime = 3000;
            MaxReloadTime = 0;
            MaxBurstSize = 3;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 10f;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 2000;
            MaxHits = 1;

            //Damage
            Damage = 10;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 300;
            BulletExplosionDamage = 4;
            attackType = AttackType.Red;
        }

        public override void SetLevel(float Level)
        {
            Damage = 2f + (Level - 1);
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            return new PlayerRocketBullet();
        }
    }
}
