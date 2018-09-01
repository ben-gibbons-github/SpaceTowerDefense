using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class WindowMissileBullet : Bullet
    {
        static Color ParticleColor = new Color(0.2f, 0.2f, 0.3f);
        static Color ParticleColor2 = new Color(0.5f, 0.5f, 1);


        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(20));
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 80, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 100 + Rand.F() * 100, 1);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 20 + Rand.F() * 40, 2);

            FlareSystem.AddLightning(Position3, ParticleColor2, 10, 20, 3, 5);
            base.Update(gameTime);
        }

        public override void Destroy()
        {
            this.Armed = true;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 600, ParticleColor, 30, 5);

            for (int i = 0; i < 2; i++)
            {
                FlareSystem.AddLightingPoint(Position3, new Vector3(0.3f), new Vector3(0, 0, 1), 25, 40, 5, 10);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1200, 5);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400, 4);
            }

            QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

            foreach (GameObject g in quadGrid.Enumerate(Position.get(), new Vector2(WinderMisslesAbility.ExplosionDistance)))
                if (g.GetType().IsSubclassOf(typeof(UnitShip)))
                {
                    UnitShip s = (UnitShip)g;
                    if (Vector2.Distance(Position.get(), s.Position.get()) < WinderMisslesAbility.ExplosionDistance / 4)
                    {
                        s.StunState = AttackType.White;
                        s.FreezeTime = 1000;
                        s.SetSpeed(Vector2.Normalize(s.Position.get() - Position.get()) * 12 / s.Mass);
                    }
                }

            base.Destroy();
        }
    }
}
