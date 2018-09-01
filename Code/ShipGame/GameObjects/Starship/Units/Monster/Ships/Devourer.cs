using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Devourer : UnitShip
    {
        Vector2 EmpPosition;
        float RegenerationRate = 0;
        FireMode fireMode;
        bool DeathWave = true;


        public Devourer(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "SmallMonsterExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            Add(UnitTag.Light);
            Add(UnitTag.Monster);
            Mass = 1f;
        }

        public override void Create()
        {
            CollisionSound = "DevourerImpact";
            base.Create();
            Size.set(new Vector2(40));

            Weakness = AttackType.Blue;
            Resistence = AttackType.Green;
            ShieldColor = new Color(0.5f, 1, 0.5f);

            fireMode = new DevourerFireMode();
            fireMode.SetParent(this);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            CanCloak = false;
            EmpPosition = Position.get();
            base.EMP(Damager, Level);
        }

        public override void Update(GameTime gameTime)
        {
            if (HullDamage > 0 && (FreezeTime < 0 || StunState != Weakness))
            {
                HullDamage -= RegenerationRate * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                Acceleration += RegenerationRate * gameTime.ElapsedGameTime.Milliseconds * 60 / 10000f;
                if (HullDamage < 0)
                    HullDamage = 0;
            }

            if (FreezeTime > 0 && StunState == AttackType.Blue)
                Damage(gameTime.ElapsedGameTime.Milliseconds / 1000f, 10, EmpPosition - Position.get(), LastDamager, AttackType.Melee);

            fireMode.Update(gameTime);

            base.Update(gameTime);
        }

        public override void SetLevel(float Level, float Mult)
        {
            RegenerationRate = 0.00125f * Level;
            CollisionDamage = 8f * Mult * (1 + Level);
            HullToughness = 0.7f + (Level - 1) / 5f;
            ShieldToughness = 0;
            Acceleration = 0.2f + (Level - 1) / 20f;

            base.SetLevel(Level, Mult);
        }

        public override void BlowUp()
        {
            if (!Dead && DeathWave)
                fireMode.Fire(Rotation.getAsRadians());

            base.BlowUp();
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            DeathWave = attackType != AttackType.Explosion;

            if (attackType != Weakness && attackType != AttackType.Explosion && attackType == AttackType.Melee)
                attackType = Resistence;

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override int GetIntType()
        {
            return InstanceManager.MonsterBasicIndex + 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Monster/Ship1");
        }

        /*
        protected override void AISearch(GameTime gameTime)
        {
            base.AISearch(gameTime);
            return;

            CurrentAttackTarget = null;
            float BestDistance = 1000000;

            QuadGrid grid = Parent2DScene.quadGrids.First.Value;

            foreach (Basic2DObject o in grid.Enumerate(getPosition(), new Vector2(BestDistance) * 2))
                if (o != this && !o.GetType().IsSubclassOf(typeof(UnitShip)))
                {
                    float d = Vector2.Distance(getPosition(), o.getPosition());
                    if (d < BestDistance && o.GetType().IsSubclassOf(typeof(UnitTurret)))
                    {
                        UnitTurret s = (UnitTurret)o;
                        if (s.GetTeam() == WaveManager.ActiveTeam && !s.Dead && !s.IsAlly(this) && s.Resistence != AttackType.Green)
                            if (d / s.ThreatLevel < BestDistance)
                            {
                                BestDistance = d / s.ThreatLevel;
                                CurrentAttackTarget = s;
                            }
                    }
                }
        }
        */
    }
}
