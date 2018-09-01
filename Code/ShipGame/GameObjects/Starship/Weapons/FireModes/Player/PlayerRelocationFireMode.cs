using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerRelocationFireMode : FireMode
    {
        public PlayerRelocationFireMode()
        {
            //Sound
            FireSound = "Special";
            FireVolume = 0.5f;

            //Ammo
            MaxROF = 400;
            MaxBurstTime = 3000;
            MaxReloadTime = 0;
            MaxBurstSize = 3;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 5f;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 20000;
            MaxHits = 1;

            //Damage
            Damage = 3;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 100;
            BulletExplosionDamage = 1;
            attackType = AttackType.Red;
        }

        public override void SetLevel(float Level)
        {
            Damage = 2f + (Level - 1);
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            return new PlayerRelocationBullet();
        }
    }
}
