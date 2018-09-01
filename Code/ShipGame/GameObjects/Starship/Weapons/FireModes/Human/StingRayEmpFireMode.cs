using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class StingRayEmpFireMode : FireMode
    {
        float Level;

        public StingRayEmpFireMode()
        {
            //Sound
            FireSound = "Special";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 500;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 6;
            MaxClipSize = 6;
            Ammo = 1;

            //Creation
            BulletSpeed = 4;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 1.5f;
            ModifierFactor = 1;
            LifeTime = 20000;
            MaxHits = 1;
            attackType = AttackType.Blue;
        }

        public override void SetLevel(float Level)
        {
            this.Level = Level;
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            ShutDownBullet s = new ShutDownBullet();
            s.Level = Level;
            return s;
        }
    }
}
