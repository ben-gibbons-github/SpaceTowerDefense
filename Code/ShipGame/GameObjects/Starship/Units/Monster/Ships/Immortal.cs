using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Immortal : UnitShip
    {
        float RegenerationRate = 0;
        FireMode fireMode;
        bool DeathWave = true;

        public Immortal(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyMonsterExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            CollisionSound = "CrusherImpact";
            HullToughness = 0.5f;
            ShieldToughness = 0.5f;
            MaxEngagementDistance = 500;
            MinEngagementDistance = 200;
            Acceleration = 0.15f;
            Add(new ImmortalGun());
            Add(UnitTag.Heavy);
            Mass = 5f;

            fireMode = new ImmortalDeathFireMode();
            fireMode.SetParent(this);
            ScoreToGive = 20;
        }

        public override void Update(GameTime gameTime)
        {
            fireMode.Update(gameTime);

            if (HullDamage > 0 && (FreezeTime < 0 || StunState != Weakness))
            {
                HullDamage -= RegenerationRate * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                if (HullDamage < 0)
                    HullDamage = 0;
            }

            base.Update(gameTime);
        }

        public override void SetLevel(float Level, float Mult)
        {
            RegenerationRate = 0.005f * Level;
            CollisionDamage = 3;
            HullToughness = (2 + Level) * 0.5f;
            ShieldToughness = 0;
            Acceleration = 0.15f + 0.05f * Level;

            base.SetLevel(Level, Mult);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            DeathWave = attackType != AttackType.Explosion;
            if (attackType != AttackType.Explosion)
            {
                damage /= 3;
                if (attackType != AttackType.Red && attackType != AttackType.White)
                    damage -= 0.2f * UnitLevel;
                if (attackType == AttackType.Green)
                {
                    damage -= 4f * UnitLevel;
                    damage /= UnitLevel;
                }
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override void BlowUp()
        {
            if (!Dead && DeathWave)
                fireMode.Fire(Rotation.getAsRadians());

            base.BlowUp();
        }

        public override void Collide(GameTime gameTime, BasicShipGameObject Other)
        {
            if (Other != null && !Other.Dead && Other.GetType().IsSubclassOf(typeof(UnitTurret)) && Other.GetTeam() == WaveManager.ActiveTeam)
                for (int i = 0; i < 2; i++)
                {
                    Damage(1000, 0, Vector2.Zero, Other, AttackType.Explosion);
                    Other.Damage(1000, 0, Vector2.Zero, this, AttackType.Explosion);
                }

            base.Collide(gameTime, Other);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (Level > 0 && TimesEMPED == 0)
            {
                FreezeTime = 3000;
                StunState = AttackType.Blue;
                LastDamager = Damager;
                TimesEMPED++;
            }
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.MonsterBasicIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Monster/Ship2");
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.Red;
            Resistence = AttackType.Blue;
            ShieldColor = new Color(0.5f, 0.5f, 1);
            Size.set(new Vector2(110));
        }
    }
}
