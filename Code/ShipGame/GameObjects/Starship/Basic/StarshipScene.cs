using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BadRabbit.Carrot.WaveFSM;
using Microsoft.Xna.Framework.Audio;

namespace BadRabbit.Carrot
{
    public class StarshipScene : Basic2DScene
    {
        private static RenderTarget2D FinalTarget;
        private static bool Loaded = false;
        private static Effect PasteEffect;

        private static Effect HudDisplaceEffect;
        private static EffectParameter HudDisplaceDisplacmentParam;
        private static EffectParameter HudDisplaceWidthParam;

        public static PlayerShip DrawingShip = null;
        public static Camera3D CurrentCamera;

        public static float SoundEffectMult = 1;

        public BloomRenderer bloomRenderer;
        public DistortionRenderer distortionRenderer;

#if WINDOWS && EDITOR
        public LinkedList<GameObject> PreDrawChildren;
#endif
        public LinkedList<GameObject> Draw2DChildren;
        public LinkedList<GameObject> SolidChildren;
        public LinkedList<GameObject> ShipChildren;

        public LinkedList<GameObject> OverDrawChildren2D;
        private LinkedList<GameObject> OverDrawViewsChildren;
        private LinkedList<GameObject> ForwardChildren;
        private LinkedList<GameObject> DepthOverChildren;
        private LinkedList<GameObject> BackgroundChildren;
        private LinkedList<GameObject> DistortionChildren;

        public QuadGrid SolidQuadGrid;

#if DRAWQUAD
        private int DrawGridTimer;          //milliseconds
        private int MaxDrawGridTimer = 100; //milliseconds
        public QuadGrid DrawQuadGrid;
#endif

        public override void Destroy()
        {
            SoundManager.LevelEnd();
            base.Destroy();
        }

        public override void Create()
        {
            SoundLibrary.Load();
            InstanceManager.Clear();
            ParticleManager.Load();
            Load();

            OverDrawChildren2D = AddTag(GameObjectTag._2DOverDraw);
            OverDrawViewsChildren = AddTag(GameObjectTag.OverDrawViews);
            Draw2DChildren = AddTag(GameObjectTag._2DForward);

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                PreDrawChildren = AddTag(GameObjectTag._2DPreDraw);
            }
            else
#endif
            {
                ForwardChildren = AddTag(GameObjectTag._3DForward);
                BackgroundChildren = AddTag(GameObjectTag._3DBackground);
                DepthOverChildren = AddTag(GameObjectTag._3DDepthOver);
            }

            SolidChildren = AddTag(GameObjectTag._2DSolid);
            ShipChildren = AddTag(GameObjectTag.ShipGameUnitBasic);
            DistortionChildren = AddTag(GameObjectTag._3DDistortion);

            SolidQuadGrid = Add(new QuadGrid());
#if DRAWQUAD
            DrawQuadGrid = Add(new QuadGrid());
#endif

            FactionManager.Init();

            base.Create();
        }

        public override void CreateInGame()
        {
            Add(new OverClock());
            Add(new OverMap());
            Add(new OverCardPicker());
            Add(new OverTeamBar());
            Add(new PathFindingManager());
            Add(new ShipGameSettings());
            MakeFadeManager();

            bloomRenderer = new BloomRenderer();
            distortionRenderer = new DistortionRenderer();

            bloomRenderer.Resize(WindowSize);
            distortionRenderer.Resize(WindowSize);

            base.CreateInGame();
        }

        public override void PlayerQuitEvent(PlayerProfile p)
        {
            int FactionNumber = -1;
            PlayerShip playerShip = null;
            
            foreach (PlayerShip s in Enumerate(typeof(PlayerShip)))
                if (s.MyProfile == p)
                {
                    playerShip = s;
                    FactionNumber = s.FactionNumber;
                }

            if (playerShip != null)
            {
                playerShip.Destroy();

                LinkedList<BasicShipGameObject> toDestroy = new LinkedList<BasicShipGameObject>();

                foreach (BasicShipGameObject o in Enumerate(typeof(BasicShipGameObject)))
                    if (o.FactionNumber == FactionNumber)
                    {
                        toDestroy.AddLast(o);
                    }

                foreach (BasicShipGameObject o in toDestroy)
                    o.Destroy();

                ChooseStartState.RemovePlayer();
                FactionManager.Remove(FactionNumber);
            }

            base.PlayerQuitEvent(p);
        }

        public override void PlayerJoinedEvent(PlayerProfile profile)
        {
            List<MineralRock> AllRocks = new List<MineralRock>();

            foreach(MineralRock r in Enumerate(typeof(MineralRock)))
                if (r.IsStartingZone.get() == 1 && r.miningPlatform == null)
                {
                    AllRocks.Add(r);
                }

            if (AllRocks.Count == 0)
                return;

            PlayerShip p = (PlayerShip)ParentLevel.AddObject(new PlayerShip(-1, profile));
            ChooseStartState.AddPlayer();
            MineralRock SelectedRock = AllRocks[Rand.r.Next(AllRocks.Count)];
            p.Position.set(SelectedRock.Position.get());
            SelectedRock.Interact(p);
        }
        
        new private static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;

                HudDisplaceEffect = AssetManager.Load<Effect>("Effects/Screen/HudDisplace");
                HudDisplaceDisplacmentParam = HudDisplaceEffect.Parameters["Displacment"];
                HudDisplaceWidthParam = HudDisplaceEffect.Parameters["ViewWidth"];

                PasteEffect = AssetManager.Load<Effect>("Effects/Deferred/Paste");
            }
        }

        public override void SetWindowSize(Vector2 WindowSize)
        {
#if EDITOR && WINDOWS
            if (!ParentLevel.LevelForEditing)
#endif
            {
                try
                {
                    if (FinalTarget != null)
                    {
                        FinalTarget.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                FinalTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, (int)WindowSize.X, (int)WindowSize.Y, false, SurfaceFormat.Color, DepthFormat.Depth24);

                if (bloomRenderer != null)
                    bloomRenderer.Resize(WindowSize);
                if (distortionRenderer != null)
                    distortionRenderer.Resize(WindowSize);
            }
            base.SetWindowSize(WindowSize);
        }

        public override void Update(GameTime gameTime)
        {
            SoundManager.Update(gameTime);
            ParticleManager.Update(gameTime);
            InstanceManager.Update(gameTime);
            FactionManager.Update(gameTime);

#if DRAWQUAD
            DrawGridTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (DrawGridTimer > MaxDrawGridTimer)
            {
                DrawGridTimer = 0;
                DrawQuadGrid.SetObjects(DrawChildren);
            }
#endif
            SolidQuadGrid.SetObjects(SolidChildren);

            base.Update(gameTime);
        }

        public override void PreDraw()
        {
#if EDITOR && WINDOWS
            if (!ParentLevel.LevelForEditing)
#endif
            {
                distortionRenderer.SetRenderTarget();

                Camera3D currentCamera = null;

                foreach (WorldViewer3D s in WorldViewerChildren)
                {
                    Render.CurrentWorldViewer3D = s;
                    s.getSceneView().Set();
                    Render.CurrentView = s.getSceneView();
                    currentCamera = s.getCamera();
                    CurrentCamera = currentCamera;

                    foreach (GameObject g in DistortionChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DDistortion);

                    InstanceManager.DrawDistortion(currentCamera);
                    ParticleManager.DrawDistortion(currentCamera);
                }
            }

            Game1.graphicsDevice.SetRenderTarget(FinalTarget);
            Game1.graphicsDevice.Clear(Color.Black);

#if EDITOR && WINDOWS
            if (!ParentLevel.LevelForEditing)
#endif
            {
                Camera3D currentCamera = null;

                foreach (WorldViewer3D s in WorldViewerChildren)
                {
                    Game1.graphicsDevice.BlendState = BlendState.Opaque;
                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

                    Render.CurrentWorldViewer3D = s;
                    if (s.GetType().Equals(typeof(PlayerShip)))
                        DrawingShip = (PlayerShip)s;

                    s.getSceneView().Set();
                    Render.CurrentView = s.getSceneView();
                    currentCamera = s.getCamera();
                    CurrentCamera = currentCamera;


                    foreach (GameObject g in BackgroundChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DBackground);

                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
                    Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

                    foreach (GameObject g in ForwardChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DForward);

                    InstanceManager.DrawShield(currentCamera);

                    ParticleManager.PreDraw(currentCamera);

                    InstanceManager.Draw(currentCamera);

                    ParticleManager.Draw(currentCamera);

                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
                    Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

                    foreach (GameObject g in DepthOverChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DForward);
                }

                MasterManager.SetViewportToFullscreen();
            }
        }

        public override void DrawScene(Camera2D DrawCamera)
        {
            if (DrawCamera == null)
                return;

#if DRAWQUAD
            DrawCamera.SetQuadGridPosition(DrawQuadGrid);
#endif

            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, DrawCamera.ViewMatrix);
            Render.AdditiveBlending = false;

#if EDITOR && WINDOWS
            foreach (GameObject o in PreDrawChildren)
                o.Draw2D(GameObjectTag._2DPreDraw);
#endif

            ChildrenDraw();

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                DrawGrid();
                objectControls.DrawControls();
            }
#endif

            Game1.spriteBatch.End();

            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, DrawCamera.ViewMatrix);
            Render.AdditiveBlending = true;
            ChildrenDraw();
            Game1.spriteBatch.End();



            Game1.spriteBatch.Begin();
            foreach (GameObject o in OverDrawChildren2D)
                o.Draw2D(GameObjectTag._2DOverDraw);
            Game1.spriteBatch.End();

            base.DrawScene(DrawCamera);
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
                DrawScene(DrawCamera);
            else
#endif
            {
                bloomRenderer.Draw(FinalTarget, distortionRenderer.distortionMap);
                Render.ViewIndex = 0;
                foreach (WorldViewer3D s in WorldViewerChildren)
                {
                    Render.ViewIndex++;
                    Render.CurrentWorldViewer3D = s;
                    s.getSceneView().Set();
                    Render.CurrentView = s.getSceneView();
                    CurrentCamera = s.getCamera();

                    if (s.GetType().Equals(typeof(PlayerShip)))
                        DrawingShip = (PlayerShip)s;

                    HudDisplaceWidthParam.SetValue(s.getSceneView().Size.X);
                    if (DrawingShip != null && DrawingShip.ShakeOffset.Length() > 0.1f)
                        HudDisplaceDisplacmentParam.SetValue(Math.Abs(DrawingShip.ShakeOffset.Length() * 10));
                    else
                        HudDisplaceDisplacmentParam.SetValue(0);


                    Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, HudDisplaceEffect);

                    if (s.GetType().Equals(typeof(PlayerShip)))
                    {
                        PlayerShip p = (PlayerShip)s;
                        TextParticleSystem.Draw(CurrentCamera);
                    }
                    foreach (GameObject o in Draw2DChildren)
                        o.Draw2D(GameObjectTag._2DForward);

                    foreach (GameObject o in OverDrawChildren2D)
                        o.Draw2D(GameObjectTag._2DOverDraw);

                    Game1.spriteBatch.End();
                }

                MasterManager.SetViewportToFullscreen();

                Game1.spriteBatch.Begin();
                foreach (GameObject o in OverDrawViewsChildren)
                    o.Draw2D(GameObjectTag.OverDrawViews);
                Game1.spriteBatch.End();
            }
        }

        private void ChildrenDraw()
        {
#if DRAWQUAD
            DrawQuadGrid.Draw2D(GameObjectTag._2DForward, DrawCamera);
#endif
#if !DRAWQUAD
            foreach (GameObject g in Draw2DChildren)
                g.Draw2D(GameObjectTag._2DForward);
#endif
        }
    }
}
