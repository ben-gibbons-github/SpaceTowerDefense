using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Vampire : UnitShip
    {
        public VampireEmpFireMode EmpLauncher;

        public Vampire(int FactionNumber)
            : base(FactionNumber)
        {
            HullToughness = 2;
            ShieldToughness = 2;
            Acceleration = 0.075f;
            Add(UnitTag.Medium);
            Add(UnitTag.Human);
            Add(new VampireGun());
            MaxEngagementDistance = 600;
            MinEngagementDistance = 200;
            EmpLauncher = new VampireEmpFireMode();
            EmpLauncher.SetParent(this);
            Mass = 5;
            ScoreToGive = 20;
        }

        protected override void AI(GameTime gameTime)
        {
            EmpLauncher.Update(gameTime);
            if (SearchTime > MaxSearchTime)
            {
                PlayerShip TargetPlayer = AISearchForPlayers();
                if (TargetPlayer != null)
                    EmpLauncher.Fire(Logic.ToAngle(TargetPlayer.Position.get() - Position.get())); 
            }

            base.AI(gameTime);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (Level > 0 && TimesEMPED == 0)
            {
                FreezeTime = 1500;
                StunState = AttackType.Blue;
                LastDamager = Damager;
                TimesEMPED++;
            }
            return;
        }

        public override void Update(GameTime gameTime)
        {
            FreezeTime -= gameTime.ElapsedGameTime.Milliseconds;
            base.Update(gameTime);
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = Level;
            HullToughness = 0.75f + Level / 8;
            ShieldToughness = 0.75f + Level / 8;
            Acceleration = 0.05f + Level / 19f;
            MaxEngagementDistance = 300 + Level * 100;

            base.SetLevel(Level, Mult);
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.Red;
            Resistence = AttackType.Blue;
            Size.set(new Vector2(60));
            ShieldColor = ShieldInstancer.BlueShield;
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship3");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            damage -= 0.1f;
            if (attackType != AttackType.Melee && attackType != AttackType.Red && attackType != AttackType.Explosion)
            {
                damage -= 0.1f * (UnitLevel - 1);
                if (attackType != AttackType.White)
                {
                    damage -= 0.2f * UnitLevel;

                    if (attackType == AttackType.Green)
                    {
                        damage -= 1f * UnitLevel;
                        damage /= 4;
                    }

                    damage /= UnitLevel;
                }
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
