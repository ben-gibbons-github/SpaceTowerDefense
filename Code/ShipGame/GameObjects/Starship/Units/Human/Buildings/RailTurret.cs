using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class RailTurret : UnitTurret
    {
        public RailTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 15;
            HullToughness = 50;
            MaxEngagementDistance = RailTurretCard.EngagementDistance;
            MaxBuildTime = 10000;
            Resistence = AttackType.Blue;
            Weakness = AttackType.Red;
            ShieldColor = new Color(0.5f, 1, 0.5f);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(RailTurretCard.STurretSize));
            Add(new RailTurretGun());
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanUnitIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Turret3");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Melee)
            {
                damage *= 0.2f;
                Guns[0].ReadyROF();
                ParticleManager.CreateParticle(new Vector3(Position.X(), 0, Position.Y()), Vector3.Zero, Color.LimeGreen, Size.X(), 0);
            }
            if (attackType == AttackType.Red)
                damage *= 0.25f;

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
