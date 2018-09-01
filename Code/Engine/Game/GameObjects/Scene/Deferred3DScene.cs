using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class Deferred3DScene : Basic3DScene
    {
        private DeferredControls deferredControls;

        private static RenderTargetBinding[] GBufferTargets;
        private static RenderTarget2D LightMap;
        private static RenderTarget2D FinalTarget;
        public static BlendState LightMapBS;
        public static Effect ClearEffect;
        public static Effect PasteEffect;
        public static Effect SpecularEffect;
        private static bool Loaded = false;

        private LinkedList<GameObject> GBufferChildren;
        private LinkedList<GameObject> WorldLightingChildren;
        private LinkedList<GameObject> OverLightingChildren;
        private LinkedList<GameObject> ForwardChildren;
        private LinkedList<GameObject> DepthOverChildren;
        private LinkedList<GameObject> UpdateChildren;
        private LinkedList<GameObject> SolidChildren;
        private LinkedList<GameObject> ShadowChildren;
        private LinkedList<GameObject> PreDrawChildren;
        private LinkedList<GameObject> BackgroundChildren;

#if EDITOR
        private StopwatchWrapper PreDrawTime;
        private StopwatchWrapper GBufferTime;
        private StopwatchWrapper LightmapTime;
        private StopwatchWrapper CompositeTime;
#endif

        public override void Create()
        {
#if EDITOR
            PreDrawTime = new StopwatchWrapper("PredrawTime");
            GBufferTime = new StopwatchWrapper("GBufferTime");
            LightmapTime = new StopwatchWrapper("LightMapTime");
            CompositeTime = new StopwatchWrapper("CompositeTime");
#endif

            AddWindowForm(deferredControls = new DeferredControls());
            GBufferChildren = AddTag(GameObjectTag._3DDeferredGBuffer);
            OverLightingChildren = AddTag(GameObjectTag._3DDeferredOverLighting);
            WorldLightingChildren = AddTag(GameObjectTag._3DDeferredWorldLighting);
            ForwardChildren = AddTag(GameObjectTag._3DForward);
            DepthOverChildren = AddTag(GameObjectTag._3DDepthOver);
            UpdateChildren = AddTag(GameObjectTag.Update);
            SolidChildren = AddTag(GameObjectTag._3DSolid);
            ShadowChildren = AddTag(GameObjectTag._3DShadow);
            PreDrawChildren = AddTag(GameObjectTag._3DPreDraw);
            BackgroundChildren = AddTag(GameObjectTag._3DBackground);

            Load();

            base.Create();
        }

        new public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;

                ClearEffect = AssetManager.Load<Effect>("Effects/Deferred/Clear");
                PasteEffect = AssetManager.Load<Effect>("Effects/Deferred/Paste");

                SpecularEffect = AssetManager.Load<Effect>("Effects/Deferred/Specular");

                LightMapBS = new BlendState();
                LightMapBS.ColorSourceBlend = Blend.One;
                LightMapBS.ColorDestinationBlend = Blend.One;
                LightMapBS.ColorBlendFunction = BlendFunction.Add;
                LightMapBS.AlphaSourceBlend = Blend.One;
                LightMapBS.AlphaDestinationBlend = Blend.One;
                LightMapBS.AlphaBlendFunction = BlendFunction.Add;
            }
        }

        public override void SetWindowSize(Vector2 WindowSize)
        {
            try
            {
                if (GBufferTargets != null)
                {
                    GBufferTargets[0].RenderTarget.Dispose();
                    GBufferTargets[1].RenderTarget.Dispose();
                    LightMap.Dispose();
                    FinalTarget.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (GBufferTargets == null || GBufferTargets[0].RenderTarget.IsDisposed)
                GBufferTargets = new RenderTargetBinding[2];

            GBufferTargets[0] = new RenderTargetBinding(new RenderTarget2D(Game1.graphicsDevice, (int)WindowSize.X, (int)WindowSize.Y, false, SurfaceFormat.Color, DepthFormat.Depth24));
            GBufferTargets[1] = new RenderTargetBinding(new RenderTarget2D(Game1.graphicsDevice, (int)WindowSize.X, (int)WindowSize.Y, false, SurfaceFormat.Single, DepthFormat.Depth24));
            LightMap = new RenderTarget2D(Game1.graphicsDevice, (int)WindowSize.X, (int)WindowSize.Y, false, SurfaceFormat.Rgba64, DepthFormat.Depth24);
            FinalTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, (int)WindowSize.X, (int)WindowSize.Y, false, SurfaceFormat.Color, DepthFormat.Depth24);

            base.SetWindowSize(WindowSize);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameObject g in UpdateChildren)
                g.Update(gameTime);

#if EDITOR && WINDOWS
            if (KeyboardManager.KeyJustPressed(Keys.F2))
                deferredControls.Cycle();
#endif

            base.Update(gameTime);
            UpdateEditor(gameTime);
        }

        public override void PreDraw()
        {
            //PreDraw
#if EDITOR
            PreDrawTime.Start();
#endif
            foreach (GameObject g in PreDrawChildren)
                g.Draw3D(DrawCamera, GameObjectTag._3DPreDraw);
#if EDITOR
            PreDrawTime.Stop();

            //GBuffer
            GBufferTime.Start();
#endif
            DClearGBuffer();
            DMakeGBuffer();
#if EDITOR
            GBufferTime.Stop();
            //LightMap
            LightmapTime.Start();
#endif
            DMakeLightMap();
#if EDITOR
            LightmapTime.Stop();
            CompositeTime.Start();
#endif
            DMakeFinalPass();
#if EDITOR
            CompositeTime.Stop();
#endif
        }

        private void DClearGBuffer()
        {
#if WINDOWS
            if (GBufferTargets[0].RenderTarget.IsDisposed || GBufferTargets[1].RenderTarget.IsDisposed)
                SetWindowSize(WindowSize);
#endif
#if XBOX
            if (GBufferTargets == null)
                SetWindowSize(new Vector2(Game1.ResolutionX, Game1.ResolutionY));
#endif
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
            Game1.graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
            Game1.graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Game1.graphicsDevice.SetRenderTargets(GBufferTargets);
            ClearEffect.CurrentTechnique.Passes[0].Apply();
            FullscreenQuad.Draw();
        }

        private void DMakeGBuffer()
        {
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
                foreach (GameObject g in GBufferChildren)
                    g.Draw3D(DrawCamera, GameObjectTag._3DDeferredGBuffer);
            else
#endif
            {
                int i = 0;
                Camera3D currentCamera = null;

                foreach (WorldViewer3D s in WorldViewerChildren)
                {
                    Render.ViewIndex = i++;
                    s.getSceneView().Set();
                    currentCamera = s.getCamera();
                    foreach (GameObject g in GBufferChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DDeferredGBuffer);
                }
            }
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
#if EDITOR && WINDOWS
            if (!ParentLevel.LevelForEditing)
            {
#endif
                MasterManager.SetViewportToFullscreen();
                Game1.graphicsDevice.Clear(Color.Black);

#if EDITOR && WINDOWS
            }

            if (deferredControls.deferredMode == DeferredMode.Depth || deferredControls.deferredMode == DeferredMode.Normal || deferredControls.deferredMode == DeferredMode.Lighting)
            {
                Game1.graphicsDevice.BlendState = BlendState.Opaque;
                Game1.graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
                Game1.graphicsDevice.Textures[0] = deferredControls.deferredMode == DeferredMode.Lighting ? LightMap : GBufferTargets[deferredControls.deferredMode == DeferredMode.Normal ? 0 : 1].RenderTarget;
                PasteEffect.CurrentTechnique.Passes[0].Apply();
                FullscreenQuad.Draw();
                objectControls.Draw3D(DrawCamera);
            }
            else
#endif
                if (DrawCamera != null)
                {
                    Game1.graphicsDevice.BlendState = BlendState.Opaque;
                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

                    Game1.graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                    Game1.graphicsDevice.Textures[0] = FinalTarget;
                    PasteEffect.CurrentTechnique.Passes[0].Apply();
                    FullscreenQuad.Draw();
#if EDITOR && WINDOWS
                    if (ParentLevel.LevelForEditing)
                        objectControls.Draw3D(DrawCamera);
#endif
                }
            base.Draw2D(DrawTag);
        }

        private void DMakeFinalPass()
        {
            Game1.graphicsDevice.SetRenderTarget(FinalTarget);
            Game1.graphicsDevice.Clear(Color.Black);

            Game1.graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

            
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing && DrawCamera != null)
            {
                DrawCamera.SetOffset(Vector2.Zero);
                DrawCamera.SetMult(Vector2.One);

                Game1.graphicsDevice.BlendState = BlendState.Opaque;
                Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

                foreach (GameObject g in BackgroundChildren)
                    g.Draw3D(DrawCamera, GameObjectTag._3DBackground);

                Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;

                if (deferredControls.deferredMode == DeferredMode.Basic)
                    Game1.graphicsDevice.Textures[0] = Render.TransparentTexture;
                else
                    Game1.graphicsDevice.Textures[0] = LightMap;

                foreach (GameObject g in ForwardChildren)
                    g.Draw3D(DrawCamera, GameObjectTag._3DForward);

                Game1.graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

                foreach (GameObject g in DepthOverChildren)
                    g.Draw3D(DrawCamera, GameObjectTag._3DForward);
            }
            else
#endif
            {
                int i = 0;
                Camera3D currentCamera = null;

                foreach (WorldViewer3D s in WorldViewerChildren)
                {
                    Game1.graphicsDevice.BlendState = BlendState.Opaque;
                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

                    Render.ViewIndex = i++;
                    s.getSceneView().Set();
                    currentCamera = s.getCamera();
                    foreach (GameObject g in BackgroundChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DBackground);

                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;

#if EDITOR && WINDOWS
                    if (deferredControls.deferredMode == DeferredMode.Basic)
                        Game1.graphicsDevice.Textures[0] = Render.TransparentTexture;
                    else
#endif
                        Game1.graphicsDevice.Textures[0] = LightMap;

                    foreach (GameObject g in ForwardChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DForward);

                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                    Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
                    foreach (GameObject g in DepthOverChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DForward);
                }
            }
        }

        private void DMakeLightMap()
        {
            Game1.graphicsDevice.SetRenderTargets(null);
            Game1.graphicsDevice.SetRenderTarget(LightMap);
            Game1.graphicsDevice.Clear(Color.Transparent);
            Game1.graphicsDevice.BlendState = LightMapBS;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;


            Game1.graphicsDevice.Textures[0] = GBufferTargets[0].RenderTarget;
            Game1.graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

            Game1.graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            Game1.graphicsDevice.Textures[1] = GBufferTargets[1].RenderTarget;

            MasterManager.SetViewportToFullscreen();

            foreach (GameObject g in OverLightingChildren)
                g.Draw3D(DrawCamera, GameObjectTag._3DDeferredOverLighting);

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing && DrawCamera != null)
            {
                DrawCamera.MakeInverse();
                foreach (GameObject g in WorldLightingChildren)
                    g.Draw3D(DrawCamera, GameObjectTag._3DDeferredOverLighting);
            }
            else
#endif
            {
                int i = 0;
                Camera3D currentCamera = null;

                foreach (WorldViewer3D s in WorldViewerChildren)
                {
                    Render.ViewIndex = i++;
                    s.getSceneView().Set();
                    currentCamera = s.getCamera();

                    currentCamera.MakeInverse();
                    foreach (GameObject g in WorldLightingChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DDeferredOverLighting);
                }
            }
        }
    }
}
