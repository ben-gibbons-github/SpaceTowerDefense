using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class ChooseStartState : WaveState
    {
        public static ChooseStartState self;

        static ChooseStartState()
        {
            self = new ChooseStartState();
        }

        int SecondsTimer = 60;
        int MillTimer = 0;
        int ReadyCounter = 0;
        int MaxReadyCount = 0;
        float SizeBonusChange = 0.01f;

        public override void Update(GameTime gameTime)
        {
            OverMap.SizeMult = Math.Min(1, OverMap.SizeMult + SizeBonusChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f);

            if (ReadyCounter > 0)
            {
                MillTimer += gameTime.ElapsedGameTime.Milliseconds;
                while (MillTimer > 1000)
                {
                    MillTimer -= 1000;
                    SecondsTimer -= 1;

                    if (SecondsTimer == 0)
                    {
                        WaveManager.SetState(CoinFlipState.self);
                    }
                }
            }

            base.Update(gameTime);
        }

        public static void Activate()
        {
            self.ReadyCounter++;
            if (self.ReadyCounter == self.MaxReadyCount)
                WaveManager.SetState(CoinFlipState.self);
        }

        public static void AddPlayer()
        {
            self.MaxReadyCount++;
        }

        public static void RemovePlayer()
        {
            self.MaxReadyCount--;
        }

        public override void Enter()
        {
            SecondsTimer = 60;
            MillTimer = 0;
            MaxReadyCount = 0;
            ReadyCounter = 0;

            foreach (PlayerShip s in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(PlayerShip)))
                MaxReadyCount++;
            base.Enter();
        }

        public override void Exit()
        {
            GameManager.GetLevel().AllowJoining = false;
            base.Exit();
        }
    }
}
