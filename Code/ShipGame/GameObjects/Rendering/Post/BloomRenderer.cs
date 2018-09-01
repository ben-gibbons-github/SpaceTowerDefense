using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class BloomRenderer
    {
        public static BloomRenderer self;

        Effect bloomExtractEffect;
        Effect bloomCombineEffect;
        Effect gaussianBlurEffectHorizontal;
        Effect gaussianBlurEffectVertical;

        RenderTarget2D renderTarget1;
        RenderTarget2D renderTarget2;

        float BloomThreshold = 0.9f;
        float BlurAmount;
        float BloomIntensity;
        float BaseIntensity;
        float BloomSaturation;
        float BaseSaturation;

        bool Ready = false;

        public BloomRenderer()
        {
            BloomIntensity = 0.5f;
            BaseIntensity = 1f;
            BloomSaturation = 1.75f / 2;
            BaseSaturation = 0.75f;

            self = this;
            Load();
        }

        public void Resize(Vector2 WindowSize)
        {
            if (WindowSize.X < 100 || WindowSize.Y < 0)
                return;

            try
            {
                if (renderTarget1 != null)
                    renderTarget1.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                if (renderTarget2 != null)
                    renderTarget2.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            PresentationParameters pp = Game1.graphicsDevice.PresentationParameters;
            int width = (int)WindowSize.X / 2;
            int height = (int)WindowSize.Y / 2;
            SurfaceFormat format = pp.BackBufferFormat;
            DepthFormat depthFormat = pp.DepthStencilFormat;

            renderTarget1 = new RenderTarget2D(Game1.graphicsDevice, width, height, false, format, depthFormat);
            renderTarget2 = new RenderTarget2D(Game1.graphicsDevice, width, height, false, format, depthFormat);
        }

        public void Load()
        {
            bloomExtractEffect = Game1.content.Load<Effect>("Effects/Post/BloomExtract");
            bloomCombineEffect = Game1.content.Load<Effect>("Effects/Post/BloomCombine");
            gaussianBlurEffectHorizontal = Game1.content.Load<Effect>("Effects/Post/GaussianBlur");
            gaussianBlurEffectVertical = gaussianBlurEffectHorizontal.Clone();

            // Look up the resolution and format of our main backbuffer.
            PresentationParameters pp = Game1.graphicsDevice.PresentationParameters;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            SurfaceFormat format = pp.BackBufferFormat;

            // Create two rendertargets for the bloom processing. These are half the
            // size of the backbuffer, in order to minimize fillrate costs. Reducing
            // the resolution in this way doesn't hurt quality, because we are going
            // to be blurring the bloom images in any case.
            width /= 3;
            height /= 3;

            renderTarget1 = new RenderTarget2D(Game1.graphicsDevice, width, height, false, format, DepthFormat.None);
            renderTarget2 = new RenderTarget2D(Game1.graphicsDevice, width, height, false, format, DepthFormat.None);
        }

        public static void SetSaturation(float NewSat)
        {
            self.Ready = false;
            self.BaseSaturation = 1.5f * NewSat / 10f;
            self.BloomSaturation = 1.75f * NewSat / 10f;
        }

        public static void SetIntensity(float NewInt)
        {
            self.Ready = false;
            self.BloomIntensity = 0.5f * NewInt / 10f;
            self.BaseIntensity = 2f * NewInt / 10f;
        }

        public void Draw(RenderTarget2D SceneRenderTarget, RenderTarget2D DisplacementRenderTarget)
        {
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
            Game1.graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            if (!Ready)
            {
                Ready = true;

                BloomThreshold = 0.5f;
                BlurAmount = 10f;

                EffectParameterCollection parameters = bloomCombineEffect.Parameters;
                parameters["BloomIntensity"].SetValue(BloomIntensity);
                parameters["BaseIntensity"].SetValue(BaseIntensity);
                parameters["BloomSaturation"].SetValue(BloomSaturation);
                parameters["BaseSaturation"].SetValue(BaseSaturation);

                bloomExtractEffect.Parameters["BloomThreshold"].SetValue(
                    BloomThreshold);


                SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0, gaussianBlurEffectVertical);
                SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height, gaussianBlurEffectHorizontal);
            }

            Game1.graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            DrawFullscreenQuad(SceneRenderTarget, renderTarget1,
                               bloomExtractEffect);

            DrawFullscreenQuad(renderTarget1, renderTarget2,
                               gaussianBlurEffectHorizontal);

            DrawFullscreenQuad(renderTarget2, renderTarget1,
                               gaussianBlurEffectVertical);

            Game1.graphicsDevice.SetRenderTarget(null);
            Game1.graphicsDevice.Textures[1] = SceneRenderTarget;
            Game1.graphicsDevice.Textures[2] = DisplacementRenderTarget;

            MasterManager.SetViewportToFullscreen();

            DrawFullscreenQuad(renderTarget1, null, bloomCombineEffect);
        }

        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
                            Effect effect)
        {
            Game1.graphicsDevice.BlendState = BlendState.Opaque;
            Game1.graphicsDevice.DepthStencilState = DepthStencilState.None;
            //Game1.graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Game1.graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            Game1.graphicsDevice.SetRenderTarget(renderTarget);
            Game1.graphicsDevice.Clear(Color.Black);
            Game1.graphicsDevice.Textures[0] = texture;

            effect.Techniques[0].Passes[0].Apply();
            FullscreenQuad.Draw();
        }

        void SetBlurEffectParameters(float dx, float dy, Effect gaussianBlurEffect)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                //float sampleOffset = i * 2 + 1.5f;
                float sampleOffset = i + 0.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        float ComputeGaussian(float n)
        {
            float theta = BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }

    }
}
