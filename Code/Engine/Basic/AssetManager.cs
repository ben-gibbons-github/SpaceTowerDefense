using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BadRabbit.Carrot.RenderTargetAssets;

namespace BadRabbit.Carrot
{
    public class AssetManager
    {
        private static Dictionary<string,Effect> LoadedEffects = new Dictionary<string,Effect>();
        private static LinkedList<RenderCubeAsset> CubeMapAssets = new LinkedList<RenderCubeAsset>();

        public static RenderCubeAsset RequestCubeMap(int Size, SurfaceFormat surfaceFormat, DepthFormat depthFormat, RenderCubeAsset OldCube)
        {
            RenderCubeAsset asset = OldCube;
            if (asset != null && !asset.InUse && asset.Value.Size == Size && asset.Value.Format == surfaceFormat && asset.Value.DepthStencilFormat == depthFormat)
                return OldCube;
            else
                return RequestCubeMap(Size, surfaceFormat, depthFormat);
        }

        public static RenderCubeAsset RequestCubeMap(int Size, SurfaceFormat surfaceFormat, DepthFormat depthFormat)
        {
            foreach(RenderCubeAsset asset in CubeMapAssets)
                if (!asset.InUse &&
                    asset.Value.Size == Size && asset.Value.Format == surfaceFormat && asset.Value.DepthStencilFormat == depthFormat)
                {
                    asset.InUse = true;
                    return asset;
                }
            return new RenderCubeAsset(new RenderTargetCube(Game1.graphicsDevice, Size >= 64 ? Size : 64, false, surfaceFormat, depthFormat));
        }

        public static void FreeRenderTarget(RenderTargetAsset Asset)
        {
            if (Asset != null)
                Asset.InUse = false;
        }

        public static void Request<E>(string AssetName)
        {

        }

        public static E Load<E>(string AssetName)
        {
            Console.WriteLine("Load: " + AssetName);
            try
            {
                return Game1.content.Load<E>(AssetName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return default(E);
        }

        public static TextureCubeReference LoadTextureCube(string AssetName)
        {
            Console.WriteLine("Load TextureCube: " + AssetName);
            try
            {
                TextureCube tc = Game1.content.Load<TextureCube>(AssetName);
                return new TextureCubeReference(tc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public static Effect LoadEffect(string AssetName)
        {
            Console.WriteLine("Load Effect: " + AssetName);
            if (LoadedEffects.ContainsKey(AssetName))
                return LoadedEffects[AssetName].Clone();
            else
            {
                Effect e = Game1.content.Load<Effect>(AssetName);
                if (e != null)
                    LoadedEffects.Add(AssetName, e);
                return e;
            }
        }

        public static E LoadUnsafe<E>(string AssetName) 
        {
            Console.WriteLine("Load Unsafe: " + AssetName);
            return Game1.content.Load<E>(AssetName);
        }
    }
}
