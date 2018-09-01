using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class CrystalKnightFireMode : FireMode
    {
        public CrystalKnightFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "CrystalKnightFire";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 2000;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 4f;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 4000;
            MaxHits = 1;

            //Damage
            Damage = 1;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0;
            attackType = AttackType.Blue;
        }

        public override void SetLevel(float Level)
        {
            Damage = 2f + (Level - 1);
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            return new CrystalKnightBullet(Parent.getParent().UnitLevel);
        }
    }
}
