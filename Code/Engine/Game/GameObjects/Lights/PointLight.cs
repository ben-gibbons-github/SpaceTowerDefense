using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BadRabbit.Carrot.RenderTargetAssets;

namespace BadRabbit.Carrot
{
    public class PointLight : BasicLight
    {
        public static Model SphereModel;
        public static bool Loaded = false;
        public static Matrix ShadowProjection;

        public Camera3D[] ShadowCameras;
        public BoolValue UseShadows;
        public BoolValue RealtimeShadows;
        public FloatValue ShadowMapSize;
        public RenderCubeAsset ShadowCube;

        public override void Create()
        {
            AddTag(GameObjectTag._3DDeferredWorldLighting);

            UseShadows = new BoolValue("Use Shadows");
            UseShadows.ChangeEvent = ShadowChange;
            
            RealtimeShadows = new BoolValue("Real Time Shadows");
            RealtimeShadows.ChangeEvent = RealtimeChange;

            ShadowMapSize = new FloatValue("Shadow Map CellSize", 256);
            ShadowMapSize.ChangeEvent = ShadowChange;

            MyEffect = new EffectValue("CommandPoint Effect", "Deferred/PointLightNoShadows", EffectHolderType.DeferredLight);

            Load();
            base.Create();

            CreateCameras();

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                AddTag(GameObjectTag._3DForward);
                ApplyScale(Vector3.One * 200, Vector3.Zero, false);
            }
#endif
        }

        private void CreateCameras()
        {
            if (ShadowCameras == null)
            {
                ShadowCameras = new Camera3D[6];
                for (int i = 0; i < 6; i++)
                    ShadowCameras[i] = new Camera3D(90 * 3.14159265f / 180, 0.1f, 50000, new Vector2(512));
            }
            ShadowCameras[0].SetLookAt(Position.get(), Position.get() + new Vector3(1000, 0, 0), Vector3.Up);
            ShadowCameras[1].SetLookAt(Position.get(), Position.get() + new Vector3(-1000, 0, 0), Vector3.Up);
            ShadowCameras[2].SetLookAt(Position.get(), Position.get() + new Vector3(0, 1000, 0), Vector3.Forward);
            ShadowCameras[3].SetLookAt(Position.get(), Position.get() + new Vector3(0, -1000, 0), Vector3.Backward);
            ShadowCameras[4].SetLookAt(Position.get(), Position.get() + new Vector3(0, 0, 1000), Vector3.Up);
            ShadowCameras[5].SetLookAt(Position.get(), Position.get() + new Vector3(0, 0, -1000), Vector3.Up);
        }

        public override void ChangePosition()
        {
            CreateCameras();
            base.ChangePosition();
        }

        private void RealtimeChange()
        {
            if (RealtimeShadows.get() && UseShadows.get())
                AddTag(GameObjectTag._3DPreDraw);
            else
                RemoveTag(GameObjectTag._3DPreDraw);
        }

        private void ShadowChange()
        {
            if (UseShadows.get())
            {
                if (MyEffect.get() == null || MyEffect.getPath().Equals("Deferred/PointLightNoShadows"))
                    MyEffect.set("Deferred/PointLightShadows");

                ShadowCube = AssetManager.RequestCubeMap((int)ShadowMapSize.get(), SurfaceFormat.HalfSingle, DepthFormat.Depth24Stencil8, ShadowCube);
                if (MyEffect.Holder != null)
                {
                    DeferredLightEffect effect3D = (DeferredLightEffect)(MyEffect.Holder);
                    if (ShadowCube.Value != null)
                        effect3D.ShadowReference.SetValue(ShadowCube.Value);
                }

                DrawShadows();
            }
            else
            {
                if (MyEffect.get() == null || MyEffect.getPath().Equals("Effects/Deferred/PointLightShadows"))
                    MyEffect.set("Deferred/PointLightNoShadows");
                AssetManager.FreeRenderTarget(ShadowCube);
            }
        }

        new private static void Load()
        {
            if (!Loaded)
            {
                ShapeRenderer.Load();
                Loaded = true;
                SphereModel = AssetManager.Load<Model>("Models/Deferred/Sphere");
                ShadowProjection = Matrix.CreatePerspectiveFieldOfView(90 * 3.14159265f / 180, 1, 0.1f, 2000) * Matrix.CreateScale(-1, 1, 1);
            }
        }

        public void DrawShadows()
        {
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;

            Transfer.LightPosition = Position.get();
            Transfer.LightDistance = Scale.get()/2;

            for (int i = 0; i < 6; i++)
            {
                CubeMapFace CurrentFace = (CubeMapFace)(i);
                Game1.graphicsDevice.SetRenderTarget(ShadowCube.Value, CurrentFace);
                Game1.graphicsDevice.Clear(Color.White);

                foreach(GameObject g in ParentScene.GetList(GameObjectTag._3DShadow))
                    g.Draw3D(ShadowCameras[i],GameObjectTag._3DShadow);
            }

            Game1.graphicsDevice.SetRenderTarget(null);
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (lightState == BasicLight.LightState.Dead)
            {
                base.Draw3D(camera, DrawTag);
                return;
            }

            if (DrawTag == GameObjectTag._3DPreDraw)
            {
                if (RealtimeShadows.get())
                    DrawShadows();
            }
            else
            {
#if EDITOR && WINDOWS
                if (DrawTag == GameObjectTag._3DForward)
                {
                    if (EditorSelected)
                    {
                        Vector4 Col = Vector4.One;
                        ShapeRenderer.DrawSphere(WorldMatrix, camera, Col);
                    }
                    if (ShadowCube != null)
                        ShapeRenderer.DrawSphere(WorldMatrix, camera, ShadowCube.Value);
                }
                else
#endif
                    if (MyEffect.Holder != null)
                    {
                        Game1.graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                        DeferredLightEffect effect3D = (DeferredLightEffect)MyEffect.Holder;

                        effect3D.SetTextureSize(ParentScene.WindowSize);
                        effect3D.SetUV(camera);
                        effect3D.SetInverseCamera(camera);
                        effect3D.SetLight(Position.get(), Scale.get() / 2);
                        effect3D.SetFromObject(this);
                        effect3D.SetFromCamera(camera);
                        effect3D.MyEffect.CurrentTechnique.Passes[0].Apply();
                        Render.DrawModel(SphereModel, MyEffect.get());
                    }
            }
            base.Draw3D(camera, DrawTag);
        }
    }
}
