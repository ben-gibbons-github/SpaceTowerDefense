using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Outcast : UnitShip
    {
        int Bounces = 5;
        int BounceTime = 0;
        bool HasCloakField = false;

        public Outcast(int FactionNumber)
            : base(FactionNumber)
        {
            CollisionSound = "HeavyImpact";
            HullToughness = 1f;
            MaxEngagementDistance = 800;
            MinEngagementDistance = 300;
            Acceleration = 0.2f;
            Add(new OutcastGun());
            Add(UnitTag.Light);
            Mass = 1;
            ScoreToGive = 50;
        }

        public override int GetUnitWeight()
        {
            return 4;
        }

        public override void Collide(GameTime gameTime, BasicShipGameObject Other)
        {
            if (Other != null && Other.GetType().Equals(typeof(PlayerShip)) && Other.GetTeam() == WaveManager.ActiveTeam && Other.CanBeTargeted())
                for (int i = 0; i < 2; i++)
                {
                    Damage(100, 100, Vector2.Zero, Other, AttackType.Melee);
                    Other.Damage(100, 100, Vector2.Zero, this, AttackType.Melee);
                }

            base.Collide(gameTime, Other);
        }

        public override void Destroy()
        {
            if (HasCloakField && BasicField.TestFieldClear(Position.get()))
            {
                HasCloakField = false;
                CloakingField c = new CloakingField();
                ParentLevel.AddObject(c);
                c.SetPosition(Position.get());
            }

            base.Destroy();
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.White && HasCloakField && BasicField.TestFieldClear(Position.get()))
            {
                HasCloakField = false;
                CloakingField c = new CloakingField();
                ParentLevel.AddObject(c);
                c.SetPosition(Position.get());
            }

            if (attackType != AttackType.White)
                damage /= 2f;
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
            Acceleration = 0.2f + Level / 12f;

            HasCloakField = true;

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
            return InstanceManager.EmpireBasicIndex + 2;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Ship3");
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.White;
            Resistence = AttackType.Red;
            ShieldColor = ShieldInstancer.RedShield;
            Size.set(new Vector2(60));
            RotationOffsetSpeed = new Vector3(0, 0, 0.05f);
        }
    }
}
