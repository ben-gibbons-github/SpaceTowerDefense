using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class VampireEmpFireMode : FireMode
    {
        public VampireEmpFireMode()
        {
            //Ammo
            MaxROF = 0;
            MaxBurstTime = 0;
            MaxReloadTime = 1;
            MaxBurstSize = 1;
            MaxClipSize = 1;
            Ammo = 1;

            //Creation
            BulletSpeed = 8;
            Accuracy = 0;
            BulletCount = 5;
            ModifierFactor = 1;
            LifeTime = 3000;
            MaxHits = 1;

            //Damage
            Damage = 1;
            PushTime = 0;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0;
            attackType = AttackType.Blue;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return MathHelper.ToRadians((BulletNumb - (BulletCount - 1) / 2f) * 6);
        }

        public override Bullet getBullet()
        {
            return new VampireBullet();
        }
    }
}
