using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BigBomb
    {
        static Color ParticleColor = new Color(0.4f, 0.175f, 0.15f);
        static Color ParticleColor2 = new Color(0.3f, 0.1f, 0.05f);

        public virtual void Trigger(PlayerShip p)
        {
            SoundManager.PlaySound("BigBombExplode2", 1, 0, 0);
            SoundManager.DeafTone();

            p.ShakeScreen(2500);
            LinkedListNode<GameObject> CurrentNode = p.ParentScene.Children.First;

            Vector3 Position3 = new Vector3(p.Position.X(), 0, p.Position.Y());
            for (int i = 0; i < 100; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 20, ParticleColor, 60, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 4000, 5);

            for (int i = 0; i < 20; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3() * 10, new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 40, 30, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

            for (int i = 0; i < 50; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 15, new Color(1, 0.75f, 0.5f), 200, 5);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 2000, 4);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 3000, 4);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 4000, 4);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 500, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 1000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 2000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 3000, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 4000, 5);

            //ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 500, 7);
            //ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 750, 7);
            //ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 1000, 7);

            while (CurrentNode != null)
            {
                GameObject g = CurrentNode.Value;
                if (g.GetType().IsSubclassOf(typeof(UnitShip)))
                {
                    UnitShip u = (UnitShip)g;
                    u.Damage(100000, 0, Vector2.One, p, AttackType.Explosion);
                    u.Damage(100000, 0, Vector2.One, p, AttackType.Explosion);
                    WaveManager.EndWave();
                }

                CurrentNode = CurrentNode.Next;
            }
        }
    }
}
