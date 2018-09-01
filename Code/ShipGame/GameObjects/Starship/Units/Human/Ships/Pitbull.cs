using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Pitbull : UnitShip
    {
        static Color ParticleColor = new Color(0.1f, 0.175f, 0.3f);

        Vector2 EMPPosition;
        FireMode fireMode;

        public Pitbull(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "EmpImpact";
            DeathVolume = 1;
            DeathDistance = 1400;
            DeathExponenent = 2f;

            CollisionSound = "PitbullImpact";
            Add(UnitTag.Light);
            Add(UnitTag.Human);
            fireMode = new PitbullFireMode();
            fireMode.SetParent(this);
        }

        public override void Destroy()
        {
            QuadGrid quad = Parent2DScene.quadGrids.First.Value;

            foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(GetEngagementDistance() * 2)))
                if (o.GetType().IsSubclassOf(typeof(UnitBasic)))
                {
                    UnitBasic s = (UnitBasic)o;
                    if (!s.Dead && !s.IsAlly(this) && Vector2.Distance(Position.get(), o.Position.get()) < GetEngagementDistance())
                    {
                        s.ShutDownTime = Math.Max(s.ShutDownTime, (int)(1000 * UnitLevel));
                    }
                }

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * GetEngagementDistance() * 2, ParticleColor, 30, 5);

            for (int i = 0; i < 2; i++)
            {
                FlareSystem.AddLightingPoint(Position3, new Vector3(0.3f), new Vector3(0, 0, 1), GetEngagementDistance(), 40, 5, 10);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, GetEngagementDistance() * 4, 5);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, GetEngagementDistance() * 1.33f, 4);
            }

            base.Destroy();
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(40));

            Weakness = AttackType.Blue;
            Resistence = AttackType.Green;
            ShieldColor = ShieldInstancer.GreenShield;
            Mass = 1;
            RotationOffsetSpeed = new Vector3(0, 0.1f, 0);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            EMPPosition = Position.get();
            base.EMP(Damager, Level);
        }

        public override void Update(GameTime gameTime)
        {
            if (FreezeTime > 0 && StunState == AttackType.Blue)
                Damage(gameTime.ElapsedGameTime.Milliseconds / 1000f, 10, EMPPosition - Position.get(), this, AttackType.Melee);

            base.Update(gameTime);
        }

        public override void Collide(GameTime gameTime, BasicShipGameObject Other)
        {
            if (Other != null && !Other.IsAlly(this) && Other.GetType().IsSubclassOf(typeof(UnitTurret)) && !Other.Dead && Other.ShutDownTime < 1)
                BlowUp();

            base.Collide(gameTime, Other);
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = Level * 2;
            HullToughness = 0.1f;
            ShieldToughness = (0.5f + (Level - 1) / 2f);
            Acceleration = (0.4f + (Level - 1) / 10f) * 1.5f;
            MaxEngagementDistance = 100 * Level;

            base.SetLevel(Level, Mult);
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 5;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship6");
        }

        protected override void AISearch(GameTime gameTime)
        {
            CurrentAttackTarget = null;
            float BestDistance = 1000000;

            foreach (Basic2DObject o in FactionManager.SortedUnits[WaveManager.ActiveTeam])
                if (o != this && o.GetType().IsSubclassOf(typeof(UnitTurret)))
                {
                    float d = Vector2.Distance(getPosition(), o.getPosition());
                    if (d < BestDistance)
                    {
                        UnitTurret s = (UnitTurret)o;
                        if (s.GetTeam() == WaveManager.ActiveTeam && s.ShutDownTime < 1 && !s.Dead && !s.IsAlly(this) && s.Resistence != AttackType.Green)
                            if (d / s.ThreatLevel < BestDistance && !PathFindingManager.CollisionLine(Position.get(), o.Position.get()))
                            {
                                BestDistance = d / s.ThreatLevel;
                                CurrentAttackTarget = s;
                            }
                    }
                }
        }
    }
}
