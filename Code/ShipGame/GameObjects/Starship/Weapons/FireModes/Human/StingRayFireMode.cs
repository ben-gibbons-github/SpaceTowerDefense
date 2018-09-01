using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class StingRayFireMode : FireMode
    {
        public StingRayFireMode()
        {
            //Sound
            FireSound = "VampireFire";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 100;
            MaxBurstTime = 500;
            MaxReloadTime = 0;
            MaxBurstSize = 5;
            MaxClipSize = 5;

            //Creation
            BulletSpeed = 8;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 0.5f;
            ModifierFactor = 1;
            LifeTime = 2000;
            MaxHits = 1;
            attackType = AttackType.Blue;
        }

        public override void SetLevel(float Level)
        {
            BulletSpeed = 3 + Level;
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            return new VampireBullet();
        }
    }
}
