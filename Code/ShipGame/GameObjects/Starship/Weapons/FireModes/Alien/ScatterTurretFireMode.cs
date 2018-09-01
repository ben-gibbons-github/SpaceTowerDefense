using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ScatterTurretFireMode : FireMode
    {
        public ScatterTurretFireMode()
        {
            //Sound
            FireSound = "ScatterTurretFire";
            FireVolume = 0.25f;

            //Ammo
            MaxROF = 300;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 4;
            LifeTime = (int)(BeamTurretCard.EngagementDistance / BulletSpeed * 1000f / 60f);
            Accuracy = 0;
            BulletCount = 1;
            Damage = 10f;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.Green;
            BulletExplosionDistance = 300;
            BulletExplosionDamage = 2f;
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return MathHelper.ToRadians((BulletNumb - (BulletCount - 1) / 2f) * 5);
        }

        public override Bullet getBullet()
        {
            return new ScatterTurretBeam();
        }
    }
}
