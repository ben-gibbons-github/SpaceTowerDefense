using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CrystalKnight : CrystalShip
    {
        public CrystalKnight(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyCrystalExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            CollisionSound = "HeavyImpact";
            HullToughness = 2;
            ShieldToughness = 2;
            Acceleration = 0.2f;
            Add(UnitTag.Medium);
            Add(new CrystalKnightGun());
            MaxEngagementDistance = 800;
            MinEngagementDistance = 300;
            Mass = 5;
            ScoreToGive = 50;
        }

        public override int GetUnitWeight()
        {
            return 4;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = 1 + Level / 8;
            HullToughness = 0.5f + Level / 2;
            ShieldToughness = 1 + Level / 2;
            Acceleration = (0.05f + Level / 10f) * 1.25f;

            base.SetLevel(Level, Mult);
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.None;
            Resistence = AttackType.Blue;
            Size.set(new Vector2(100));
            ShieldColor = ShieldInstancer.WhiteShield;
            RotationOffsetSpeed = new Vector3(0, 0, 0);
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienBasicIndex + 5;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Ship6");
        }

        protected override void AISearch(GameTime gameTime)
        {
            CurrentAttackTarget = null;
            float BestDistance = 1000000;

            foreach (Basic2DObject o in FactionManager.SortedUnits[WaveManager.ActiveTeam])
                if (o != this)
                {
                    float d = Vector2.Distance(getPosition(), o.getPosition());
                    if (d < BestDistance && o.GetType().IsSubclassOf(typeof(BasicShipGameObject)) && !o.GetType().Equals(typeof(PlayerShip)))
                    {
                        BasicShipGameObject s = (BasicShipGameObject)o;
                        if (s.GetTeam() == WaveManager.ActiveTeam && !s.Dead && !s.IsAlly(this) && s.CanBeTargeted())
                            if (d / s.ThreatLevel < BestDistance && !PathFindingManager.CollisionLine(Position.get(), s.Position.get()))
                            {
                                BestDistance = d / s.ThreatLevel;
                                CurrentAttackTarget = s;
                            }
                    }
                }
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Explosion)
            {
                if (damage > 0.25f && ShieldDamage < ShieldToughness)
                {
                    damage = 0.25f;
                    ShieldFlash(1);
                }
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
            FreezeTime = 0;
        }
    }
}
