#if EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public class FPSCounter
    {
        private enum PerformanceDisplay
        {
            None,
            BorderTest,
            Default,
            Extended
        }

        private static PerformanceDisplay performanceDisplay = PerformanceDisplay.Default;
        private static TimeSpan elapsedTime = TimeSpan.Zero;
        private static int frameRate = 0;
        private static int frameCounter = 0;
        private static SpriteFont Font;
        private static bool Loaded = false;
        private static Rectangle OuterBorderTestRectangle;
        private static Rectangle InnerBorderTestRectangle;

        private static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                Font = AssetManager.Load<SpriteFont>("Editor/FPSFont");

                OuterBorderTestRectangle = new Rectangle(Game1.ResolutionX / 20, Game1.ResolutionY / 20, (int)(Game1.ResolutionX * 0.9), (int)(Game1.ResolutionY * 0.9));
                InnerBorderTestRectangle = new Rectangle(Game1.ResolutionX / 10, Game1.ResolutionY / 10, (int)(Game1.ResolutionX * 0.8), (int)(Game1.ResolutionY * 0.8));
            }
        }

        public static void Update(GameTime gameTime)
        {
            if (performanceDisplay != PerformanceDisplay.None)
            {
                elapsedTime += gameTime.ElapsedGameTime;

                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }
            }
#if WINDOWS
            if (KeyboardManager.KeyJustPressed(Keys.F1))
#endif
#if XBOX
            if (PlayerProfile.getController(0) != null && PlayerProfile.getController(0).StartButton() && !PlayerProfile.getController(0).StartButton())
#endif
                performanceDisplay = performanceDisplay == PerformanceDisplay.Extended ? PerformanceDisplay.None : performanceDisplay + 1;

        }

        public static void Draw()
        {
            if (performanceDisplay != PerformanceDisplay.None)
            {
                if (performanceDisplay == PerformanceDisplay.BorderTest)
                {
                    Load();

                    Game1.spriteBatch.Begin();

                    Render.DrawOutlineRect(InnerBorderTestRectangle, 3, Color.Yellow);
                    Render.DrawOutlineRect(OuterBorderTestRectangle, 3, Color.Red);

                    Game1.spriteBatch.End();
                }
                else
                {
                    frameCounter++;

                    Load();

                    string fps = string.Format("fps: {0}", frameRate);
                    Game1.spriteBatch.Begin();

                    Render.DrawShadowedText(Font, fps, new Vector2(32), Vector2.One);
                    if (performanceDisplay == PerformanceDisplay.Extended)
                    {
                        Vector2 DrawPos = new Vector2(32, 64);

                        Render.DrawShadowedText(Font, "DrawCalls: " + Render.DrawCalls, DrawPos, Vector2.One);
                        DrawPos += new Vector2(0, 32);

                        Render.DrawShadowedText(Font, "Objects: " + GameManager.GetLevel().getCurrentScene().Children.Count, DrawPos, Vector2.One);
                        DrawPos += new Vector2(0, 32);

                        Render.DrawShadowedText(Font, "EffectUpdates: " + Render.EffectUpdateCalls, DrawPos, Vector2.One);
                        DrawPos += new Vector2(0, 32);

                        Render.DrawShadowedText(Font, Render.RenderTime.get(), DrawPos, Vector2.One);

                        if (GameManager.GetLevel().MyScene != null)
                            foreach (StopwatchWrapper sw in GameManager.GetLevel().MyScene.Watches)
                            {
                                DrawPos += new Vector2(0, 32);
                                Render.DrawShadowedText(Font, sw.get(), DrawPos, Vector2.One);
                            }


                    }
                    Game1.spriteBatch.End();

                    Render.DrawCalls = 0;
                }
            }
        }
    }
}
#endif