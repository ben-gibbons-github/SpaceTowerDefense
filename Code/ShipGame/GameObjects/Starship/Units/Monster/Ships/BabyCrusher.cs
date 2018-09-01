using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class BabyCrusher : UnitShip
    {
        float RegenerationRate = 0;
        bool HasDamageField = false;

        public BabyCrusher(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyMonsterExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            CollisionSound = "CrusherImpact";
            MaxEngagementDistance = 400;
            Add(UnitTag.Heavy);
            Mass = 2;
            //Add(new BabyCrusherGun());
            ScoreToGive = 50;
        }

        public override int GetUnitWeight()
        {
            return 4;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(80));

            Weakness = AttackType.Green;
            Resistence = AttackType.Red;
            ShieldColor = ShieldInstancer.RedShield;
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

        public override void Destroy()
        {
            if (HasDamageField && BasicField.TestFieldClear(Position.get()))
            {
                HasDamageField = false;
                DamageBoostField c = new DamageBoostField();
                ParentLevel.AddObject(c);
                c.SetPosition(Position.get());
            }
            

            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            if (HullDamage > 0 && (FreezeTime < 0 || StunState == Weakness))
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
            CollisionDamage = 1f * Mult * (1 + Level);
            HullToughness = (3 + (Level - 1)) * 0.75f;
            ShieldToughness = 0;
            Acceleration = (0.15f + (Level - 1) / 15f) * 0.5f;
            HasDamageField = true;

            base.SetLevel(Level, Mult);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.White && HasDamageField && BasicField.TestFieldClear(Position.get()))
            {
                HasDamageField = false;
                DamageBoostField c = new DamageBoostField();
                ParentLevel.AddObject(c);
                c.SetPosition(Position.get());
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override int GetIntType()
        {
            return InstanceManager.MonsterBasicIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Monster/Ship3");
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
