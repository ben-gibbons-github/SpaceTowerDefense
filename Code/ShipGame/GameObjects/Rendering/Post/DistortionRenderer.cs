using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class DistortionRenderer
    {
        public RenderTarget2D distortionMap;

        public DistortionRenderer()
        {
            Load();
        }

        void Load()
        {
            PresentationParameters pp = Game1.graphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;
            DepthFormat depthFormat = pp.DepthStencilFormat;

            distortionMap = new RenderTarget2D(Game1.graphicsDevice, width, height, false, format, depthFormat);
        }

        public void Resize(Vector2 WindowSize)
        {
            if (WindowSize.X < 100 || WindowSize.Y < 0)
                return;

            try
            {
                if (distortionMap != null)
                {
                    distortionMap.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            PresentationParameters pp = Game1.graphicsDevice.PresentationParameters;
            int width = (int)WindowSize.X;
            int height = (int)WindowSize.Y;
            SurfaceFormat format = pp.BackBufferFormat;
            DepthFormat depthFormat = pp.DepthStencilFormat;

            distortionMap = new RenderTarget2D(Game1.graphicsDevice, width, height, false, format, depthFormat);
        }

        public void SetRenderTarget()
        {
            Game1.graphicsDevice.SetRenderTarget(distortionMap);
            Game1.graphicsDevice.Clear(new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}
