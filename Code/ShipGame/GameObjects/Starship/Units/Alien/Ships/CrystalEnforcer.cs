using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CrystalEnforcer : UnitShip
    {
        LinkedList<UnitTurret> Damagers = new LinkedList<UnitTurret>();

        public CrystalEnforcer(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyCrystalExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            CollisionSound = "HeavyImpact";
            HullToughness = 0.5f;
            ShieldToughness = 0.5f;
            MaxEngagementDistance = 400;
            MinEngagementDistance = 200;
            Acceleration = 0.2f;
            Add(UnitTag.Light);
            Mass = 10f;
            ScoreToGive = 50;
        }

        public override int GetUnitWeight()
        {
            return 4;
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 1, 1), Size.X() * 30, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 600, new Color(1, 1, 1), 60, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 1, 1), Size.X() * 8, 5);

            foreach (UnitTurret t in Damagers)
            {
                t.ShutDownTime = Math.Max(t.ShutDownTime, (int)(2000 * UnitLevel));

                Position3 = new Vector3(t.Position.X(), 0, t.Position.Y());

                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 1, 1), t.Size.X() * 10, 4);
                for (int i = 0; i < 30; i++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * 300, new Color(1, 1, 1), 30, 5);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 1, 1), t.Size.X() * 4, 5);
            }

            base.Destroy();
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Explosion)
            {
                if (Damager.GetType().IsSubclassOf(typeof(UnitTurret)) && !Damagers.Contains((UnitTurret)Damager))
                    Damagers.AddLast((UnitTurret)Damager);

                if (damage > 0.5f && ShieldDamage < ShieldToughness)
                {
                    damage = 0.5f;
                    ShieldFlash(1);
                }
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = 1;
            HullToughness = (0.5f + Level / 4) * 4;
            ShieldToughness = (0.75f + Level / 4) * 4;

            base.SetLevel(Level, Mult);
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

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (ShieldDamage < ShieldToughness)
            {
                if (TimesEMPED == 0)
                {
                    FreezeTime = 1600 - 400 * UnitLevel + 1000 * Level;
                    StunState = AttackType.Blue;
                    TimesEMPED++;
                    LastDamager = Damager;
                }
            }
            else
            {
                if (Level > 0)
                {
                    if (TimesEMPED == 0)
                    {
                        FreezeTime = 1600 - 400 * UnitLevel;
                        StunState = AttackType.Blue;
                        TimesEMPED++;
                        LastDamager = Damager;
                    }
                }
                else
                    ShieldFlash(1);
            }
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienBasicIndex + 4;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Ship5");
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.None;
            Resistence = AttackType.Red;
            Size.set(new Vector2(90));
            ShieldColor = ShieldInstancer.WhiteShield;
        }
    }
}
