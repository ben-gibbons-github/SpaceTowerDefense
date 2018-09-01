using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FlamingChunkSystem : BasicParticleSystem
    {
        Model ChunkModel;
        Effect ChunkEffect;
        Texture2D ChunkTexture;

        EffectParameter WorldParam;
        EffectParameter ViewParam;
        EffectParameter ProjectionParam;
        EffectTechnique ForwardInstancedTechnique;

        FlamingChunk[] Chunks;
        int ChunkCount = 0;
        int MaxChunks = 0;
        int Timer = 0;
        int PreviousTimer = 0;
        bool AlternatingControll;
        static FlamingChunkSystem self;

        public static void AddParticle(Vector3 Position, Vector3 Velocity, Vector3 Gravity,
            Vector3 Rot, Vector3 RotSpeed, float Size, float GlowSizeMult,
            Vector3 MinColor, Vector3 MaxColor, int ParticleType, float ParticleSizeMult)
        {
            if (self.ChunkCount < self.MaxChunks)
            {
                self.Chunks[self.ChunkCount].Create(self.Timer, Position, Velocity, Gravity,
                    Rot, RotSpeed, Size, GlowSizeMult, MinColor, MaxColor, ParticleType, ParticleSizeMult);
                self.Chunks[self.ChunkCount++].Update(self.Timer, self.PreviousTimer);
            }
        }

        public override void Clear()
        {
            ChunkCount = 0;
            base.Clear();
        }

        public FlamingChunkSystem(int MaxChunks)
        {
            self = this;
            this.MaxChunks = MaxChunks;

            Chunks = new FlamingChunk[MaxChunks];
            for (int i = 0; i < MaxChunks; i++)
                Chunks[i] = new FlamingChunk();

            ChunkModel = AssetManager.Load<Model>("Models/ShipGame/World/Chunk");

            ChunkEffect = AssetManager.LoadEffect("Effects/Tex");
            WorldParam = ChunkEffect.Parameters["World"];
            ViewParam = ChunkEffect.Parameters["View"];
            ProjectionParam = ChunkEffect.Parameters["Projection"];
            ForwardInstancedTechnique = ChunkEffect.Techniques["ForwardInstancedTechnique"]; 

            ChunkTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/World/ChunkSurface_Color");
        }

        private void ApplyEffectParameters()
        {
            ChunkEffect.Parameters["Texture"].SetValue(ChunkTexture);
            WorldParam.SetValue(Matrix.Identity);
        }

        private void Destroy(int Index)
        {
            if (ChunkCount > 1)
            {
                FlamingChunk temp = Chunks[Index];
                Chunks[Index] = Chunks[ChunkCount - 1];
                Chunks[ChunkCount - 1] = temp;
            }
            ChunkCount--;
        }

        public override void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;

            for (int i = 0; i < ChunkCount; i++)
            {
                FlamingChunk c = Chunks[i];
                if (Timer < c.StartTime + FlamingChunk.LifeTime)
                    ParticleManager.CreateParticle(c.GlowPosition, Vector3.Zero,
                        new Color(Logic.RLerp(c.MinColor, c.MaxColor)),
                        c.Size * c.GlowSizeMult * (1 - (Timer - c.StartTime) / (float)FlamingChunk.LifeTime), 1);
            }

            AlternatingControll = !AlternatingControll;
            if (!AlternatingControll)
                return;

            while (ChunkCount > 0 && Timer > Chunks[0].StartTime + FlamingChunk.LifeTime)
                Destroy(0);

            if (Timer > 300000)
            {
                Timer = 0;
                PreviousTimer = 0;
                ChunkCount = 0;
            }
            else
            {
                if (ChunkCount > 0)
                {
                    for (int i = 0; i < ChunkCount; i++)
                        Chunks[i].Update(Timer, PreviousTimer);

                    PreviousTimer = Timer;
                }
                else
                {
                    Timer = 0;
                    PreviousTimer = 0;
                }
            }
        }
    }
}
