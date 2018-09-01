using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Particle3DSample;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace BadRabbit.Carrot
{
    public class LineParticleSystem : BasicParticleSystem
    {
        static LineParticleSystem self;

        LineVertex[] particles;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        int firstActiveParticle;
        int firstNewParticle;
        int firstFreeParticle;
        int firstRetiredParticle;

        int drawCounter;
        Random random = new Random();

        float ParticleDuration;
        int MaxParticles;

        Effect ParticleEffect;
        Deferred3DEffect ParticleHolder;

        public LineParticleSystem(int MaxParticles, float ParticleDuration)
        {
            self = this;
            this.MaxParticles = MaxParticles;
            this.ParticleDuration = ParticleDuration;

            this.ParticleEffect = AssetManager.LoadEffect("Effects/ShipGame/LineParticles");
            ParticleHolder = (Deferred3DEffect)new Deferred3DEffect().Create(ParticleEffect);

            CreateArray();
        }

        public override void Clear()
        {
            firstActiveParticle = 0;
            firstFreeParticle = 0;
            firstNewParticle = 0;
            firstRetiredParticle = 0;
            base.Clear();
        }

        public override void Ready()
        {
            ParticleEffect.Parameters["Duration"].SetValue(ParticleDuration);
        }

        public void CreateArray()
        {
            particles = new LineVertex[MaxParticles * 2];

            vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, LineVertex.VertexDeclaration,
                                                   MaxParticles * 2, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[MaxParticles * 2];

            for (int i = 0; i < MaxParticles; i++)
            {
                indices[i * 2 + 0] = (ushort)(i * 2 + 0);
                indices[i * 2 + 1] = (ushort)(i * 2 + 1);
            }

            indexBuffer = new IndexBuffer(Game1.graphicsDevice, typeof(ushort), indices.Length, BufferUsage.WriteOnly);

            indexBuffer.SetData(indices);
        }

        public override void Update(GameTime gameTime)
        {
            RetireActiveParticles();
            FreeRetiredParticles();

            if (firstRetiredParticle == firstActiveParticle)
                drawCounter = 0;
        }

        public void RetireActiveParticles()
        {
            while (firstActiveParticle != firstNewParticle)
            {
                float particleAge = Level.Time - particles[firstActiveParticle * 2].Time;

                if (particleAge < ParticleDuration)
                    break;

                particles[firstActiveParticle * 2].Time = drawCounter;

                // MoveControl the particle from the active to the retired queue.
                firstActiveParticle++;

                if (firstActiveParticle >= MaxParticles)
                    firstActiveParticle = 0;
            }
        }

        public void FreeRetiredParticles()
        {
            while (firstRetiredParticle != firstActiveParticle)
            {
                int age = drawCounter - (int)particles[firstRetiredParticle * 2].Time;

                if (age < 3)
                    break;

                firstRetiredParticle++;

                if (firstRetiredParticle >= MaxParticles)
                    firstRetiredParticle = 0;
            }
        }

        public void AddNewParticlesToVertexBuffer()
        {
            int stride = LineVertex.SizeInBytes;

            if (firstNewParticle < firstFreeParticle)
            {
                // If the new particles are all in one consecutive range,
                // we can upload them all in a single call.
                vertexBuffer.SetData(firstNewParticle * stride * 2, particles,
                                     firstNewParticle * 2,
                                     (firstFreeParticle - firstNewParticle) * 2,
                                     stride, SetDataOptions.NoOverwrite);
            }
            else
            {
                // If the new particle range wraps past the end of the queue
                // back to the start, we must split them over two upload calls.
                vertexBuffer.SetData(firstNewParticle * stride * 2, particles,
                                     firstNewParticle * 2,
                                     (MaxParticles - firstNewParticle) * 2,
                                     stride, SetDataOptions.NoOverwrite);

                if (firstFreeParticle > 0)
                {
                    vertexBuffer.SetData(0, particles,
                                         0, firstFreeParticle * 2,
                                         stride, SetDataOptions.NoOverwrite);
                }
            }

            // MoveControl the particles we just uploaded from the new to the active queue.
            firstNewParticle = firstFreeParticle;
        }

        public static void AddParticle(Vector3 position, Vector3 position2, Color color)
        {
            self.addParticle(position, position2, color);
        }

        void addParticle(Vector3 position, Vector3 position2, Color color)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= MaxParticles)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == firstRetiredParticle)
                return;

            particles[firstFreeParticle * 2].Position = position;
            particles[firstFreeParticle * 2 + 1].Position = position2;
            particles[firstFreeParticle * 2].Time = Level.Time;
            particles[firstFreeParticle * 2 + 1].Time = Level.Time;
            particles[firstFreeParticle * 2].color = color;
            particles[firstFreeParticle * 2 + 1].color = color;
                
            firstFreeParticle = nextFreeParticle;
        }

        public override void ReadyDraw()
        {
            base.ReadyDraw();
        }

        public override void Draw(Camera3D camera)
        {
            if (vertexBuffer.IsContentLost)
                vertexBuffer.SetData(particles);

            if (firstNewParticle != firstFreeParticle)
                AddNewParticlesToVertexBuffer();

            if (firstActiveParticle != firstFreeParticle)
            {
                Deferred3DEffect effect3D = ParticleHolder;
                effect3D.SetFromCamera(camera);

                Game1.graphicsDevice.BlendState = BlendState.Additive;
                Game1.graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

                effect3D.SetTextureSize(new Vector2(0.5f / Game1.graphicsDevice.Viewport.AspectRatio, -0.5f));
                effect3D.SetTime(Level.Time);
                Game1.graphicsDevice.SetVertexBuffer(vertexBuffer);
                Game1.graphicsDevice.Indices = indexBuffer;

                // Activate the particle effect.
                foreach (EffectPass pass in ParticleEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    if (firstActiveParticle < firstFreeParticle)
                    {
                        // If the active particles are all in one consecutive range,
                        // we can draw them all in a single call.
                        Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0,
                                                     firstActiveParticle * 2, (firstFreeParticle - firstActiveParticle) * 2,
                                                     firstActiveParticle * 2, (firstFreeParticle - firstActiveParticle) * 2);
                    }
                    else
                    {
                        // If the active particle range wraps past the end of the queue
                        // back to the start, we must split them over two draw calls.
                        Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0,
                                                     firstActiveParticle * 2, (MaxParticles - firstActiveParticle) * 2,
                                                     firstActiveParticle * 2, (MaxParticles - firstActiveParticle) * 2);

                        if (firstFreeParticle > 0)
                        {
                            Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0,
                                                         0, firstFreeParticle * 2,
                                                         0, firstFreeParticle * 2);
                        }
                    }
                }
            }

            drawCounter++;
        }
    }
}
