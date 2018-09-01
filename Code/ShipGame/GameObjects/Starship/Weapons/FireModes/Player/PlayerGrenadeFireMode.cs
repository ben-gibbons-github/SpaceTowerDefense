using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerGrenadeFireMode : FireMode
    {
        public PlayerGrenadeFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "Grenade";
            FireVolume = 0.4f;

            //Ammo
            MaxROF = 400;
            MaxBurstTime = 2000;
            MaxReloadTime = 0;
            MaxBurstSize = 3;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 7f;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 1000;
            MaxHits = 1;

            //Damage
            Damage = 6;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 200;
            BulletExplosionDamage = 3f;
            attackType = AttackType.Red;
        }

        public override void SetLevel(float Level)
        {
            Damage = 2f + (Level - 1);
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            return new PlayerGrenadeBullet();
        }
    }
}
