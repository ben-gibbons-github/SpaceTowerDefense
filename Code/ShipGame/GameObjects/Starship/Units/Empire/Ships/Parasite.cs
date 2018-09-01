using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Parasite : UnitShip
    {
        int Bounces = 5;
        int BounceTime = 0;

        public Parasite(int FactionNumber)
            : base(FactionNumber)
        {
            HullToughness = 1f;
            MaxEngagementDistance = 600;
            MinEngagementDistance = 200;
            Acceleration = 0.2f;
            Add(new ParasiteGun());
            Add(UnitTag.Light);
            Mass = 1;
            ScoreToGive = 12;
        }

        public override void Update(GameTime gameTime)
        {
            BounceTime -= gameTime.ElapsedGameTime.Milliseconds;
            base.Update(gameTime);
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

        public override void SetLevel(float Level, float Mult)
        {
            CollisionDamage = 2 *Level;
            HullToughness = 0.25f + Level / 5;
            Acceleration = 0.125f + Level * 0.075f;
            ShieldToughness = 0;
            Bounces = ((int)Math.Ceiling(Level));

            base.SetLevel(Level, Mult);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (Level > 0 && TimesEMPED == 0)
            {
                FreezeTime = 1600 - 400 * UnitLevel;
                StunState = AttackType.Blue;
                TimesEMPED++;
                LastDamager = Damager;
            }

            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.EmpireBasicIndex + 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Ship2");
        }

        public override void Create()
        {
            base.Create();
            Weakness = AttackType.Green;
            Resistence = AttackType.Red;
            ShieldColor = ShieldInstancer.RedShield;
            Size.set(new Vector2(50));
            RotationOffsetSpeed = new Vector3(0, 0, 0.05f);
        }
    }
}
