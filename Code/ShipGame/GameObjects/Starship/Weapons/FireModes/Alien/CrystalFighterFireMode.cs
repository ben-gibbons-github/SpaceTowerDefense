using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class CrystalFighterFireMode : FireMode
    {
        public CrystalFighterFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "CrystalFire";
            FireVolume = 0.4f;

            //Ammo
            MaxROF = 100;
            MaxBurstTime = 1200;
            MaxReloadTime = 0;
            MaxBurstSize = 4;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 5f;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = 2000;
            MaxHits = 1;

            //Damage
            Damage = 2f;
            PushTime = 1.5f;
            ModifierFactor = 1;
            PushVelocityMult = 0.1f;
            BulletExplosionDistance = 0;
            BulletExplosionDamage = 0;
            attackType = AttackType.Red;
        }

        public override void SetLevel(float Level)
        {
            BulletSpeed = 4f + Level / 2;
            Damage = (2f + (Level - 1)) / 10;
            base.SetLevel(Level);
        }

        public override float getDirectionPattern(int BulletNumb)
        {
            return MathHelper.ToRadians((BulletNumb - (BulletCount - 1) / 2f) * 10);
        }

        public override Bullet getBullet()
        {
            return new CrystalFighterBullet();
        }
    }
}
