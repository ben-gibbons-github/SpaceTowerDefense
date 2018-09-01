using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class BulletInstancer
    {
        public static readonly VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public const float ModelScale = 400;
        public static BulletInstancer self;

        int BulletCount = 0;
        LinkedList<Bullet> Bullets = new LinkedList<Bullet>();
        bool BufferReady = false;

        Model BulletModel;
        Effect ShipEffect;

        EffectParameter WorldParam;
        EffectParameter ViewParam;
        EffectParameter ProjectionParam;
        EffectTechnique ForwardInstancedTechnique;
        Matrix ModelTransformMatrix = Matrix.Identity;

        Matrix[] ModelTransforms;
        DynamicVertexBuffer vertexBuffer;

        public BulletInstancer()
        {
            self = this;
            BulletModel = AssetManager.Load<Model>("Models/ShipGame/World/Spark");
            ShipEffect = AssetManager.Load<Effect>("Effects/WhiteEffect");

            WorldParam = ShipEffect.Parameters["World"];
            ViewParam = ShipEffect.Parameters["View"];
            ProjectionParam = ShipEffect.Parameters["Projection"];
            ForwardInstancedTechnique = ShipEffect.Techniques["ForwardInstancedTechnique"];
        }

        public static void AddChild(Bullet b)
        {
            self.BulletCount++;
            self.Bullets.AddLast(b);
        }

        public static void RemoveChild(Bullet b)
        {
            self.BulletCount--;
            self.Bullets.Remove(b);
        }

        public void Update(GameTime gameTime)
        {
            BufferReady = false;
        }

        public void DrawInstanced(Camera3D DrawCamera)
        {
            if (BulletCount == 0)
                return;

            ViewParam.SetValue(DrawCamera.ViewMatrix);
            ProjectionParam.SetValue(DrawCamera.ProjectionMatrix);
            WorldParam.SetValue(Matrix.Identity);
            ShipEffect.CurrentTechnique = ForwardInstancedTechnique;

            if (!BufferReady)
            {
                BufferReady = true;
                Array.Resize(ref ModelTransforms, Bullets.Count);
                int i = 0;
                foreach (Bullet s in Bullets)
                    ModelTransforms[i++] = s.WorldMatrix;

                if ((vertexBuffer == null) ||
                    (Bullets.Count > vertexBuffer.VertexCount))
                {
                    if (vertexBuffer != null)
                        vertexBuffer.Dispose();

                    vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, instanceVertexDeclaration,
                                                                   ModelTransforms.Length, BufferUsage.WriteOnly);
                }

                vertexBuffer.SetData(ModelTransforms, 0, ModelTransforms.Length, SetDataOptions.Discard);
            }
            else if (vertexBuffer.IsContentLost)
                vertexBuffer.SetData(ModelTransforms, 0, ModelTransforms.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in BulletModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    Game1.graphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(vertexBuffer, 0, 1)
                    );

                    Game1.graphicsDevice.Indices = meshPart.IndexBuffer;

                    foreach (EffectPass pass in ShipEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        Game1.graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, ModelTransforms.Length);
                    }
                }
            }
        }
    }
}
