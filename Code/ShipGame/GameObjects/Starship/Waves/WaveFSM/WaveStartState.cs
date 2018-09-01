using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShipGame.Wave;

namespace BadRabbit.Carrot.WaveFSM
{
    public class WaveStartState : WaveState
    {
        public static WaveStartState self;

        static WaveStartState()
        {
            self = new WaveStartState();
        }

        int Timer = 0;
        int MaxTimer = 2000;

        public override void Enter()
        {
            FactionManager.NeutralUnitCount = 0;

            MaxTimer = 500;

            do
            {
                if (WaveManager.CurrentTeamCount != -1)
                {
                    BarTeam TopBar = OverTeamBar.BarTeams[0];
                    TopBar.ListTargetPosition = FactionManager.TeamCount - 1;
                    for (int i = 1; i < OverTeamBar.BarTeams.Count; i++)
                    {
                        OverTeamBar.BarTeams[i - 1] = OverTeamBar.BarTeams[i];
                        OverTeamBar.BarTeams[i].ListTargetPosition--;
                    }
                    OverTeamBar.BarTeams[OverTeamBar.BarTeams.Count - 1] = TopBar;
                }
                WaveManager.ActiveTeam = OverTeamBar.BarTeams[0].Team;
            }
            while (FactionManager.TeamDead.ContainsKey(WaveManager.ActiveTeam) && FactionManager.TeamDead[WaveManager.ActiveTeam]);

            WaveManager.CurrentTeamCount++;

            if (!FactionManager.TeamStreak.ContainsKey(WaveManager.ActiveTeam))
                FactionManager.TeamStreak.Add(WaveManager.ActiveTeam, 0);
            else
                FactionManager.TeamStreak[WaveManager.ActiveTeam]++;

            if (WaveManager.CurrentTeamCount > FactionManager.TeamCount - 1)
            {
                WaveManager.CurrentTeamCount = 0;
                WaveManager.CurrentWaveEvent += WaveManager.GameSpeed;

                if (WaveManager.CurrentWaveEvent > 1)
                {
                    WaveManager.NewWave(GameManager.GetLevel().getCurrentScene());
                    WaveManager.CurrentWaveEvent = 0;
                }
            }

            Timer = 0;
            MaxTimer = 2000;

            PathFindingManager.Rebuild();

            base.Enter();
        }

        public override void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            if (Timer > MaxTimer)
                WaveManager.SetState(PickEnemyState.self);

            base.Update(gameTime);
        }
    }
}
