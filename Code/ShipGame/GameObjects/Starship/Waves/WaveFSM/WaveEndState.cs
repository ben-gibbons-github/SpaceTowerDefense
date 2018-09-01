using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class WaveEndState : WaveState
    {
        public static WaveEndState self;

        static WaveEndState()
        {
            self = new WaveEndState();
        }

        int Timer = 0;
        int MaxTimer = 1000;

        public override void Enter()
        {
            OverMap.NoCard();
            WaveManager.EndWave();
            WaveManager.WaveEnd(GameManager.GetLevel().getCurrentScene());

            FactionManager.LastTimedTeam = -1;

            int LiveTeamCount = FactionManager.TeamCount;

            foreach (int Key in FactionManager.TeamDead.Keys)
                if (FactionManager.TeamDead[Key])
                    LiveTeamCount--;

            if (LiveTeamCount < 2)
            {

            }

            Timer = 0;
            MaxTimer = 500;
        }

        public override void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            if (Timer > MaxTimer)
                WaveManager.SetState(WaveStartState.self);

            base.Update(gameTime);
        }
    }
}
