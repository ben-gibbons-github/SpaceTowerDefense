using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class RailTurretFireMode : FireMode
    {
        static Color ChargeColor = new Color(0.25f, 1, 0.25f);

        public RailTurretFireMode(GunBasic Parent)
        {
            //Sound
            FireSound = "RailTurretFire";
            FireVolume = 0.75f;

            //Ammo
            MaxROF = 400;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;
            MaxChargeTime = 0;

            //Creation
            BulletSpeed = 6;
            Accuracy = 0;
            BulletCount = 1;
            ModifierFactor = 1;
            LifeTime = (int)(RailTurretCard.EngagementDistance / BulletSpeed * 1000f / 60f);
            MaxHits = 1;

            //Damage
            Damage = 8;
            ModifierFactor = 1;
            PushTime = 50;
            PushVelocityMult = 1f;
            BulletExplosionDistance = 200;
            BulletExplosionDamage = 0.5f;
            attackType = AttackType.Green;
        }

        public override void CreateChargeParticles(float A)
        {
            if (ParentUnit == null)
                ParentUnit = Parent.getParent();

            ParticleManager.CreateParticle(new Vector3(ParentUnit.Position.X(), ParentUnit.Y, ParentUnit.Position.Y()), Vector3.Zero,
                ChargeColor, ParentUnit.Size.X() * 10 * A, 1);
            ParticleManager.CreateParticle(new Vector3(ParentUnit.Position.X(), ParentUnit.Y, ParentUnit.Position.Y()), Vector3.Zero,
                ChargeColor, ParentUnit.Size.X() * 6 * A, 1);
        }

        public override Bullet getBullet()
        {
            return new RailTurretBullet();
        }
    }
}
