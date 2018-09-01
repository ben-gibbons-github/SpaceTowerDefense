using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using BadRabbit.Carrot.EffectParameters;

namespace BadRabbit.Carrot
{
    public class DrawShip : DrawItem
    {
        Vector4 DrawColor = new Vector4(1, 1, 1, 1);
        Vector4 SpecularColor = new Vector4(1, 1, 1, 1);
        float SpecularExponent = 0.3f;
        public float ShipScale = 1;
        float ModelRotation = 1; // Radians

        Model ShipModel;
        Effect ShipEffect;
        bool Applied = false;
        bool BufferReady = false;
        bool DistortionBufferReady = false;

        EffectParameter WorldParam;
        EffectParameter ViewParam;
        EffectParameter ProjectionParam;
        EffectParameter ViewPosParam;
        EffectParameter ColorParameter;
        EffectTechnique ForwardInstancedTechnique;
        EffectTechnique ForwardTechnique;
        Texture2D ShipTexture;
        Matrix ModelTransformMatrix = Matrix.Identity;
        BlendState blendState = BlendState.AlphaBlend;
        DepthStencilState depthStencilState = DepthStencilState.Default;

        ShipVertex[] ShipVertecies;
        DynamicVertexBuffer vertexBuffer;

        ShipVertex[] DistortionShipVertecies;
        DynamicVertexBuffer DistortionvertexBuffer;

        DrawShipWeaponPoint[] WeaponPoints;
        Dictionary<int, LinkedList<DrawShipParticlePoint>> ParticlePoints = new Dictionary<int, LinkedList<DrawShipParticlePoint>>();
        float ColorMult = 1;

        public DrawShip(string Fname)
        {
            LoadShip(Fname);
        }

        public DrawShip(string Fname, BlendState blendState, DepthStencilState depthStencilState, float ShipScale, float ColorMult)
        {
            this.ShipScale = ShipScale;
            this.blendState = blendState;
            this.depthStencilState = depthStencilState;
            this.ColorMult = ColorMult;
            LoadShip(Fname);
        }

        public override void EmitParticle(int Layer, ref Vector3 Position, ref Matrix RotationMatrix, float Scale, float ColorMult)
        {
            if (ParticlePoints.Keys.Count > 0)
                foreach (DrawShipParticlePoint point in ParticlePoints[Layer])
                {
                    if (point.CanProduce)
                        point.ProduceParticle(ref Position, ref RotationMatrix, Scale, ColorMult);
                }
        }

        public override Vector3 GetWeaponPosition(Vector3 Position, ref Matrix RotationMatrix, int ID, float Scale)
        {
            if (WeaponPoints.Length > 0)
            {
                return Position + Vector3.Transform(WeaponPoints[ID % WeaponPoints.Length].Position * Scale / 100, RotationMatrix);
            }
            else
                return Position;
        }

        public override void Update(GameTime gameTime)
        {
            BufferReady = false;
            DistortionBufferReady = false;
            foreach (LinkedList<DrawShipParticlePoint> list in ParticlePoints.Values)
                foreach (DrawShipParticlePoint point in list)
                    point.AddTime(gameTime, false);
        }

        private void LoadShip(string Fname)
        {
            BinaryReader Reader = new BinaryReader(TitleContainer.OpenStream("Content/Extras/ShipGame/" + Fname + ".shp"));
            SaveHelper.MyReader = Reader;

            Reader.ReadByte();
            string Test = Reader.ReadString();

            ShipScale *= Reader.ReadSingle() / 1.25f;
            ModelRotation = MathHelper.ToRadians(Reader.ReadSingle());

            ReadShipViewer(Reader);
            ReadParticlePoints(Reader);
            ReadWeaponPoints(Reader);
        }

        private void ReadShipViewer(BinaryReader Reader)
        {
            string ShipString = Reader.ReadString();
            ShipModel = AssetManager.Load<Model>(ShipString);
            ModelTransformMatrix = Matrix.CreateScale(1 / ShipScale) * Matrix.CreateRotationY(ModelRotation);

            ShipEffect = AssetManager.LoadEffect(Reader.ReadString());
            WorldParam = ShipEffect.Parameters["World"];
            ViewParam = ShipEffect.Parameters["View"];
            ProjectionParam = ShipEffect.Parameters["Projection"];
            ViewPosParam = ShipEffect.Parameters["CameraPosition"];
            ForwardInstancedTechnique = ShipEffect.Techniques["ForwardInstancedTechnique"];
            ForwardTechnique = ShipEffect.Techniques["ForwardTechnique"];
            ColorParameter = ShipEffect.Parameters["DrawColor"];

            ShipTexture = AssetManager.Load<Texture2D>(Reader.ReadString());

            DrawColor = SaveHelper.ReadVector4();
            SpecularColor = SaveHelper.ReadVector4();
            SpecularExponent = Reader.ReadSingle();
        }

        private void ReadParticlePoints(BinaryReader Reader)
        {
            int ParticleCount = Reader.ReadInt32();

            for (int i = 0; i < ParticleCount; i++)
            {
                DrawShipParticlePoint s = new DrawShipParticlePoint();

                s.Position = Vector3.Transform(SaveHelper.ReadVector3() * 100, ModelTransformMatrix);
                s.Layer = Reader.ReadInt32();
                s.CinematicDelay = Reader.ReadInt32();
                s.GameDelay = Reader.ReadInt32();
                s.ParticleType = Reader.ReadInt32();
                s.MaxVelocity = Vector3.Transform(SaveHelper.ReadVector3() * 100, ModelTransformMatrix);
                s.MinVelocity = Vector3.Transform(SaveHelper.ReadVector3() * 100, ModelTransformMatrix);
                s.MinColor = SaveHelper.ReadVector4();
                s.MaxColor = SaveHelper.ReadVector4();
                s.MinSize = Reader.ReadSingle() / ShipScale * 100;
                s.MaxSize = Reader.ReadSingle() / ShipScale * 100;
                s.CinematicOnly = Reader.ReadBoolean();

                if (!ParticlePoints.ContainsKey(s.Layer))
                    ParticlePoints.Add(s.Layer, new LinkedList<DrawShipParticlePoint>());
                
                ParticlePoints[s.Layer].AddLast(s);
            }
        }

        private void ReadWeaponPoints(BinaryReader Reader)
        {
            int WeaponCount = Reader.ReadInt32();

            WeaponPoints = new DrawShipWeaponPoint[WeaponCount];

            for (int i = 0; i < WeaponCount; i++)
            {
                DrawShipWeaponPoint s = new DrawShipWeaponPoint();
                WeaponPoints[i] = s;

                s.Position = Vector3.Transform(SaveHelper.ReadVector3() * 100, ModelTransformMatrix);
                s.Layer = Reader.ReadInt32();
            }
        }

        private void ApplyEffectParameters()
        {
            Applied = true;
            ShipEffect.Parameters["Texture"].SetValue(ShipTexture);
            ShipEffect.Parameters["DrawColor"].SetValue(DrawColor * new Vector4(4 * ColorMult, 4 * ColorMult, 4 * ColorMult, 1));
            ShipEffect.Parameters["SpecularColor"].SetValue(SpecularColor);
            ShipEffect.Parameters["SpecularExponent"].SetValue(SpecularExponent);

            ShipEffect.Parameters["AmbientLightColor"].SetValue(ShipLighting.WorldAmbientLightColor);
            ShipEffect.Parameters["LightOneDirection"].SetValue(ShipLighting.WorldLightOneDirection);
            ShipEffect.Parameters["LightOneColor"].SetValue(ShipLighting.WorldLightOneColor);
            ShipEffect.Parameters["LightTwoDirection"].SetValue(ShipLighting.WorldLightTwoDirection);
            ShipEffect.Parameters["LightTwoColor"].SetValue(ShipLighting.WorldLightTwoColor);

            WorldParam.SetValue(ModelTransformMatrix);
            ShipEffect.CurrentTechnique = ForwardInstancedTechnique;
        }

        public override void DrawSingle(Vector3 Position, float Size, Vector4 Color, Camera3D DrawCamera)
        {
            WorldParam.SetValue(ModelTransformMatrix * Matrix.CreateScale(Size) * Matrix.CreateTranslation(Position));
            ViewParam.SetValue(DrawCamera.ViewMatrix);
            ProjectionParam.SetValue(DrawCamera.ProjectionMatrix);
            ViewPosParam.SetValue(DrawCamera.Position);
            ColorParameter.SetValue(Color);
            ShipEffect.CurrentTechnique = ForwardTechnique;

            Render.DrawModel(ShipModel, ShipEffect);

            ColorParameter.SetValue(DrawColor * ColorMult);
            WorldParam.SetValue(ModelTransformMatrix);
        }
        
        public override void DrawInstanced(LinkedList<BasicShipGameObject> ships, Camera3D DrawCamera)
        {
            if (ships.Count == 0)
                return;

            Game1.graphicsDevice.BlendState = blendState;
            Game1.graphicsDevice.DepthStencilState = depthStencilState;

            if (!Applied)
                ApplyEffectParameters();

            ViewParam.SetValue(DrawCamera.ViewMatrix);
            ProjectionParam.SetValue(DrawCamera.ProjectionMatrix);
            ViewPosParam.SetValue(DrawCamera.Position);

            if (!BufferReady)
            {
                BufferReady = true;
                Array.Resize(ref ShipVertecies, ships.Count);
                int i = 0;
                foreach (BasicShipGameObject s in ships)
                {
                    ShipVertecies[i].WorldMatrix = s.WorldMatrix;
                    ShipVertecies[i++].color = s.MyColor;
                }

                if ((vertexBuffer == null) ||
                    (ships.Count > vertexBuffer.VertexCount))
                {
                    if (vertexBuffer != null)
                        vertexBuffer.Dispose();

                    vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, ShipVertex.shipVertexDeclaration,
                                                                   ShipVertecies.Length, BufferUsage.WriteOnly);
                }

                vertexBuffer.SetData(ShipVertecies, 0, ShipVertecies.Length, SetDataOptions.Discard);
            }
            else if (vertexBuffer.IsContentLost)
                vertexBuffer.SetData(ShipVertecies, 0, ShipVertecies.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in ShipModel.Meshes)
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
                                                               meshPart.PrimitiveCount, ShipVertecies.Length);
                    }
                }
            }
        }

        public void DrawInstanced(LinkedList<WallItem> walls, Camera3D DrawCamera)
        {
            if (walls.Count == 0)
                return;

            Game1.graphicsDevice.BlendState = blendState;
            Game1.graphicsDevice.DepthStencilState = depthStencilState;

            if (!Applied)
                ApplyEffectParameters();

            ViewParam.SetValue(DrawCamera.ViewMatrix);
            ProjectionParam.SetValue(DrawCamera.ProjectionMatrix);
            ViewPosParam.SetValue(DrawCamera.Position);

            if (!BufferReady)
            {
                BufferReady = true;
                Array.Resize(ref ShipVertecies, walls.Count);
                int i = 0;
                foreach (WallItem s in walls)
                {
                    ShipVertecies[i].WorldMatrix = s.GetMatrix();
                    ShipVertecies[i++].color = new Color(0.25f, 0.25f, 0.25f, 1);
                }

                if ((vertexBuffer == null) ||
                    (walls.Count > vertexBuffer.VertexCount))
                {
                    if (vertexBuffer != null)
                        vertexBuffer.Dispose();

                    vertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, ShipVertex.shipVertexDeclaration,
                                                                   ShipVertecies.Length, BufferUsage.WriteOnly);
                }

                vertexBuffer.SetData(ShipVertecies, 0, ShipVertecies.Length, SetDataOptions.Discard);
            }
            else if (vertexBuffer.IsContentLost)
                vertexBuffer.SetData(ShipVertecies, 0, ShipVertecies.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in ShipModel.Meshes)
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
                                                               meshPart.PrimitiveCount, ShipVertecies.Length);
                    }
                }
            }
        }

        public override void DrawDistortion(LinkedList<BasicShipGameObject> ships, Camera3D DrawCamera)
        {
            if (ships.Count == 0)
                return;

            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

            if (!Applied)
                ApplyEffectParameters();

            DistortionEffectContainer.ViewParam.SetValue(DrawCamera.ViewMatrix);
            DistortionEffectContainer.ProjectionParam.SetValue(DrawCamera.ProjectionMatrix);
            DistortionEffectContainer.WorldParam.SetValue(ModelTransformMatrix);

            if (!DistortionBufferReady)
            {
                DistortionBufferReady = true;
                Array.Resize(ref DistortionShipVertecies, ships.Count);
                int i = 0;
                foreach (BasicShipGameObject s in ships)
                {
                    DistortionShipVertecies[i].WorldMatrix = s.WorldMatrix;
                    DistortionShipVertecies[i++].color = s.MyColor;
                }

                if ((DistortionvertexBuffer == null) ||
                    (ships.Count > DistortionvertexBuffer.VertexCount))
                {
                    if (DistortionvertexBuffer != null)
                        DistortionvertexBuffer.Dispose();

                    DistortionvertexBuffer = new DynamicVertexBuffer(Game1.graphicsDevice, ShipVertex.shipVertexDeclaration,
                                                                   DistortionShipVertecies.Length, BufferUsage.WriteOnly);
                }

                DistortionvertexBuffer.SetData(DistortionShipVertecies, 0, DistortionShipVertecies.Length, SetDataOptions.Discard);
            }
            else if (DistortionvertexBuffer.IsContentLost)
                DistortionvertexBuffer.SetData(DistortionShipVertecies, 0, DistortionShipVertecies.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in ShipModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    Game1.graphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(DistortionvertexBuffer, 0, 1)
                    );

                    Game1.graphicsDevice.Indices = meshPart.IndexBuffer;

                    foreach (EffectPass pass in DistortionEffectContainer.DistortionEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        Game1.graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, DistortionShipVertecies.Length);
                    }
                }
            }
        }
    }
}
