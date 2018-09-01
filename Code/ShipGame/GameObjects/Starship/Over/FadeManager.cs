using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FadeManager : GameObject
    {
        public static FadeManager self;

        static Effect BasicFadeEffect;
        static EffectParameter BasicFadeColorParam;
        static bool Loaded = false;

        ColorValue FadeColor;
        IntValue FadeType;

        static bool Fading;
        static float FadingAlpha = 0;
        static SceneObject TargetScene;
        static string TargetLevel;

        public FadeManager()
        {
            self = this;
            Load();
        }

        public override void Create()
        {
            AddTag(GameObjectTag.OverDrawViews);
            AddTag(GameObjectTag.Update);

            FadeColor = new ColorValue("Fade Color", Vector4.Zero);
            FadeType = new IntValue("Fade Type");

            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            if (Fading)
            {
                FadingAlpha += gameTime.ElapsedGameTime.Milliseconds * 1 / 1000f;
                if (FadingAlpha > 1)
                {
                    FadingAlpha = 1;
                    Fading = false;

                    if (TargetScene != null)
                    {
                        ParentLevel.SetScene(TargetScene);
                        TargetScene = null;
                    }
                    else
                        GameManager.LoadTitleLevel(TargetLevel);
                }

                SetFadeColor(new Vector4(0, 0, 0, FadingAlpha));
            }
            else if (FadingAlpha > 0)
            {
                FadingAlpha -= gameTime.ElapsedGameTime.Milliseconds * 1 / 1000f;
                if (FadingAlpha < 0)
                    FadingAlpha = 0;

                SetFadeColor(new Vector4(0, 0, 0, FadingAlpha));
            }

            base.Update(gameTime);
        }

        new static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                BasicFadeEffect = AssetManager.Load<Effect>("Effects/Screen/BasicFade");
                BasicFadeColorParam = BasicFadeEffect.Parameters["Color"];
            }
        }

        public static void SetFadeColor(Vector4 Color)
        {
            self.FadeColor.set(Color);
        }

        public static void SetFadingTarget(SceneObject s)
        {
            TargetScene = s;
            FadingAlpha = 0;
            Fading = true;
        }

        public static void SetFadingTarget(string s)
        {
            TargetLevel = s;
            FadingAlpha = 0;
            Fading = true;
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (FadeColor.A() > 0)
            {
                switch (FadeType.get())
                {
                    default:
                        Game1.graphicsDevice.BlendState = BlendState.AlphaBlend;
                        BasicFadeEffect.CurrentTechnique.Passes[0].Apply();
                        BasicFadeColorParam.SetValue(FadeColor.get());

                        break;
                }

                FullscreenQuad.Draw();
            }

            base.Draw2D(DrawTag);
        }
    }
}
