using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ImmortalFireMode : FireMode
    {
        public ImmortalFireMode()
        {
            //Sound
            FireSound = "ImmortalFire";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 6000;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 8f;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 1400;
            MaxHits = 1;

            //Damage
            Damage = 12f;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0;
            attackType = AttackType.Blue;
        }

        public override void SetLevel(float Level)
        {
            Damage = (2f + (Level - 1)) * 6;
            base.SetLevel(Level);
        }


        public override Bullet getBullet()
        {
            return new ImmortalBullet();
        }
    }
}
