using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Crusher : UnitShip
    {
        const float PlaceRotation = (float)(Math.PI * 2);

        float RegenerationRate = 0;
        float PlaceRotationMult;
        float PlaceRotationMultChange = 0.025f;

        int UnitSpawnTime = 0;
        int MaxUnitSpawnTime = 3000;
        int UnitCount = 200;

        public Crusher(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyMonsterExplode";
            DeathVolume = 2;
            DeathDistance = 2000;
            DeathExponenent = 1.5f;

            CollisionSound = "CrusherImpact";
            Add(UnitTag.Heavy);
            IgnoresWalls = true;
            Mass = 1000;
            Solid = false;
            Add(new CrusherGun());
            ScoreToGive = 5000;
        }

        public override int GetUnitWeight()
        {
            return 100;
        }

        private void SpawnUnit(UnitShip s)
        {
            ParentLevel.AddObject(s);
            s.SetLevel(1, 1);

            PlaceRotationMult += PlaceRotationMultChange;
            if (PlaceRotationMult > 1)
                PlaceRotationMult -= 1;

            s.Position.set(Position.get() + Logic.ToVector2(PlaceRotationMult * PlaceRotation) * (Size.X() + s.Size.X()) / 1.95f);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(200));

            Weakness = AttackType.Green;
            Resistence = AttackType.Red;
            ShieldColor = ShieldInstancer.RedShield;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            Damager.BlowUp();
            base.EMP(Damager, Level);
            FreezeTime /= 2;
        }

        public override void Update(GameTime gameTime)
        {
            if (UnitCount > 0)
            {
                UnitSpawnTime += gameTime.ElapsedGameTime.Milliseconds * 6;
                if (UnitSpawnTime > MaxUnitSpawnTime)
                {
                    UnitCount--;
                    UnitSpawnTime -= MaxUnitSpawnTime;
                    SpawnUnit(new Devourer(FactionNumber));
                }
            }

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
            CollisionDamage = 100f * Mult * (1 + Level);
            HullToughness = (3 + (Level - 1)) * 25;
            ShieldToughness = 0;
            Acceleration = 0.05f + (Level - 1) / 15f;

            base.SetLevel(Level, Mult);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.White)
                damage /= UnitLevel * 2;

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
            CurrentAttackTarget = null;
            float BestDistance = 1000000;

            QuadGrid grid = Parent2DScene.quadGrids.First.Value;

            foreach (Basic2DObject o in FactionManager.SortedUnits[WaveManager.ActiveTeam])
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
