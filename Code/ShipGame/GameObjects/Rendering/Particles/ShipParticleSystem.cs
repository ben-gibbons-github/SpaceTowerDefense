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
    public class ShipParticleSystem : BasicParticleSystem
    {
        ParticleVertexColor[] particles;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        int firstActiveParticle;
        int firstNewParticle;
        int firstFreeParticle;
        int firstRetiredParticle;

        int drawCounter;
        Random random = new Random();

        string TexturePath;
        float ParticleDuration;
        int MaxParticles;
        float RotateSpeed;
        float StartSize;
        float EndSize;
        Texture2D ParticleTexture;

        Effect ParticleEffect;
        Deferred3DEffect ParticleHolder;

        public ShipParticleSystem(int MaxParticles, float ParticleDuration, float RotateSpeed, string TexturePath, float StartSize, float EndSize)
        {
            this.TexturePath = TexturePath;
            this.MaxParticles = MaxParticles;
            this.ParticleDuration = ParticleDuration;
            this.RotateSpeed = RotateSpeed;
            this.StartSize = StartSize;
            this.EndSize = EndSize;

            LoadTexture();
            this.ParticleEffect = ParticleManager.ParticleEffect.Clone();
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
            ParticleEffect.Parameters["RotateSpeed"].SetValue(RotateSpeed);
            ParticleEffect.Parameters["Texture"].SetValue(ParticleTexture);
            ParticleEffect.Parameters["StartSize"].SetValue(StartSize);
            ParticleEffect.Parameters["EndSize"].SetValue(EndSize);
        }

        public override void LoadTexture()
        {
            ParticleTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/Particles/" + TexturePath + ParticleManager.Quality.ToString());
        }

        public void CreateArray()
        {
            particles = new ParticleVertexColor[MaxParticles * 4];

            for (int i = 0; i < MaxParticles; i++)
            {
                particles[i * 4 + 0].Corner = new Short2(-1, -1);
                particles[i * 4 + 1].Corner = new Short2(1, -1);
                particles[i * 4 + 2].Corner = new Short2(1, 1);
                particles[i * 4 + 3].Corner = new Short2(-1, 1);
            }

            vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, ParticleVertexColor.VertexDeclaration,
                                                   MaxParticles * 4, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[MaxParticles * 6];

            for (int i = 0; i < MaxParticles; i++)
            {
                indices[i * 6 + 0] = (ushort)(i * 4 + 0);
                indices[i * 6 + 1] = (ushort)(i * 4 + 1);
                indices[i * 6 + 2] = (ushort)(i * 4 + 2);

                indices[i * 6 + 3] = (ushort)(i * 4 + 0);
                indices[i * 6 + 4] = (ushort)(i * 4 + 2);
                indices[i * 6 + 5] = (ushort)(i * 4 + 3);
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
                float particleAge = Level.Time - particles[firstActiveParticle * 4].TimeSize.X;

                if (particleAge < ParticleDuration)
                    break;

                particles[firstActiveParticle * 4].TimeSize.X = drawCounter;

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
                int age = drawCounter - (int)particles[firstRetiredParticle * 4].TimeSize.X;

                if (age < 3)
                    break;

                firstRetiredParticle++;

                if (firstRetiredParticle >= MaxParticles)
                    firstRetiredParticle = 0;
            }
        }

        public void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertexColor.SizeInBytes;

            if (firstNewParticle < firstFreeParticle)
            {
                // If the new particles are all in one consecutive range,
                // we can upload them all in a single call.
                vertexBuffer.SetData(firstNewParticle * stride * 4, particles,
                                     firstNewParticle * 4,
                                     (firstFreeParticle - firstNewParticle) * 4,
                                     stride, SetDataOptions.NoOverwrite);
            }
            else
            {
                // If the new particle range wraps past the end of the queue
                // back to the start, we must split them over two upload calls.
                vertexBuffer.SetData(firstNewParticle * stride * 4, particles,
                                     firstNewParticle * 4,
                                     (MaxParticles - firstNewParticle) * 4,
                                     stride, SetDataOptions.NoOverwrite);

                if (firstFreeParticle > 0)
                {
                    vertexBuffer.SetData(0, particles,
                                         0, firstFreeParticle * 4,
                                         stride, SetDataOptions.NoOverwrite);
                }
            }

            // MoveControl the particles we just uploaded from the new to the active queue.
            firstNewParticle = firstFreeParticle;
        }

        public override void AddParticle(Vector3 position, Vector3 velocity, Color color, float Size)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= MaxParticles)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == firstRetiredParticle)
                return;
            double horizontalAngle = random.NextDouble() * MathHelper.TwoPi;

            for (int i = 0; i < 4; i++)
            {
                particles[firstFreeParticle * 4 + i].Position = position;
                particles[firstFreeParticle * 4 + i].Velocity = velocity;
                particles[firstFreeParticle * 4 + i].ParticleColor = color;
                particles[firstFreeParticle * 4 + i].TimeSize.X = Level.Time;
                particles[firstFreeParticle * 4 + i].TimeSize.Y = Size;
            }

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
                        Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * 4, (firstFreeParticle - firstActiveParticle) * 4,
                                                     firstActiveParticle * 6, (firstFreeParticle - firstActiveParticle) * 2);
                    }
                    else
                    {
                        // If the active particle range wraps past the end of the queue
                        // back to the start, we must split them over two draw calls.
                        Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * 4, (MaxParticles - firstActiveParticle) * 4,
                                                     firstActiveParticle * 6, (MaxParticles - firstActiveParticle) * 2);

                        if (firstFreeParticle > 0)
                        {
                            Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                         0, firstFreeParticle * 4,
                                                         0, firstFreeParticle * 2);
                        }
                    }
                }
            }

            drawCounter++;
        }
    }
}
