using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ShieldInstancer
    {
        public static Color RedShield = new Color(1, 0.25f, 0.25f);
        public static Color GreenShield = new Color(0.25f, 1, 0.25f);
        public static Color BlueShield = new Color(0.25f, 0.25f, 1);
        public static Color WhiteShield = new Color(0.25f, 0.25f, 0.25f);

        static ShieldInstancer self;

        Model ShieldModel;
        Effect ShieldEffect;
        bool Applied = false;
        bool BufferReady = false;


        EffectParameter WorldParam;
        EffectParameter ViewParam;
        EffectParameter ProjectionParam;
        EffectParameter ViewPosParam;

        Texture2D ShieldTexture;
        Matrix ModelTransformMatrix = Matrix.Identity;

        DynamicVertexBuffer vertexBuffer;

        ShieldVertex[] ShieldVertecies;
        LinkedList<UnitBasic> UnitChildren = new LinkedList<UnitBasic>();

        public ShieldInstancer()
        {
            self = this;
            Load();
        }

        private void Load()
        {
            ShieldModel = AssetManager.Load<Model>("Models/ShipGame/ShieldModel");
            ShieldEffect = AssetManager.Load<Effect>("Effects/ShipGame/Shield");
            ShieldTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/ShieldTexture");

            ViewParam = ShieldEffect.Parameters["View"];
            ProjectionParam = ShieldEffect.Parameters["Projection"];
            ViewPosParam = ShieldEffect.Parameters["CameraPosition"];
            WorldParam = ShieldEffect.Parameters["World"];
        }

        public static void Add(UnitBasic u)
        {
            self.UnitChildren.AddLast(u);
        }

        public static void Remove(UnitBasic u)
        {
            self.UnitChildren.Remove(u);
        }

        public void Update(GameTime gameTime)
        {
            BufferReady = false;

            foreach (UnitBasic b in UnitChildren)
                ParticleManager.CreateParticle(new Vector3(b.Position.X(), b.Y, b.Position.Y()), Vector3.Zero, b.GetShieldColor() * 0.5f, b.Size.X() * 12, 1);
        }

        private void ApplyEffectParameters()
        {
            Applied = true;
            ShieldEffect.Parameters["Texture"].SetValue(ShieldTexture);
            ShieldEffect.Parameters["World"].SetValue(ModelTransformMatrix);

            WorldParam.SetValue(Matrix.CreateScale(0.01f));
        }

        public void DrawInstanced(Camera3D DrawCamera)
        {
            if (UnitChildren.Count == 0)
                return;

            Game1.graphicsDevice.BlendState = BlendState.Additive;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

            if (!Applied)
                ApplyEffectParameters();

            ViewParam.SetValue(DrawCamera.ViewMatrix);
            ProjectionParam.SetValue(DrawCamera.ProjectionMatrix);
            ViewPosParam.SetValue(DrawCamera.Position);
            
            if (!BufferReady)
            {
                BufferReady = true;
                Array.Resize(ref ShieldVertecies, UnitChildren.Count);
                int i = 0;
                foreach (UnitBasic s in UnitChildren)
                {
                    ShieldVertecies[i].WorldMatrix = s.WorldMatrix;
                    ShieldVertecies[i].color = s.GetShieldColor() * 0.5f;

                    i++;
                }

                if ((vertexBuffer == null) ||
                    (UnitChildren.Count > vertexBuffer.VertexCount))
                {
                    if (vertexBuffer != null)
                        vertexBuffer.Dispose();

                    vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, ShieldVertex.shieldVertexDeclaration,
                                                                   ShieldVertecies.Length, BufferUsage.WriteOnly);
                }

                vertexBuffer.SetData(ShieldVertecies, 0, ShieldVertecies.Length, SetDataOptions.Discard);
            }
            else if (vertexBuffer.IsContentLost)
                vertexBuffer.SetData(ShieldVertecies, 0, ShieldVertecies.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in ShieldModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    Game1.graphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(vertexBuffer, 0, 1)
                    );

                    Game1.graphicsDevice.Indices = meshPart.IndexBuffer;

                    foreach (EffectPass pass in ShieldEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        Game1.graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, ShieldVertecies.Length);
                    }
                }
            }


            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }
}
