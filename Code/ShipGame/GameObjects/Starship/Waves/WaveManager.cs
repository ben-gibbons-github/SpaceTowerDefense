using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BadRabbit.Carrot.WaveFSM;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class WaveManager : GameObject
    {
        public static int GameSpeed = 2;
        public static float DifficultyMult = 1;
        public static int CurrentWave = 0;
        public static int CurrentWaveEvent = 0;
        public static int CurrentTeamCount = 0;
        public static int ActiveTeam = 0;
        public static StrikeCard CurrentStrike = null;
        public static WaveManager self;

        private static SpriteFont WaveFont;
        private static string WaveMessage = "";
        private static float WaveAlpha = 0;
        private static float WaveAlphaChange = 0.005f;

        public static bool SuperWave = false;

        WaveStateManager StateManager;

        public WaveManager()
        {
            StateManager = new WaveStateManager();
            DifficultyMult = 2;
            GameSpeed = 2;
        }

        public override void Create()
        {
            self = this;

            if (WaveFont == null)
                WaveFont = AssetManager.Load<SpriteFont>("Fonts/ShipGame/WaveFont");
            AddTag(GameObjectTag.Update);

            base.Create();
        }

        public override void CreateInGame()
        {
            StateManager.SetState(CameraFlybyState.self);
            base.CreateInGame();
        }

        public static void SetState(WaveState NewState)
        {
            self.StateManager.SetState(NewState);
        }

        public static WaveState GetState()
        {
            return self.StateManager.CurrentState;
        }

        public static void WaveEnd(SceneObject Scene)
        {
            FactionManager.WaveEnd(Scene);
        }

        public static void NewEvent(SceneObject Scene)
        {
            FactionManager.NewWaveEvent(Scene);
            NeutralManager.NewWaveEvent();
        }

        public static void NewWave(SceneObject Scene)
        {
            CurrentWave++;
            WaveMessage = "Wave " + CurrentWave.ToString();
            WaveAlpha = 1;

            FactionManager.NewWave(Scene);
            NeutralManager.NewWave();
        }

        public override void Destroy()
        {
            WaveAlpha = 0;
            CurrentWave = 0;
            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            DifficultyMult = 1.25f;
            StateManager.Update(gameTime);
            WaveAlpha -= WaveAlphaChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;

            base.Update(gameTime);
        }

        public override void UpdateEditor(GameTime gameTime)
        {
            WaveAlpha = 0;
            CurrentWaveEvent = 0;
            CurrentWave = 0;

            base.UpdateEditor(gameTime);
        }

        public static void Draw(SceneView view)
        {
            Render.DrawShadowedText(WaveFont, WaveMessage, view.Size / HUDFormat.BaseScreenSize * HUDFormat.WavePosition - new Vector2(WaveFont.MeasureString(WaveMessage).X / 2, 0)
                , Vector2.One / (WaveAlpha + 0.1f), Color.White * WaveAlpha, Color.Black * WaveAlpha);
        }

        public static void EndWave()
        {
            NeutralManager.EndWave();
        }
    }
}
