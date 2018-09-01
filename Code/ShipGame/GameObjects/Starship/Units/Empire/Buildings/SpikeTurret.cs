using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SpikeTurret : UnitTurret
    {
        static float RotationChangeF = 0.01f;

        float RotationOffsetSpeedX;
        float MaxRotationOffsetSpeedX;
        float MaxCollisionDamage;

        public SpikeTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 50;
            HullToughness = 50;
            MaxEngagementDistance = SpikeTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Blue;
            Weakness = AttackType.Green;
            ShieldColor = ShieldInstancer.GreenShield;
            //ThreatLevel = 0.1f;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                MaxRotationOffsetSpeedX = 10;
                MaxCollisionDamage = 100;
                RotationChangeF = 0.1f;
                if (VirusTime > 0 || ShutDownTime > 0)
                {
                    RotationOffsetSpeedX -= RotationChangeF * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                    if (RotationOffsetSpeedX < 0)
                        RotationOffsetSpeedX = 0;
                }
                else
                {
                    RotationOffsetSpeedX += RotationChangeF * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                    if (RotationOffsetSpeedX > MaxRotationOffsetSpeedX)
                        RotationOffsetSpeedX = MaxRotationOffsetSpeedX;
                }
                CollisionDamage = MaxCollisionDamage * RotationOffsetSpeedX / MaxRotationOffsetSpeedX;
                RotationOffsetSpeed = new Vector3(RotationOffsetSpeedX, 0, 0);
            }
            base.Update(gameTime);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(SpikeTurretCard.STurretSize));
        }

        protected override void Upgrade()
        {
            ShieldToughness *= 2f;
            HullToughness *= 2f;
            MaxCollisionDamage *= 2;

            base.Upgrade();
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }
        /*
        public override void BlowUp()
        {
            DeathParticles();
            Destroy();
            InstanceManager.RemoveChild(this);

            float BulletExplosionDistance = 200;
            float BulletExplosionDamage = 1f;
            QuadGrid grid = Parent2DScene.quadGrids.First.Value;

            for (int i = 0; i < 2; i++)
                foreach (Basic2DObject o in grid.Enumerate(Position.get(), new Vector2(BulletExplosionDistance * 2)))
                    if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                    {
                        BasicShipGameObject s = (BasicShipGameObject)o;
                        float dist = Vector2.Distance(s.Position.get(), Position.get()) - o.Size.X() / 2;

                        if (dist < BulletExplosionDistance && GetTeam() != s.GetTeam())
                        {
                            float DistMult = 1;
                            if (dist > 0)
                                DistMult = (BulletExplosionDistance - dist) / BulletExplosionDistance;
                            s.Damage(DistMult * BulletExplosionDamage, DistMult, Vector2.Normalize(s.Position.get() - Position.get()), this, AttackType.Explosion);
                        }
                    }
        }
        */
        public override int GetIntType()
        {
            return InstanceManager.EmpireUnitIndex + 4;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Turret5");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            //if (attackType == AttackType.Melee && damage > 0)
            {
                if (damage > 0.5f && CollisionDamage > 0)
                    damage = 0.5f;
                base.Damage(damage, pushTime, pushSpeed, Damager, AttackType.Melee);
            }
        }
    }
}
