using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ForceTurretFireMode : FireMode
    {
        static Color ChargeColor = new Color(0.5f, 0.5f, 1);

        public ForceTurretFireMode()
        {
            //Sound
            FireSound = "ForceFire";
            FireVolume = 0.5f;

            //Ammo
            MaxROF = 100;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;
            MaxChargeTime = 2000;

            //Creation
            LifeTime = 800;
            BulletSpeed = 15;
            Accuracy = 0;
            BulletCount = 1;
            Damage = 1f;
            PushTime = 1;
            ModifierFactor = 1;
            attackType = AttackType.Blue;
        }

        public override void CreateChargeParticles(float A)
        {
            if (ParentUnit == null)
                ParentUnit = Parent.getParent();

            ParticleManager.CreateParticle(new Vector3(ParentUnit.Position.X(), ParentUnit.Y, ParentUnit.Position.Y()), Vector3.Zero,
                ChargeColor, ParentUnit.Size.X() * 3, 1);
            ParticleManager.CreateParticle(new Vector3(ParentUnit.Position.X(), ParentUnit.Y, ParentUnit.Position.Y()), Vector3.Zero,
                ChargeColor, ParentUnit.Size.X(), 0);
        }

        public override Bullet getBullet()
        {
            return new ForceTurretBullet();
        }
    }
}
