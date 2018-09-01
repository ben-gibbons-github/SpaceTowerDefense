using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ScramblerFireMode : FireMode
    {
        float Level;

        public ScramblerFireMode()
        {
            //Sound
            FireSound = "Special";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 5000;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            BulletSpeed = 5;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 0.5f;
            ModifierFactor = 1;
            LifeTime = 20000;
            MaxHits = 1;
            attackType = AttackType.Red;
        }

        public override void SetLevel(float Level)
        {
            this.Level = Level;
            base.SetLevel(Level);
        }

        public override Bullet getBullet()
        {
            ScramblerBullet s = new ScramblerBullet();
            s.Level = this.Level;
            return s;
        }
    }
}
