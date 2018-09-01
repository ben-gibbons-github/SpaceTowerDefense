using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class RecluseFireMode : FireMode
    {
        public RecluseFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "HornetFire";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 50;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 4f;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 300;
            MaxHits = 1;

            //Damage
            Damage = 0.1f;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0f;
            attackType = AttackType.Green;
        }

        public override void SetLevel(float Level)
        {
            Damage = 2f + (Level - 1);
            base.SetLevel(Level);
        }


        public override Bullet getBullet()
        {
            return new ParasiteBullet();
        }
    }
}
