using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SplinterTurret : UnitTurret
    {
        public SplinterTurret(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "CrystalTurretExplode";
            ShieldToughness = 4;
            HullToughness = 8;
            MaxEngagementDistance = SplinterTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Blue;
            Weakness = AttackType.Red;
            ShieldColor = ShieldInstancer.BlueShield;
            ThreatLevel = 0.5f;
            RotationSpeed = 1f;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(SplinterTurretCard.STurretSize));
            Add(new SplinterTurretGun());
        }

        protected override void Upgrade()
        {
            MaxEngagementDistance *= 1.5f;
            Guns[0].FireModes[0].BulletSpeed *= 1.5f;
            //RotationSpeed *= 2;

            base.Upgrade();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        { 
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Turret1");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Blue)
                damage /= 16;
            if (Damager.TestTag(UnitTag.Player))
                damage /= 4;

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
