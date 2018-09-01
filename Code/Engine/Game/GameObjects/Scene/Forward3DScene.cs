using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Forward3DScene : Basic3DScene
    {
        private static RenderTarget2D FinalTarget;
        private static bool Loaded = false;
        private static Effect PasteEffect;

        private LinkedList<GameObject> UpdateChildren;
        private LinkedList<GameObject> ForwardChildren;
        private LinkedList<GameObject> DepthOverChildren;
        private LinkedList<GameObject> BackgroundChildren;

        public override void Create()
        {
            UpdateChildren = AddTag(GameObjectTag.Update);
            ForwardChildren = AddTag(GameObjectTag._3DForward);
            BackgroundChildren = AddTag(GameObjectTag._3DBackground);
            DepthOverChildren = AddTag(GameObjectTag._3DDepthOver);

            Load();

            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameObject o in UpdateChildren)
                o.Update(gameTime);

            foreach (GameObject o in UpdateChildren)
                o.Update2(gameTime);

            base.Update(gameTime);
        }

        public override void SetWindowSize(Vector2 WindowSize)
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

            base.SetWindowSize(WindowSize);
        }

        new private static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;

                PasteEffect = AssetManager.Load<Effect>("Effects/Deferred/Paste");
            }
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            foreach (GameObject g in BackgroundChildren)
                g.Draw3D(DrawCamera, GameObjectTag._3DBackground);

            Game1.graphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (GameObject g in ForwardChildren)
                g.Draw3D(DrawCamera, GameObjectTag._3DForward);

            Game1.graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (GameObject g in DepthOverChildren)
                g.Draw3D(DrawCamera, GameObjectTag._3DForward);

            base.Draw3D(camera, DrawTag);
        }

        public override void PreDraw()
        {
            Game1.graphicsDevice.SetRenderTarget(FinalTarget);
            Game1.graphicsDevice.Clear(Color.Black);
            
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing && DrawCamera != null)
            {
                DrawCamera.SetOffset(Vector2.Zero);
                DrawCamera.SetMult(Vector2.One);

                Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
                Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;

                Draw3D(DrawCamera, GameObjectTag._3DForward);
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

                    Draw3D(currentCamera, GameObjectTag._3DForward);
                }
            }

            base.PreDraw();
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
#if EDITOR
            if (FinalTarget == null)
            {
                Console.WriteLine("Final Target is null");
            }
#endif

            MasterManager.SetViewportToFullscreen();
            Game1.graphicsDevice.Clear(Color.Black);

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
    }
}
