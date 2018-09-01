using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public abstract class UnitBuilding : UnitBasic
    {
        public const float BuildingRingSizeMult = 2.5f;
        protected static Texture2D DeadIcon;

        public static bool Loaded = false;
        protected bool ShouldRebuild = false;
        public bool IsUpdgraded = false;
        public int UpgradeCost = 500;

        public UnitBuilding(int FactionNumber) :
            base(FactionNumber)
        {

        }

        public override bool StopsBullet(BasicShipGameObject Other)
        {
            return base.StopsBullet(Other) && WaveManager.ActiveTeam == GetTeam();
        }

        public override void NewWave()
        {
            if (WaveManager.GameSpeed < 2 || WaveManager.CurrentWave % 2 > 0)
            ShouldRebuild = true;
            base.NewWave();
        }

        public override void NewWaveEvent()
        {
            if (ShouldRebuild)
            {
                HullDamage = 0;
                ShieldDamage = 0;
                ShouldRebuild = false;
            }
            base.NewWaveEvent();
        }

        public override void Create()
        {
            base.Create();
            Add(UnitTag.Building);
            RotationMatrix = Matrix.Identity;
            Load();
            Position.ChangeEvent = ChangePosition;
            Size.ChangeEvent = ChangePosition;
        }

        private void ChangePosition()
        {
            SetQuadGridPosition();
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateRotationY(Rotation.getAsRadians()) * Matrix.CreateTranslation(new Vector3(Position.X(), Y, Position.Y()));
        }

        public override bool AllowInteract(PlayerShip p)
        {
            return !IsUpdgraded && p.FactionNumber == FactionNumber;
        }

        public override void Interact(PlayerShip p)
        {
            IsUpdgraded = true;
            FactionManager.AddCells(p.FactionNumber, -UpgradeCost);

            HullDamage = 0;
            ShieldDamage = 0;

            Upgrade();

            base.Interact(p);
        }

        protected virtual void Upgrade()
        {
            SoundManager.Play3DSound("PlayerLevelUp",
                new Vector3(Position.X(), 0, Position.Y()), 0.25f, 1000, 2);
        }

        public override bool CanInteract(PlayerShip p)
        {
            return FactionManager.CanAfford(p.FactionNumber, UpgradeCost);
        }

        public override int getMaxInteractionTime()
        {
            return 300;
        }

        public override void Update2(GameTime gameTime)
        {
            if (GetIntType() != -1)
            InstanceManager.EmitParticle(GetIntType(), new Vector3(Position.X(), Y, Position.Y()), ref RotationMatrix, 0, Size.X(), 1);
            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateRing(Position3, Size.X() * BuildingRingSizeMult, GetTeam());
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z) * Matrix.CreateTranslation(Position3);
            
            if (FreezeTime > 0 && StunState != AttackType.None)
            {
                switch (StunState)
                {
                    case AttackType.Blue:
                        ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(0.25f, 0.25f, 1), Size.X() * 5 * Rand.F(), 1);
                        break;
                    case AttackType.Red:
                        ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.25f, 0.25f), Size.X() * 5 * Rand.F(), 1);
                        break;
                    case AttackType.Green:
                        ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(0.25f, 1, 0.25f), Size.X() * 5 * Rand.F(), 1);
                        break;
                }
            }
        }

        new public static void Load()
        {
            if (!Loaded)
            {
                DeadIcon = AssetManager.Load<Texture2D>("Textures/ShipGame/Dead");
            }
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            float PreviousShieldDamage = Math.Min(ShieldDamage, ShieldToughness);
            float PreviousHullDamage = Math.Min(HullDamage, HullToughness);

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);

            float DamageAmount = (Math.Min(ShieldDamage, ShieldToughness) - PreviousShieldDamage) + (Math.Min(HullDamage, HullToughness) - PreviousHullDamage);
            if (DamageAmount > 0)
            {
                if (DamageAmount > 0.25f)
                    TextParticleSystem.AddParticle(new Vector3(Position.X(), Y, Position.Y()), ((int)(DamageAmount * 4)).ToString(), (byte)Damager.GetTeam());

                if (Damager.GetType().IsSubclassOf(typeof(UnitShip)))
                {
                    UnitShip s = (UnitShip)Damager;
                    //if (s.IsGhostMode)
                      //  return;
                }
                
                if (Damager.GetTeam() == NeutralManager.NeutralTeam)
                    FactionManager.AddDamage(DamageAmount * 4);
                else
                    FactionManager.AddDamage(Damager.GetTeam(), DamageAmount * 4);
            }
        }

        protected override void DrawHealthBar(float HealthMult, Vector2 Position, float Size)
        {
            if (!Dead)
                base.DrawHealthBar(HealthMult, Position, Size);
            else
                Render.DrawSprite(DeadIcon, Position, new Vector2(Size), 0);
        }

        public override void BlowUp()
        {
            Destroy();
            base.BlowUp();
        }
    }
}
