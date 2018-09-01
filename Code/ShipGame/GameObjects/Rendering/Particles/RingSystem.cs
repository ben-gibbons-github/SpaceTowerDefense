using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace BadRabbit.Carrot
{
    public class RingSystem : BasicParticleSystem
    {
        private static Vector3[] Offset = { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(1, 0, 1), new Vector3(-1, 0, 1) };
        bool BufferReady = false;

        public Effect RingEffect;
        Deferred3DEffect ParticleHolder;

        EffectParameter TeamParam;
        EffectParameter FactionParam;

        RingVertex[] particles;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        int firstFreeParticle = 0;

        int MaxParticles;
        Random random = new Random();

        public RingSystem(int MaxParticles, string TexturePath)
        {
            RingEffect = AssetManager.LoadEffect("Effects/ShipGame/ShipRings");
            TeamParam = RingEffect.Parameters["Team"];
            FactionParam = RingEffect.Parameters["Faction"];

            RingEffect.Parameters["Colors"].SetValue(TeamInfo.Colors3);

            ParticleHolder = (Deferred3DEffect)new Deferred3DEffect().Create(RingEffect);
            this.MaxParticles = MaxParticles;
            CreateArray();
        }

        private void CreateArray()
        {
            particles = new RingVertex[MaxParticles * 4];

            for (int i = 0; i < MaxParticles; i++)
            {
                particles[i * 4 + 0].Corner = new Short2(-1, -1);
                particles[i * 4 + 1].Corner = new Short2(1, -1);
                particles[i * 4 + 2].Corner = new Short2(1, 1);
                particles[i * 4 + 3].Corner = new Short2(-1, 1);
            }

            vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, RingVertex.VertexDeclaration,
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
            firstFreeParticle = 0;
            BufferReady = false;
            base.Update(gameTime);
        }

        public void AddParticle(Vector3 position, float Size, float Team)
        {
            if (firstFreeParticle == MaxParticles)
                return;

            for (int i = 0; i < 4; i++)
            {
                particles[firstFreeParticle * 4 + i].Position = position + Offset[i] * Size / 2;
                particles[firstFreeParticle * 4 + i].Team = Team;
            }

            firstFreeParticle++;
        }

        public override void Draw(Camera3D camera)
        {
            if (firstFreeParticle == 0)
                return;

            if (vertexBuffer.IsContentLost || !BufferReady)
            {
                BufferReady = true;
                vertexBuffer.SetData(0, particles, 0, firstFreeParticle * 4, RingVertex.SizeInBytes, SetDataOptions.Discard);
            }

            Deferred3DEffect effect3D = ParticleHolder;
            effect3D.SetFromCamera(camera);

            Game1.graphicsDevice.BlendState = BlendState.Additive;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

            Game1.graphicsDevice.SetVertexBuffer(vertexBuffer);
            Game1.graphicsDevice.Indices = indexBuffer;

            // Activate the particle effect.
            foreach (EffectPass pass in RingEffect.CurrentTechnique.Passes)
            {
                pass.Apply();


                Game1.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                             0, firstFreeParticle * 4,
                                             0, firstFreeParticle * 2);
            }
        }
    }
}
