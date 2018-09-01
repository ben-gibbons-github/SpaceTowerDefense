using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class RelocationLauncher : UnitTurret
    {
        static Color ParticleColor = new Color(0.1f, 0.175f, 0.3f);

        int Shots = 0;
        int MaxShots = 2;
        int PauseTime = 0;
        int MaxPauseTime = 5000;
        UnitTurret MissileAttackTarget = null;
        FireMode RelocationFireMode;

        public RelocationLauncher(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 10;
            HullToughness = 15;
            MaxBuildTime = 5000;
            Resistence = AttackType.None;
            Weakness = AttackType.None;
            ShieldColor = ShieldInstancer.WhiteShield;
            RelocationFireMode = new PlayerRelocationFireMode();
            RotationSpeed = 0.5f;
        }

        public override void NewWaveEvent()
        {
            if (WaveManager.ActiveTeam != GetTeam())
            {
                Shots = MaxShots;
                PauseTime = 0;
            }

            base.NewWaveEvent();
        }

        protected override void Upgrade()
        {
            MaxShots *= 3;

            base.Upgrade();
        }

        public override void Update(GameTime gameTime)
        {
            if (Shots > 0)
            {
                PauseTime += gameTime.ElapsedGameTime.Milliseconds;
                if (PauseTime > MaxPauseTime)
                {
                    if (MissileAttackTarget == null || !MissileAttackTarget.CanBeTargeted())
                    {
                        float BestDistance = 100000;
                        foreach (BasicShipGameObject o in FactionManager.SortedUnits[WaveManager.ActiveTeam])
                            if (o.GetType().IsSubclassOf(typeof(UnitTurret)))
                            {
                                float d = Vector2.Distance(Position.get(), o.Position.get());
                                if (d < BestDistance)
                                {
                                    BestDistance = d;
                                    MissileAttackTarget = (UnitTurret)o;
                                }
                            }
                    }
                    else
                    {
                        float TargetRotation = Logic.ToAngle(MissileAttackTarget.Position.get() - Position.get());
                        Rotation.set(MathHelper.ToDegrees(Logic.Clerp(Rotation.getAsRadians(), TargetRotation, RotationSpeed * gameTime.ElapsedGameTime.Milliseconds * 60.0f / 1000.0f)));
                        RotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z);
                        if (Math.Abs(Rotation.getAsRadians() - TargetRotation) < 0.1f)
                        {
                            PauseTime = 0;
                            Shots -= 1;
                            RelocationFireMode.SetParent(this);
                            RelocationFireMode.Fire(TargetRotation);
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(RelocationLauncherCard.STurretSize));
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("ExtraTurrets/Turret4");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (Damager.TestTag(UnitTag.Player))
                damage /= 2;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
