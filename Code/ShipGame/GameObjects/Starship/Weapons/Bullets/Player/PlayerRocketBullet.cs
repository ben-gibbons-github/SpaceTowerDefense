using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerRocketBullet : Bullet
    {
        static Color ParticleColor = new Color(0.25f, 0.1f, 0.05f);
        static Color ParticleColor2 = new Color(0.5f, 0.25f, 0.1f);
        static int MaxSearchTime = 200;
        static int SearchDistance = 500;
        static float TurnSpeed = 0.01f;

        int SearchTime = 0;
        UnitBasic AttackTarget;
        float SpeedL;


        public override void Create()
        {
            ShouldDodge = true;
            base.Create();
            Size.set(new Vector2(20));
        }

        public override void SetSpeed(Vector2 Speed)
        {
            SpeedL = Speed.Length();
            base.SetSpeed(Speed);
        }

        public override void Destroy()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, ParticleColor, 20, 5);


            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1400, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 800, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1400, 4);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1400, 7);

            for (int i = 0; i < 2; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 1.5f, new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 15, 30, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 5, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);

            Armed = true;
            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            SearchTime += gameTime.ElapsedGameTime.Milliseconds;
            if (SearchTime > MaxSearchTime || (AttackTarget != null && AttackTarget.Dead))
            {
                SearchTime -= MaxSearchTime;
                AttackTarget = null;
                float BestDistance = SearchDistance;

                QuadGrid quad = Parent2DScene.quadGrids.First.Value;
                foreach(Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(SearchDistance * 2)))
                    if (o.GetType().IsSubclassOf(typeof(UnitBasic)))
                    {
                        UnitBasic s = (UnitBasic)o;

                        if (!s.Dead && !s.IsAlly(ParentUnit))
                        {
                            float d = Vector2.Distance(Position.get(), o.Position.get());

                            if (d < BestDistance)
                            {
                                BestDistance = d;
                                AttackTarget = s;
                            }
                        }
                    }
            }

            if (AttackTarget != null)
                Speed = Logic.ToVector2(Logic.Clerp(Logic.ToAngle(Speed), Logic.ToAngle(AttackTarget.Position.get() - Position.get()),
                    TurnSpeed * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f)) * SpeedL;

            int Mult = 4;
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 2);
            base.Update(gameTime);
        }

        public override float getDamage(BasicShipGameObject s, float Mult)
        {
            if (s.TestTag(UnitTag.Building))
            {
                if (s.TestTag(UnitTag.Ring))
                    Mult *= PlayerWeapon.GetRingMult();
                else
                    Mult *= PlayerWeapon.GetTurretMult();
            }
            else if (s.TestTag(UnitTag.Player))
                Mult *= PlayerWeapon.GetPlayerMult();

            return base.getDamage(s, Mult);
        }
    }
}
