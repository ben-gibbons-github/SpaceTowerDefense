using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BabyCrusherFireMode : FireMode
    {
        public BabyCrusherFireMode()
        {
            //Sound
            FireSound = null;

            //Ammo
            MaxROF = 4000;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 6f;
            Accuracy = 0;
            BulletCount = 9;
            ModifierFactor = 1;
            LifeTime = 4000;
            MaxHits = 1;

            //Damage
            Damage = 4f;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0;
            attackType = AttackType.Red;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return (float)(Math.PI * BulletNumb / 4.5f);
        }

        public override Bullet getBullet()
        {
            return new BabyCrusherBullet();
        }
    }
}
