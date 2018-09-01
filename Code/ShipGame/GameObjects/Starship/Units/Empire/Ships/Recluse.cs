using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Recluse : UnitShip
    {
        Vector2 EMPPosition;
        int Bounces = 2;
        int BounceTime = 0;

        public Recluse(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "SmallHumanExplode";
            DeathVolume = 1;
            DeathDistance = 800;
            DeathExponenent = 1.5f;

            Add(UnitTag.Light);
            Mass = 1;
            //Add(new RecluseGun());
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(40));

            Weakness = AttackType.Blue;
            Resistence = AttackType.Green;
            ShieldColor = ShieldInstancer.GreenShield;
        }

        public override bool SnapBounce()
        {
            if (Bounces > 0)
            {
                SoundManager.Play3DSound("ShieldBounce", new Vector3(Position.X(), Y, Position.Y()),
                    0.5f, 500, 1);
                ShieldFlash(1);

                Bounces = 0;
                return false;
            }
            else
                return true;
        }

        public override bool BulletBounces(Bullet b)
        {
            if (b.attackType != Weakness)
            {
                if (BounceTime > 0)
                    return true;
                else if (Bounces > 0)
                {
                    Bounces--;
                    BounceTime = 150;
                    return true;
                }
            }
            return false;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            Bounces = 0;
            EMPPosition = Position.get();
            CanCloak = false;
            base.EMP(Damager, Level);
        }

        public override void Update(GameTime gameTime)
        {
            BounceTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (FreezeTime > 0 && StunState == AttackType.Blue)
                Damage(gameTime.ElapsedGameTime.Milliseconds / 1000f, 10, EMPPosition - Position.get(), LastDamager, AttackType.Melee);

            base.Update(gameTime);
        }

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = Level * 2.5f;
            HullToughness = 0.5f;
            ShieldToughness = 0;
            Acceleration = (0.35f + (Level - 1) / 10f) * 1.4f;
            Bounces = ((int)Math.Ceiling(Level));

            base.SetLevel(Level, Mult);
        }

        public override int GetIntType()
        {
            return InstanceManager.EmpireBasicIndex + 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Ship1");
        }
    }
}
