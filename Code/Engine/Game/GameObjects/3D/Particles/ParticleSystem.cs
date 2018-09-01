using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Particle3DSample;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ParticleSystem : GameObject
    {
        public EffectValue MyEffect;
        public ObjectListValue Emitters;
        public IntValue MaxParticles;
        public BoolValue Additive;
        public BoolValue UseDepth;
        public Vector2Value MinVelocity;
        public Vector2Value MaxVelocity;
        public FloatValue ParticleDuration;
        public EffectParameter DurationParameter;

        ParticleVertex[] particles;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        int firstActiveParticle;
        int firstNewParticle;
        int firstFreeParticle;
        int firstRetiredParticle;

        int drawCounter;
        static Random random = new Random();

        private bool NeedDurationChange = true;


        public override void Create()
        {
#if EDITOR && WINDOWS
            this.AddRightClickEvent("Add Emitter", AddEmitter);
#endif
            Emitters = new ObjectListValue("Emitters", typeof(ParticleEmitter));
            MaxParticles = new IntValue("MaxParticles", 50, CreateArray);
            Additive = new BoolValue("Additive", true);
            UseDepth = new BoolValue("Use Depth", true);
            MinVelocity = new Vector2Value("Min Velocity");
            MaxVelocity = new Vector2Value("Max Velocity", Vector2.One);
            MyEffect = new EffectValue("Effect","ParticleEffect", EffectHolderType.Deferred3D);
            ParticleDuration = new FloatValue("ParticleDuration", 10);
            ParticleDuration.ChangeEvent = DurationChange;
            MaxParticles.ChangeEvent = MaxChange;
            MyEffect.ChangeEvent = EffectChange;
            CreateArray();

            AddTag(GameObjectTag._3DDepthOver);
            AddTag(GameObjectTag.Update);
            base.Create();
        }

        private void DurationChange()
        {
            NeedDurationChange = true;
        }

        private void MaxChange()
        {
            firstActiveParticle = 0;
            firstNewParticle = 0;
            firstFreeParticle = 0;
            firstRetiredParticle = 0;
            CreateArray();
        }

        private void AddEmitter(Button b)
        {
            Emitters.add(Add(new ParticleEmitter()));
        }

        private void EffectChange()
        {
            DurationParameter = MyEffect.findEffectParameter("Duration");
            NeedDurationChange = true;
        }

        private void CreateArray()
        {
            particles = new ParticleVertex[MaxParticles.get() * 4];

            for (int i = 0; i < MaxParticles.get(); i++)
            {
                particles[i * 4 + 0].Corner = new Short2(-1, -1);
                particles[i * 4 + 1].Corner = new Short2(1, -1);
                particles[i * 4 + 2].Corner = new Short2(1, 1);
                particles[i * 4 + 3].Corner = new Short2(-1, 1);
            }

            vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, ParticleVertex.VertexDeclaration,
                                                   MaxParticles.get() * 4, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[MaxParticles.get() * 6];

            for (int i = 0; i < MaxParticles.get(); i++)
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

        public override void UpdateEditor(GameTime gameTime)
        {
            Update(gameTime);
            base.UpdateEditor(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (MyEffect.get() != null)
            {
                AddParticles(gameTime);

                RetireActiveParticles();
                FreeRetiredParticles();

                if (firstRetiredParticle == firstActiveParticle)
                    drawCounter = 0;
            }

            base.Update(gameTime);
        }

        private void AddParticles(GameTime gameTime)
        {
            foreach (ParticleEmitter p in Emitters.Value)
            {
                p.Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (p.Timer > p.Delay.get())
                {
                    p.Timer -= p.Delay.get();
                    AddParticle(p.getRandomPosition(), Vector3.Zero);
                }
            }
        }

        void RetireActiveParticles()
        {

            while (firstActiveParticle != firstNewParticle)
            {
                float particleAge = Level.Time - particles[firstActiveParticle * 4].Time;

                if (particleAge < ParticleDuration.get())
                    break;

                particles[firstActiveParticle * 4].Time = drawCounter;

                // MoveControl the particle from the active to the retired queue.
                firstActiveParticle++;

                if (firstActiveParticle >= (int)MaxParticles.get())
                    firstActiveParticle = 0;
            }
        }

        void FreeRetiredParticles()
        {
            while (firstRetiredParticle != firstActiveParticle)
            {
                int age = drawCounter - (int)particles[firstRetiredParticle * 4].Time;

                if (age < 3)
                    break;

                firstRetiredParticle++;

                if (firstRetiredParticle >= (int)MaxParticles.get())
                    firstRetiredParticle = 0;
            }
        }

        public override void  Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (MyEffect.get() == null)
                return;
            GraphicsDevice device = Game1.graphicsDevice;

            // Restore the vertex buffer contents if the graphics device was lost.
            if (vertexBuffer.IsContentLost)
            {
                vertexBuffer.SetData(particles);
            }

            if (NeedDurationChange && DurationParameter != null && ParticleDuration != null)
            {
                DurationParameter.SetValue(ParticleDuration.get());
                NeedDurationChange = false;
            }

            // If there are any particles waiting in the newly added queue,
            // we'd better upload them to the GPU ready for drawing.
            if (firstNewParticle != firstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }

            // If there are any active particles, draw them now!
            if (firstActiveParticle != firstFreeParticle)
            {
                Deferred3DEffect effect3D = (Deferred3DEffect)MyEffect.Holder;
                effect3D.SetFromCamera(camera);

                device.BlendState = Additive.get() ? BlendState.Additive : BlendState.AlphaBlend;
                device.DepthStencilState = UseDepth.get() ? DepthStencilState.DepthRead : DepthStencilState.None;

                effect3D.SetTextureSize(new Vector2(0.5f / device.Viewport.AspectRatio, -0.5f));
                effect3D.SetTime(Level.Time);
                device.SetVertexBuffer(vertexBuffer);
                device.Indices = indexBuffer;

                // Activate the particle effect.
                foreach (EffectPass pass in MyEffect.get().CurrentTechnique.Passes)
                {
                    pass.Apply();

                    if (firstActiveParticle < firstFreeParticle)
                    {
                        // If the active particles are all in one consecutive range,
                        // we can draw them all in a single call.
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * 4, (firstFreeParticle - firstActiveParticle) * 4,
                                                     firstActiveParticle * 6, (firstFreeParticle - firstActiveParticle) * 2);
                    }
                    else
                    {
                        // If the active particle range wraps past the end of the queue
                        // back to the start, we must split them over two draw calls.
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                     firstActiveParticle * 4, ((int)MaxParticles.get() - firstActiveParticle) * 4,
                                                     firstActiveParticle * 6, ((int)MaxParticles.get() - firstActiveParticle) * 2);

                        if (firstFreeParticle > 0)
                        {
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                         0, firstFreeParticle * 4,
                                                         0, firstFreeParticle * 2);
                        }
                    }
                }
            }

            drawCounter++;
            base.Draw3D(camera, DrawTag);
        }


        /// <summary>
        /// Helper for uploading new particles from our managed
        /// array to the GPU vertex buffer.
        /// </summary>
        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

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
                                     ((int)MaxParticles.get() - firstNewParticle) * 4,
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

        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= (int)MaxParticles.get())
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == firstRetiredParticle)
                return;

            // Adjust the input velocity based on how much
            // this particle system wants to be affected by it.
            //velocity *= settings.EmitterVelocitySensitivity;

            // Add in some random amount of horizontal velocity.
            float horizontalVelocity = MathHelper.Lerp(MinVelocity.get().X,
                                                       MaxVelocity.get().X,
                                                       (float)random.NextDouble());

            double horizontalAngle = random.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            // Add in some random amount of vertical velocity.
            velocity.Y += MathHelper.Lerp(MinVelocity.get().Y,
                                          MaxVelocity.get().Y,
                                          (float)random.NextDouble());

            // Choose four random control values. These will be used by the vertex
            // shader to give each particle a different size, rotation, and color.
            Color randomValues = new Color((byte)random.Next(255),
                                           (byte)random.Next(255),
                                           (byte)random.Next(255),
                                           (byte)random.Next(255));

            for (int i = 0; i < 4; i++)
            {
                particles[firstFreeParticle * 4 + i].Position = position;
                particles[firstFreeParticle * 4 + i].Velocity = velocity;
                particles[firstFreeParticle * 4 + i].Random = randomValues;
                particles[firstFreeParticle * 4 + i].Time = Level.Time;
            }

            firstFreeParticle = nextFreeParticle;
        }


    }
}
