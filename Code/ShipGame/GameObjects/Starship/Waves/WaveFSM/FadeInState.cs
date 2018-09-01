using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class FadeInState : WaveState
    {
        public static FadeInState self;

        static FadeInState()
        {
            self = new FadeInState();
        }

        public static void SetTargetState(WaveState TargetState)
        {
            self.TargetState = TargetState;
        }

        float FadeSpeed = 0.01f;

        WaveState TargetState;
        float Fade = 1;

        public override void Update(GameTime gameTime)
        {
            Fade -=FadeSpeed * gameTime.ElapsedGameTime.Milliseconds *60 / 1000f;
            if (Fade < 0)
            {
                WaveManager.SetState(TargetState);
                FadeManager.SetFadeColor(Vector4.Zero);
            }
            else
                FadeManager.SetFadeColor(new Vector4(0, 0, 0, Fade));

            base.Update(gameTime);
        }

        public override void Enter()
        {
            Fade = 1;
            base.Enter();
        }
    }
}
