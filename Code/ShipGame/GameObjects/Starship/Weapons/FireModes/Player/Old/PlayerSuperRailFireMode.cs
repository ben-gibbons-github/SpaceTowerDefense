using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerSuperRailFireMode : FireMode
    {
        static Color ChargeColor = new Color(1, 0.25f, 0.25f);

        public PlayerSuperRailFireMode(GunBasic Parent)
        {
            //Ammo
            MaxROF = 100;
            MaxBurstTime = 0;
            MaxReloadTime = 0;
            MaxBurstSize = 1;
            MaxClipSize = 1;

            //Creation
            MaxChargeTime = 2000;
            LifeTime = 800;
            BulletSpeed = 15;
            Accuracy = 0;
            BulletCount = 6;
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

        public override Vector2 getPositionPattern(int BulletNumb)
        {
            return new Vector2((BulletNumb - 0.5f) * 10, 0);
        }

        public override Bullet getBullet()
        {
            return new PlayerSuperRailBullet();
        }
    }
}
