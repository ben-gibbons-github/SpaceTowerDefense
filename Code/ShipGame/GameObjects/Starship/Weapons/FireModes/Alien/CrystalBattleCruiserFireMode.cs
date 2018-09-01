using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class CrystalBattleCruiserFireMode : FireMode
    {
        public CrystalBattleCruiserFireMode()
        {
            //Sound
            FireSound = "CrystalFire";
            FireVolume = 0.4f;

            //Ammo
            MaxROF = 200;
            MaxBurstTime = 2000;
            MaxReloadTime = 0;
            MaxBurstSize = 6;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 8f;
            Accuracy = 0;
            BulletCount = 2;
            ModifierFactor = 1;
            LifeTime = 2000;
            MaxHits = 1;

            //Damage
            Damage = 1;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0;
            attackType = AttackType.Red;
        }

        public override void SetLevel(float Level)
        {
            BulletSpeed = (2.5f + Level / 2) * 1.5f;
            Damage = 2f + (Level - 1);
            base.SetLevel(Level);
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return 0;
            //return MathHelper.ToRadians((BulletNumb / 2 - (BulletCount / 2 - 1) / 2f) * 10);
        }

        public override Bullet getBullet()
        {
            return new CrystalBattleCruiserBullet();
        }
    }
}
