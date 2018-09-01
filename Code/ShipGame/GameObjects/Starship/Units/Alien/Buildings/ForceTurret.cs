using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ForceTurret : UnitTurret
    {
        int MaxPauseTime = 1000;

        float TargetRotation;

        int PauseTime = 0;

        public ForceTurret(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "CrystalTurretExplode";
            ShieldToughness = 80;
            HullToughness = 120;
            MaxEngagementDistance = ForceTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.Green;
            Weakness = AttackType.Blue;
            ShieldColor = ShieldInstancer.BlueShield;
            ThreatLevel = 0.5f;
            RotationSpeed = 5;

            Add(new ForceTurretGun());
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(ForceTurretCard.STurretSize));
        }

        protected override void AI(GameTime gameTime)
        {
            return;
        }

        public override void Update(GameTime gameTime)
        {
            if (Dead || VirusTime > 0 || ShutDownTime > 0 || GetTeam() != WaveManager.ActiveTeam)
            {
                base.Update(gameTime);
                return;
            }

            if (Rotation.getAsRadians() > TargetRotation)
            {
                Guns[0].FireModes[0].SetParent(this);
                Guns[0].FireModes[0].Fire(TargetRotation - (float)Math.PI / 2);
                TargetRotation += (float)(Math.PI / 2);

                if (TargetRotation > Math.PI * 1000)
                {
                    TargetRotation -= (float)(Math.PI * 1000);
                    Rotation.add((float)(Math.PI * 1000));
                }

                PauseTime = MaxPauseTime;
            }
            else 
            {
                PauseTime -= gameTime.ElapsedGameTime.Milliseconds;
                if (PauseTime < 1)
                {
                    Rotation.add(RotationSpeed); 
                    RotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z);
                }
            }
            base.Update(gameTime);
        }



        protected override void Upgrade()
        {
            MaxPauseTime /= 2;
            MaxEngagementDistance *= 1.5f;
            RotationSpeed *= 2;
            HullToughness *= 2;
            ShieldToughness *= 2;

            base.Upgrade();
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Turret3");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Melee)
                damage *= 0.25f;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
