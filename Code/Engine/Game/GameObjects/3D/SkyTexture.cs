using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BadRabbit.Carrot
{
    public class SkyTexture: ReturnsTextureCube
    {
        public static bool Loaded = false;
        public static Texture2D SkyFaceTexture;

        public StringValue Path;
        public FloatValue Resolution;

        public override void Create()
        {
            MyCube = new TextureCubeReference();
            Path = new StringValue("Path");
            Path.ChangeEvent = PathChange;
            Resolution = new FloatValue("Resolution", 1024);
            Resolution.ChangeEvent = PathChange;
            Load();
            base.Create();
        }

        public override void SetWindowSize(Vector2 Size)
        {
            PathChange();
            base.SetWindowSize(Size);
        }

        new public static void Load()
        {
            Deferred3DScene.Load();
        }

        private void PathChange()
        {
            RenderTargetCube tc = new RenderTargetCube(Game1.graphicsDevice, Math.Max(64, (int)Resolution.get()), false, SurfaceFormat.Color, DepthFormat.None);
            
            {

                for (int i = 0; i < 6;i++ )
                {
                    CubeMapFace face = (CubeMapFace)i;
                    SkyFaceTexture = AssetManager.Load<Texture2D>(this.GetParent().TextureDirectory.get() +
                        Path.get() + face.ToString().Substring(0, 3).ToLower() +
                        face.ToString().Last().ToString().ToUpper());
                    Game1.graphicsDevice.SetRenderTarget(tc, face);
                    Game1.graphicsDevice.Clear(Color.Black);
                    Game1.graphicsDevice.Textures[0] = SkyFaceTexture;
                    Deferred3DScene.PasteEffect.CurrentTechnique.Passes[0].Apply();
                    FullscreenQuad.Draw();
                }
            }
            
            Game1.graphicsDevice.SetRenderTarget(null);
            Game1.graphicsDevice.Clear(Color.Transparent);
            if (MyCube.get() != null)
                MyCube.get().Dispose();
            MyCube.set(tc);
        }
        public override void Destroy()
        {
            if (MyCube.get() != null)
                MyCube.get().Dispose();
            base.Destroy();
        }
    }
}
