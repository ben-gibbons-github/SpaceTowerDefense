using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Scrambler : UnitShip
    {
        int Bounces = 5;
        int BounceTime = 0;

        public Scrambler(int FactionNumber)
            : base(FactionNumber)
        {
            HullToughness = 1f;
            MaxEngagementDistance = 800;
            MinEngagementDistance = 300;
            Acceleration = 0.2f;
            Add(new ScramblerGun());
            Add(UnitTag.Light);
            Mass = 1;
            ScoreToGive = 50;
        }

        public override int GetUnitWeight()
        {
            return 4;
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (CurrentAttackTarget != null)
            {
                if (CurrentAttackTarget.GetType().Equals(typeof(PlayerShip)))
                    MinEngagementDistance = 100;
                else
                    MinEngagementDistance = 300;
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override void Update(GameTime gameTime)
        {
            BounceTime -= gameTime.ElapsedGameTime.Milliseconds;
            base.Update(gameTime);
        }

        public override void SetLevel(float Level, float Mult)
        {
            Bounces = (int)Math.Ceiling(Level) * 2;
            CollisionDamage = 1;
            HullToughness = 2f;
            ShieldToughness = 0;
            Acceleration = 0.1f + Level / 30f;

            base.SetLevel(Level, Mult);
        }

        public override bool BulletBounces(Bullet b)
        {
            if (BounceTime > 0)
                return true;
            else if (Bounces > 0)
            {
                Bounces--;
                BounceTime = 150;
                return true;
            }

            return false;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.EmpireBasicIndex + 3;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Ship4");
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.Red;
            Resistence = AttackType.Green;
            ShieldColor = new Color(1, 0.5f, 0.5f);
            Size.set(new Vector2(60));
            RotationOffsetSpeed = new Vector3(0, 0, 0.05f);
        }
    }
}
