using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ParticleManager
    {
        public enum ShipParticleQuality
        {
            Low,//32
            Med,//64
            High//128
        }

        public static ShipParticleQuality Quality = ShipParticleQuality.Low;
        private static BasicParticleSystem[] ParticleSystems;
        private static LinkedList<BasicParticleSystem> ColorParticleSystem = new LinkedList<BasicParticleSystem>();
        private static LinkedList<BasicParticleSystem> DisplacementParticleSystem = new LinkedList<BasicParticleSystem>();

        public static Effect ParticleEffect;
        private static bool Loaded = false;
        private static bool Ready = false;

        public static DepthStencilState ParticleDepthStencil = DepthStencilState.None;
        private static RingSystem ringSystem;

        public static void LoadTextures(ShipParticleQuality Quality)
        {
            ParticleManager.Quality = Quality;
            foreach (BasicParticleSystem r in ParticleSystems)
                r.LoadTexture();
            ringSystem.LoadTexture();
        }

        public static void Load()
        {
            TextParticleSystem.Load();
            if (!Loaded)
            {
                ParticleEffect = AssetManager.Load<Effect>("Effects/ShipGame/ShipParticles");
                ParticleSystems = new BasicParticleSystem[8];

                ParticleSystems[0] = new ShipParticleSystem(2000, 0.6f, 2, "Smoke", 1, 2f);
                ParticleSystems[1] = new FlareSystem(5000, 20, "Flare");
                ParticleSystems[2] = new ShipParticleSystem(2000, 0.1f, 2, "Smoke", 1, 1f);
                ParticleSystems[3] = new FlamingChunkSystem(100);
                ParticleSystems[4] = new ShipParticleSystem(100, 0.25f, 0, "Ring", 0, 2f);
                ParticleSystems[5] = new ShipParticleSystem(2000, 1f, 0, "Spark", 1, 0);
                ParticleSystems[6] = new ShipParticleSystem(50, 4, 0, "Ring", 0.1f, 2f);
                ParticleSystems[7] = new LineParticleSystem(1000, 10);

                for (int i = 0; i < ParticleSystems.Length; i++)
                    ColorParticleSystem.AddLast(ParticleSystems[i]);

                ringSystem = new RingSystem(250, "Ring");

                Loaded = true;
            }
            else
            {
                foreach (BasicParticleSystem system in ParticleSystems)
                    system.Clear();
            }
        }

        static void SwitchSystemToDisplacement(int Index)
        {
            ColorParticleSystem.Remove(ParticleSystems[Index]);
            DisplacementParticleSystem.Remove(ParticleSystems[Index]);
        }

        public static void Update(GameTime gameTime)
        {
            TextParticleSystem.Update(gameTime);
            foreach (BasicParticleSystem r in ParticleSystems)
                r.Update(gameTime);
            ringSystem.Update(gameTime);
        }

        public static void CreateRing(Vector3 Position, float Size, float Team)
        {
            ringSystem.AddParticle(Position, Size, Team);
            //ringSystem.AddParticle(Position, Vector3.Zero, Color.Blue, Size);
        }

        public static void CreateParticle(Vector3 Position, Vector3 Velocity, Color color, float Size, int ParticleType)
        {
#if EDITOR && WINDOWS
            if (ParticleType < 0 || ParticleType >= ParticleSystems.Length)
                return;
#endif
            if (ParticleType > 6)
                ParticleType = 6;

            ParticleSystems[ParticleType].AddParticle(Position, Velocity, color, Size);
        }

        public static void PreDraw(Camera3D camera)
        {
            if (!Ready)
            {
                ringSystem.ReadyDraw();
                foreach (BasicParticleSystem r in ParticleSystems)
                    r.Ready();

                Ready = true;
            }

            ringSystem.Draw(camera);
        }

        public static void Draw(Camera3D camera)
        {
            foreach (BasicParticleSystem s in ColorParticleSystem)
                s.Draw(camera);
        }

        public static void DrawDistortion(Camera3D camera)
        {
            foreach (BasicParticleSystem s in DisplacementParticleSystem)
                s.Draw(camera);
        }
    }
}
