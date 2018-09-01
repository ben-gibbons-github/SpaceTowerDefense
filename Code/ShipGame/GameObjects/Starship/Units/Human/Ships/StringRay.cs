using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Stingray : UnitShip
    {
        public static LinkedList<UnitTurret> MarkedTurrets = new LinkedList<UnitTurret>(); 

        FireMode stingRayEmpFireMode;

        public Stingray(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyHumanExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            CollisionSound = "HeavyImpact";
            HullToughness = 75f;
            ShieldToughness = 25f;
            MaxEngagementDistance = 800;
            MinEngagementDistance = 150;
            Acceleration = 0.05f;
            Add(new StingRayGun());
            Add(UnitTag.Heavy);
            Add(UnitTag.Human);

            stingRayEmpFireMode = new StingRayEmpFireMode();
            stingRayEmpFireMode.SetParent(this);

            Mass = 10;
            ScoreToGive = 50;
        }

        public override int GetUnitWeight()
        {
            return 4;
        }

        public override void Update(GameTime gameTime)
        {
            NoShootTime = -1;
            FreezeTime = -1;

            if (stingRayEmpFireMode.Ammo > 0 && CurrentAttackTarget != null && 
                CurrentAttackTarget.GetType().IsSubclassOf(typeof(UnitTurret)) && !MarkedTurrets.Contains((UnitTurret)CurrentAttackTarget))
            {
                stingRayEmpFireMode.SetLevel(UnitLevel);
                stingRayEmpFireMode.Fire(Logic.ToAngle(CurrentAttackTarget.Position.get() - Position.get()));
                stingRayEmpFireMode.Ammo = 0;
                MarkedTurrets.AddLast((UnitTurret)CurrentAttackTarget);
            }
            base.Update(gameTime);
        }

        public override void Create()
        {
            StunTime = 0;
            WeaknessStunTime = 200;
            base.Create();
            Size.set(new Vector2(120));

            Weakness = AttackType.Red;
            Resistence = AttackType.Blue;
            ShieldColor = ShieldInstancer.BlueShield;
            EnergyToGive = 1000;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (TimesEMPED == 0)
            {
                if (Level > 0)
                {
                    FreezeTime = 3000;
                    StunState = AttackType.Blue;
                    LastDamager = Damager;
                }
                else
                {
                    FreezeTime = 1000;
                    StunState = AttackType.Blue;
                    LastDamager = Damager;
                }
                TimesEMPED++;
            }
        }
        /*
        protected override void AISearch(GameTime gameTime)
        {
            CurrentAttackTarget = null;
            float BestDistance = 1000000;

            foreach (Basic2DObject o in FactionManager.SortedUnits[WaveManager.ActiveTeam])
                if (o != this)
                {
                    float d = Vector2.Distance(getPosition(), o.getPosition());
                    if (PreferPlayer && o.GetType().Equals(typeof(PlayerShip)))
                    {
                        BasicShipGameObject s = (BasicShipGameObject)o;
                        if (s.GetTeam() == WaveManager.ActiveTeam && !s.Dead && !PathFindingManager.CollisionLine(Position.get(), o.Position.get()))
                            CurrentAttackTarget = s;
                    }
                    else if (d < BestDistance)
                    {
                        BasicShipGameObject s = (BasicShipGameObject)o;
                        if (s.GetTeam() == WaveManager.ActiveTeam && s.ShutDownTime < 1 && !s.Dead && !s.IsAlly(this))
                            if (d / s.ThreatLevel < BestDistance && (!s.GetType().IsSubclassOf(typeof(UnitTurret)) || !MarkedTurrets.Contains((UnitTurret)s)) &&
                                !PathFindingManager.CollisionLine(Position.get(), s.Position.get()))
                            {
                                BestDistance = d / s.ThreatLevel;
                                CurrentAttackTarget = s;
                            }
                    }
                }
        }
        */

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = 10 * Level;
            HullToughness = 1 + Level;
            ShieldToughness = 1 + Level;
            Acceleration = 0.1f + Level / 40f;

            base.SetLevel(Level, Mult);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Melee && attackType != AttackType.Explosion)
            {
                if (attackType != AttackType.Red)
                    damage -= 0.5f;
                else
                    damage -= 0.1f;

                if (attackType != AttackType.White)
                    damage /= 3;

                if (attackType == AttackType.Green)
                {
                    damage -= 0.5f * UnitLevel;
                    damage /= 20;
                }
                if (attackType == AttackType.Blue)
                    damage /= 2;

                base.Damage(damage, pushTime, Vector2.Zero, Damager, attackType);
            }
            else
                base.Damage(damage, pushTime, pushSpeed, Damager, attackType);

        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 4;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship5");
        }

    }
}
