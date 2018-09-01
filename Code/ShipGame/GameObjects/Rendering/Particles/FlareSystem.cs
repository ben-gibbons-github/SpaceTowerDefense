using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace BadRabbit.Carrot
{
    public class FlareSystem : BasicParticleSystem
    {
        bool BufferReady = false;
        public Effect FlareEffect;
        Deferred3DEffect ParticleHolder;

        FlareVertex[] particles;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        int firstFreeParticle = 0;
        Texture2D FlareTexture;
        static FlareSystem self;

        LightningPoint[] Points;
        int MaxPoints;
        int PointCount = 0;
        int Timer = 0;

        int MaxParticles;
        Random random = new Random();

        public FlareSystem(int MaxParticles, int MaxPoints, string TexturePath)
        {
            this.MaxPoints = MaxPoints;
            Points = new LightningPoint[MaxPoints];
            for (int i = 0; i < MaxPoints; i++)
                Points[i] = new LightningPoint();

            self = this;
            FlareEffect = AssetManager.LoadEffect("Effects/ShipGame/ShipFlares");
            ParticleHolder = (Deferred3DEffect)new Deferred3DEffect().Create(FlareEffect);
            FlareTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/Particles/" + TexturePath + ParticleManager.Quality.ToString());
            this.MaxParticles = MaxParticles;
            CreateArray();
        }

        public override void Ready()
        {
            FlareEffect.Parameters["Texture"].SetValue(FlareTexture);
        }

        public override void Clear()
        {
            PointCount = 0;
            base.Clear();
        }

        private void CreateArray()
        {
            particles = new FlareVertex[MaxParticles * 4];

            for (int i = 0; i < MaxParticles; i++)
            {
                particles[i * 4 + 0].Corner = new Short2(-1, -1);
                particles[i * 4 + 1].Corner = new Short2(1, -1);
                particles[i * 4 + 2].Corner = new Short2(1, 1);
                particles[i * 4 + 3].Corner = new Short2(-1, 1);
            }

            vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, FlareVertex.VertexDeclaration,
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
            BufferReady = false;
            firstFreeParticle = 0;

            if (PointCount == 0)
                Timer = 0;
            else
            {
                Timer += gameTime.ElapsedGameTime.Milliseconds;

                while (PointCount > 0 && Points[0].StartTime + LightningPoint.LifeTime < Timer)
                {
                    if (PointCount > 1)
                    {
                        LightningPoint temp = Points[0];
                        Points[0] = Points[--PointCount];
                        Points[PointCount] = temp;
                    }
                    else
                        PointCount = 0;
                }

                for (int i = 0; i < PointCount; i++)
                    Points[i].Update(Timer);
            }

            base.Update(gameTime);
        }

        public override void AddParticle(Vector3 position, Vector3 velocity, Color color, float Size)
        {
            if (firstFreeParticle == MaxParticles)
                return;

            for (int i = 0; i < 4; i++)
            {
                particles[firstFreeParticle * 4 + i].Position = position;
                particles[firstFreeParticle * 4 + i].ParticleColor = color;
                particles[firstFreeParticle * 4 + i].Size = Size;
            }

            firstFreeParticle++;
        }

        public static void AddLightning(Vector3 position, Color color, float Size,
            float Spread, int Lines, int LinePop)
        {
            for (int i = 0; i < Lines; i++)
            {
                Vector3 PreviousPosition = position;
                position += Rand.V3() * Spread;

                for (int j = 0; j < LinePop; j++)
                    self.AddParticle(PreviousPosition + (position - PreviousPosition) * (j / (float)LinePop), Vector3.Zero, color, Size);
            }
        }

        public static void AddLightingPoint(Vector3 Position, Vector3 MinColor, Vector3 MaxColor, float Size,
            float Spread, int Lines, int LinePop)
        {
            if (self.PointCount < self.MaxPoints)
                self.Points[self.PointCount++].Create(Position, MinColor, MaxColor, Size, Spread, Lines, LinePop);
        }

        public override void Draw(Camera3D camera)
        {
            if (firstFreeParticle == 0)
                return;

            if (vertexBuffer.IsContentLost || !BufferReady)
            {
                BufferReady = true;
                vertexBuffer.SetData(0, particles, 0, firstFreeParticle * 4, FlareVertex.SizeInBytes, SetDataOptions.Discard);
            }

            Deferred3DEffect effect3D = ParticleHolder;
            effect3D.SetFromCamera(camera);

            Game1.graphicsDevice.BlendState = BlendState.Additive;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            effect3D.SetTextureSize(new Vector2(0.5f / Game1.graphicsDevice.Viewport.AspectRatio, -0.5f));
            effect3D.SetTime(Level.Time);
            Game1.graphicsDevice.SetVertexBuffer(vertexBuffer);
            Game1.graphicsDevice.Indices = indexBuffer;

            // Activate the particle effect.
            foreach (EffectPass pass in FlareEffect.CurrentTechnique.Passes)
            {
                pass.Apply();


                Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                             0, firstFreeParticle * 4,
                                             0, firstFreeParticle * 2);
            }
        }
    }
}
