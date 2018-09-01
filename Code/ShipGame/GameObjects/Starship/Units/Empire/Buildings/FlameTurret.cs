using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FlameTurret : UnitTurret
    {
        public FlameTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 10;
            HullToughness = 30;
            MaxEngagementDistance = FlameTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Blue;
            Weakness = AttackType.Red;
            ShieldColor = ShieldInstancer.BlueShield;
            ThreatLevel = 0.5f;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(FlameTurretCard.STurretSize));
            Add(new FlameTurretGun());
        }

        public override void BlowUp()
        {
            if (!Dead)
            {
                for (int i = 0; i < 36; i++)
                    Guns[0].FireModes[0].Fire((float)Math.PI * (float)i / 18);

                Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());

                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 10, 4);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 6, 6);

                float BulletExplosionDistance = IsUpdgraded ? 300 : 500;
                float BulletExplosionDamage = IsUpdgraded ? 0.5f : 1f;
                QuadGrid grid = Parent2DScene.quadGrids.First.Value;

                for (int i = 0; i < 2; i++)
                {
                    bool ActivateDeathSound = true;
                    foreach (Basic2DObject o in grid.Enumerate(Position.get(), new Vector2(BulletExplosionDistance * 2)))
                        if (o.GetType().IsSubclassOf(typeof(UnitShip)))
                        {
                            BasicShipGameObject s = (BasicShipGameObject)o;
                            float dist = Vector2.Distance(s.Position.get(), Position.get()) - o.Size.X() / 2;

                            if (dist < BulletExplosionDistance && GetTeam() != s.GetTeam())
                            {
                                float DistMult = 1;
                                if (dist > 0)
                                    DistMult = (BulletExplosionDistance - dist) / BulletExplosionDistance;

                                if (s.GetType().IsSubclassOf(typeof(UnitShip)))
                                {
                                    UnitShip ship = (UnitShip)s;
                                    ship.CanDeathSound = ActivateDeathSound;
                                }
                                s.Damage(DistMult * BulletExplosionDamage, DistMult, 
                                    Vector2.Normalize(s.Position.get() - Position.get()), this, AttackType.Explosion);

                                if (s.Dead)
                                    ActivateDeathSound = false;
                                else if (s.GetType().IsSubclassOf(typeof(UnitShip)))
                                {
                                    UnitShip ship = (UnitShip)s;
                                    ship.CanDeathSound = true;
                                }
                            }
                        }
                }
            }

            base.BlowUp();
        }

        protected override void Upgrade()
        {
            MaxEngagementDistance *= 2f;
            ShieldToughness *= 2f;
            HullToughness *= 2f;
            Guns[0].FireModes[0].BulletSpeed *= 2f;

            base.Upgrade();
        }

        public void LevelUp()
        {
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
            return InstanceManager.EmpireUnitIndex + 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Turret1");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Blue)
                damage /= 8;

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
