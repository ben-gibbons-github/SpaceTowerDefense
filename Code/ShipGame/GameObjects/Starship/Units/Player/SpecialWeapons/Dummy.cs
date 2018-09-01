using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Dummy : UnitShip
    {
        static Color WhiteColor = new Color(0.1f, 0.1f, 0.1f);

        int ParticleTimer = 0;
        int MaxParticleTimer = 100;

        int TimeAlive = 0;
        int LifeTime = 4000;

        public Dummy(int FactionNumber)
            : base(FactionNumber)
        {
            ThreatLevel = 2;
            Add(UnitTag.Light);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(60));
        }

        public override void Update(GameTime gameTime)
        {
            TimeAlive += gameTime.ElapsedGameTime.Milliseconds;
            if (TimeAlive > LifeTime)
                BlowUp();
            else
            {
                ParticleTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (ParticleTimer > MaxParticleTimer)
                {
                    ParticleTimer -= MaxParticleTimer;
                    ParticleManager.CreateParticle(new Vector3(Position.X(), 0, Position.Y()), Vector3.Zero, WhiteColor, Size.X() * (1 + 0.5f * Rand.F()), 0);
                }
            }

            base.Update(gameTime);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            return;
        }
    }
}
