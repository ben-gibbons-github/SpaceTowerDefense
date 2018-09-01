#if EDITOR && WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class WorldViewer:Window
    {
        public static WorldViewer self;
        public Viewport MyViewport;


        public WorldViewer()
        {
            self = this;
            this.InnerColor = Color.Black;
        }

        public override void UpdateRenderTarget()
        {
            DisposeRenderTarget();
            MyRenderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, (int)InnerRectangle.Width, (int)InnerRectangle.Height);
            NeedsToRedraw = true;
            MyViewport = new Viewport(0, 0, MyRenderTarget.Width, MyRenderTarget.Height);
            //for (int i = 0; i < FormChildren.Count;i++ )
            //    FormChildren[i].SetPositionFromScreen(new Vector2(MyRenderTarget.Width, MyRenderTarget.Height));
        }

        public override void Update(GameTime gameTime)
        {
            if (GameManager.GetEditorLevel() != null && GameManager.GetEditorLevel().MyScene != null && GameManager.GetEditorLevel().MyScene.WindowTools != null)
                FormChildren = GameManager.GetEditorLevel().MyScene.WindowTools.FormChildren;
            else
                FormChildren = null;

            base.Update(gameTime);
            NeedsToRedraw = true;
        }

        public override void Draw()
        {
            if (GameManager.GetEditorLevel() != null && GameManager.GetEditorLevel().MyScene != null && GameManager.GetEditorLevel().MyScene.WindowTools != null)
                FormChildren = GameManager.GetEditorLevel().MyScene.WindowTools.FormChildren;
            else
                FormChildren = null;
             
            base.Draw();
        }

        public override void PreDraw()
        {
            if (GameManager.GetEditorLevel() != null && GameManager.GetEditorLevel().MyScene != null && !GameManager.GetEditorLevel().Loading)
            {
                SceneObject s = GameManager.GetEditorLevel().MyScene;
                if (s.WindowSize.X != MyRectangle.Width || s.WindowSize.Y != MyRectangle.Height)
                    s.SetWindowSize(new Vector2(Math.Max(MyRectangle.Width, 64), Math.Max(MyRectangle.Height, 64)));

                if (Game1.self.IsActive)
                    s.PreDraw();
                
                s.DrawToThumbnail();
            }
                
            base.PreDraw();
        }

        public override void DrawWindowContents()
        {
            if (GameManager.GetEditorLevel() != null && GameManager.GetEditorLevel().MyScene != null)
            {
                SceneObject s = GameManager.GetEditorLevel().MyScene;

                s.Draw2D(GameObjectTag.SceneDrawScene);
                InnerColor = GameManager.GetEditorLevel().MyScene.ClearColor;
            }

            

            Matrix scrollbarMatrix = Matrix.CreateTranslation(new Vector3(-ScrollBarHorizontal.self.X * ScrollBarHorizontal.difference, -ScrollBarVertical.self.Y * ScrollBarVertical.difference, 0));

            Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, scrollbarMatrix);
            DrawChildren();
            Game1.spriteBatch.End();

            Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, scrollbarMatrix);
            DrawAdditiveChildren();
            Game1.spriteBatch.End();
            
        }

        public override void DrawChildren()
        {
            if (FormChildren != null)
                for (int i = 0; i < FormChildren.Count(); i++)
                    if (FormChildren[i] != null)
                        FormChildren[i].Draw();
            if (HasScrollbar)
            {
                if (ScrollBarHorizontal.Active)
                    ScrollBarHorizontal.Draw();
                if (ScrollBarVertical.Active)
                    ScrollBarVertical.Draw();
            }
        }
    }
}
#endif