using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
 {
    public class StarshipMenuScene : Basic2DScene
    {
        private static RenderTarget2D FinalTarget;
        private static Effect PasteEffect;

        private LinkedList<GameObject> Draw2DChildren;
        private LinkedList<GameObject> ForwardChildren;
        private LinkedList<GameObject> OverDrawViewsChildren;
        private static bool Loaded;

        public override void Create()
        {
            Load();
            ParticleManager.Load();

            ForwardChildren = AddTag(GameObjectTag._3DForward);
            OverDrawViewsChildren = AddTag(GameObjectTag.OverDrawViews);
            Draw2DChildren = AddTag(GameObjectTag._2DForward);
            AddTag(GameObjectTag.Form);

            base.Create();
        }

        public override void PlayerJoinedEvent(PlayerProfile p)
        {
            PlayerMarker m = new PlayerMarker(p);

            foreach (GameObject o in GetList(GameObjectTag.Form))
            {
                BasicGameForm f = (BasicGameForm)o;
                if (f.StartingForm.get())
                {
                    m.SetCurrentForm(f);
                }
            }

            Add(m);
            base.PlayerJoinedEvent(p);
        }

        public override void PlayerQuitEvent(PlayerProfile p)
        {
            GameObject ToDestroy = null;
            foreach (GameObject g in Children)
                if (g.GetType().Equals(typeof(PlayerMarker)) || g.GetType().IsSubclassOf(typeof(PlayerMarker)))
                {
                    PlayerMarker m = (PlayerMarker)g;
                    if (m.MyPlayer == p)
                    {
                        ToDestroy = m;
                        break;
                    }
                }
            if (ToDestroy != null)
                ToDestroy.Destroy();

            base.PlayerQuitEvent(p);
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
            }

            base.SetWindowSize(WindowSize);
        }

        public override void Update(GameTime gameTime)
        {
            ParticleManager.Update(gameTime);
            base.Update(gameTime);
        }

        public override void PreDraw()
        {
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

                    s.getSceneView().Set();
                    Render.CurrentView = s.getSceneView();
                    currentCamera = s.getCamera();
                    StarshipScene.CurrentCamera = currentCamera;

                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;
                    Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

                    foreach (GameObject g in ForwardChildren)
                        g.Draw3D(currentCamera, GameObjectTag._3DForward);

                    ParticleManager.PreDraw(currentCamera);
                    ParticleManager.Draw(currentCamera);

                    Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
                    Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
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

            foreach (GameObject g in Draw2DChildren)
                g.Draw2D(GameObjectTag._2DForward);

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

            foreach (GameObject g in Draw2DChildren)
                g.Draw2D(GameObjectTag._2DForward);

            Game1.spriteBatch.End();


            Game1.spriteBatch.Begin();
            foreach (GameObject g in OverDrawViewsChildren)
                g.Draw2D(GameObjectTag.OverDrawViews);
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
                Game1.graphicsDevice.SetRenderTarget(null);
                Game1.graphicsDevice.Clear(Color.Black);

                Game1.graphicsDevice.BlendState = BlendState.Opaque;
                Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

                Game1.graphicsDevice.Textures[0] = FinalTarget;
                PasteEffect.Techniques[0].Passes[0].Apply();

                FullscreenQuad.Draw();

                DrawScene(DrawCamera);
            }
        }

        public override void CreateInGame()
        {
            base.CreateInGame();
        }

        new private static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;

                PasteEffect = AssetManager.Load<Effect>("Effects/Deferred/Paste");
            }
        }
    }
}
