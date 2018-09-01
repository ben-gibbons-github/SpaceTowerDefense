using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class VampireFireMode : FireMode
    {
        public VampireFireMode()
        {
            //Sound
            FireSound = "VampireFire";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 100;
            MaxBurstTime = 3000;
            MaxReloadTime = 1;
            MaxBurstSize = 3;
            MaxClipSize = 3;

            //Creation
            BulletSpeed = 5;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 2000;
            MaxHits = 1;

            //Damage
            Damage = 1.5f;
            PushTime = 6;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0;
            attackType = AttackType.Blue;
        }

        public override void SetLevel(float Level)
        {
            BulletSpeed = 3 + Level;
            Damage = (0.75f + (Level * 2.2f - 2.2f) / 2.25f) * 0.5f;
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            return new VampireBullet();
        }
    }
}
