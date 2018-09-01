using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class PlasmaTurret : UnitTurret
    {
        bool StartingTurret = false;

        public PlasmaTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 10;
            HullToughness = 30;
            MaxEngagementDistance = PlasmaTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Blue;
            Weakness = AttackType.Red;
            ShieldColor = new Color(0.5f, 0.5f, 1);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(PlasmaTurretCard.STurretSize));
            Add(new PlasmaTurretGun());
        }

        protected override void Upgrade()
        {
            if (!StartingTurret)
            {
                MaxEngagementDistance *= 1.25f;
                ShieldToughness *= 1.5f;
                HullToughness *= 1.5f;
                Guns[0].FireModes[0].BulletSpeed *= 1.25f;
            }

            base.Upgrade();
        }

        public override void Update(GameTime gameTime)
        {
            if (StartingTurret)
                Guns[0].Update(gameTime);

            base.Update(gameTime);
        }

        public void SetAsStarting()
        {
            StartingTurret = true;
            MaxEngagementDistance *= 1.5f;
            ShieldToughness *= 2;
            HullToughness *= 2;
            Guns[0].FireModes[0].BulletSpeed *= 1.5f;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanUnitIndex + 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Turret1");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Blue)
                damage /= 8;

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
