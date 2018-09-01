using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class EngineerTurretFireMode : FireMode
    {
        public EngineerTurretFireMode()
        {
            //Sound
            FireSound = "VampireFire";
            FireVolume = 1;

            //Ammo
            MaxROF = 150;
            MaxBurstTime = 1000;
            MaxReloadTime = 0;
            MaxBurstSize = 5;
            MaxClipSize = 5;

            //Creation
            BulletSpeed = 8;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 1f;
            ModifierFactor = 1;
            LifeTime = 2000;
            MaxHits = 1;
            attackType = AttackType.Blue;
        }

        public override Bullet getBullet()
        {
            VampireBullet v = new VampireBullet();
            v.ImpactVolume *= 1.5f;
            v.ImpactDistance *= 2;
            return v;
        }
    }
}
