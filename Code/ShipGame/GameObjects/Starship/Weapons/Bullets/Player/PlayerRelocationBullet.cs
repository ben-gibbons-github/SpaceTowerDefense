﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerRelocationBullet : Bullet
    {
        static Color ParticleColor = new Color(0.1f, 0.3f, 0.175f);
        static Color ParticleColor2 = new Color(0.1f, 0.5f, 0.25f);

        public float Level = 1;

        static int MaxSearchTime = 200;
        static int SearchDistance = 300;
        static float TurnSpeed = 0.2f;

        int SearchTime = 0;
        float SpeedL;
        UnitBasic AttackTarget;

        public override void Create()
        {
            ImpactVolume = 1;
            ImpactString = "RelocationImpact";
            ImpactExponent = 1;
            ImpactDistance = 1400;

            base.Create();
            Size.set(new Vector2(16));
        }

        public override void SetSpeed(Vector2 Speed)
        {
            SpeedL = Speed.Length();
            base.SetSpeed(Speed);
        }

        public override void Collide(BasicShipGameObject s)
        {
            if (!s.GetType().Equals(typeof(PlayerShip)) && !s.GetType().Equals(typeof(CrystalWall)))
                base.Collide(s);
        }

        public override void Update(GameTime gameTime)
        {
            int Mult = 4;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 2);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 0);
            FlareSystem.AddLightning(Position3, ParticleColor2, 4, 20, 4, 10);

            SearchTime += gameTime.ElapsedGameTime.Milliseconds;
            if (SearchTime > MaxSearchTime || (AttackTarget != null && !AttackTarget.CanBeTargeted()))
            {
                SearchTime -= MaxSearchTime;
                AttackTarget = null;
                float BestDistance = SearchDistance;

                QuadGrid quad = Parent2DScene.quadGrids.First.Value;
                foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(SearchDistance * 2)))
                    if (o.GetType().IsSubclassOf(typeof(UnitTurret)))
                    {
                        UnitBasic s = (UnitBasic)o;

                        if (s.CanBeTargeted() && !s.IsAlly(ParentUnit))
                        {
                            float d = Vector2.Distance(Position.get(), o.Position.get());

                            if (d < BestDistance && !o.GetType().Equals(typeof(CrystalWall)))
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

            base.Update(gameTime);
        }

        public override void Destroy()
        {
            this.Armed = false;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 2000, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 4000, 5);

            for (int i = 0; i < 2; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3() / 1.5f, new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 150, 30, new Vector3(0.2f, 0.5f, 1), new Vector3(0.2f, 0.5f, 1), 0, 3);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(0.5f, 0.75f, 1), Size.X() * 50, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(0.5f, 0.75f, 1), 200, 5);

            QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

            float MaxDist = 100 * Level;

            foreach (GameObject g in quadGrid.Enumerate(Position.get(), new Vector2(MaxDist * 2)))
                if (g.GetType().IsSubclassOf(typeof(UnitTurret)))
                {
                    UnitTurret s = (UnitTurret)g;
                    if (Vector2.Distance(Position.get(), s.Position.get()) < MaxDist && !s.IsAlly(ParentUnit))
                    {
                        s.Relocate();
                    }
                }

            base.Destroy();
        }
    }
}
