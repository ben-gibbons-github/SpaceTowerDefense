using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SmallBomb : Bullet
    {
        static Color ParticleColor = new Color(0.4f, 0.175f, 0.15f) * 2f;
        static Color ParticleColor2 = new Color(0.3f, 0.1f, 0.05f) * 2f;
        static int MaxSearchTime = 200;

        int BestEnemyCount = 0;
        int SearchTime = 0;

        public override void Create()
        {
            ImpactString = null;
            ImpactVolume = 0;

            base.Create();
            Size.set(new Vector2(32));
        }

        public SmallBomb()
        {
            BulletCanBounce = false;
        }

        public override void Collide(BasicShipGameObject s)
        {
            return;
        }

        public override void Update(GameTime gameTime)
        {
            int Mult = 4;

            SearchTime += gameTime.ElapsedGameTime.Milliseconds;
            if (SearchTime > MaxSearchTime)
            {
                MaxSearchTime -= SearchTime;

                int EnemyCount = 0;

                QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

                foreach (GameObject g in quadGrid.Enumerate(Position.get(), new Vector2(BulletExplosionDistance)))
                    if (g.GetType().IsSubclassOf(typeof(UnitBasic)))
                    {
                        UnitBasic s = (UnitBasic)g;
                        if (Vector2.Distance(Position.get(), s.Position.get()) < BulletExplosionDistance / 8 && !s.IsAlly(ParentUnit) && !s.GetType().IsSubclassOf(typeof(UnitBuilding)) && !s.Dead)
                        {
                            EnemyCount += s.GetUnitWeight();
                        }
                    }
                if (BestEnemyCount > 4 && EnemyCount < BestEnemyCount)
                    Destroy();
                else
                    BestEnemyCount = EnemyCount;
            }

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, 60 * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (70 + Rand.F() * 70) * Mult, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 2);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor2, (10 + Rand.F() * 20) * Mult, 0);
            FlareSystem.AddLightning(Position3, ParticleColor2, 30, 20, 4, 10);

            base.Update(gameTime);
        }

        public override void Destroy()
        {
            SoundManager.PlaySound("SmallBombExplode", 1, 0, 0);
            SoundManager.DeafTone();
            PlayerShip ParentPlayer = (PlayerShip)ParentUnit;
            ParentPlayer.ShakeScreen(20);

            this.Armed = false;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 6, ParticleColor, 20, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 4000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 7);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 300, 7);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 500, 7);

            for (int i = 0; i < 10; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3(), new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 20, 60, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 500, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 10, new Color(1, 0.75f, 0.5f), 200, 5);
            
            QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

            foreach (GameObject g in quadGrid.Enumerate(Position.get(), new Vector2(BulletExplosionDistance)))
                if (g.GetType().IsSubclassOf(typeof(UnitBasic)))
                {
                    UnitBasic s = (UnitBasic)g;
                    if (Vector2.Distance(Position.get(), s.Position.get()) < BulletExplosionDistance / 4 && !s.IsAlly(ParentUnit) && !s.GetType().IsSubclassOf(typeof(UnitBuilding)))
                    {
                        s.SmallBomb(ParentUnit);
                    }
                }

            base.Destroy();
        }
    }
}
