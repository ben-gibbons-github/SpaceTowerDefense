using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class SmallBombLauncher : FireMode
    {
        public SmallBombLauncher()
        {
            FireVolume = 1;
            FireSound = "SmallBomb";
            FireDistance = 1000;
            FireExponent = 1;

            //AmmoS
            MaxROF = 1000;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;
            Ammo = 0;

            //Creation
            BulletSpeed = 4;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 4500;
            MaxHits = 1;

            //Damage
            Damage = 0;
            PushTime = 0;
            ModifierFactor = 1;
            PushVelocityMult = 1f;
            BulletExplosionDistance = 1500;
            BulletExplosionDamage = 100;
            attackType = AttackType.Explosion;
        }

        public override Bullet getBullet()
        {
            return new SmallBomb();
        }
    }
}
