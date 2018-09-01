using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class CoinFlipState : WaveState
    {
        public static CoinFlipState self;

        static CoinFlipState()
        {
            self = new CoinFlipState();
        }

        float SizeBonusChange = 0.01f;

        int RandomCounter = 0;
        int TeamCount = 0;

        int MaxTimer = 500;
        int Timer = 0;
        List<BarTeam> Teams = new List<BarTeam>();
        int Shuffles = 10;

        public override void Enter()
        {
            Shuffles = 10;
            Teams = OverTeamBar.BarTeams;
            OverTeamBar.Clear();

            float Position = 0;
            TeamCount = 0;

            for (int i = 0; i < TeamInfo.MaxTeams; i++)
                if (FactionManager.Teams.ContainsKey(i) && FactionManager.Teams[i].Count > 0)
                {
                    OverTeamBar.AddBarTeam(new BarTeam(i, Position++));
                }

            TeamCount = FactionManager.TeamCount;
            RandomCounter = TeamCount - 1;

            OverTeamBar.SizeBonus = 0;
            OverTeamBar.Alpha = 0;
            Timer = 0;

            base.Enter();
        }

        public override void Exit()
        {
            WaveManager.CurrentWave = 1;
            WaveManager.CurrentWaveEvent = 0;
            WaveManager.CurrentTeamCount = -1;
            base.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            if (RandomCounter > 0)
            {
                if (OverTeamBar.SizeBonus < 1)
                {
                    OverTeamBar.SizeBonus = Math.Min(1, OverTeamBar.SizeBonus + SizeBonusChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f);
                    OverTeamBar.Alpha = Math.Min(1, OverTeamBar.Alpha + SizeBonusChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f);
                    OverMap.SizeMult = Math.Max(0, OverMap.SizeMult - SizeBonusChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f);
                }
                else
                {
                    Timer += gameTime.ElapsedGameTime.Milliseconds * Shuffles;
                    if (Timer > MaxTimer)
                    {

                        int SwapPos = Rand.r.Next(RandomCounter + 1);

                        Teams[RandomCounter].ListTargetPosition = SwapPos;
                        Teams[SwapPos].ListTargetPosition = RandomCounter;

                        BarTeam Temp = Teams[SwapPos];
                        Teams[SwapPos] = Teams[RandomCounter];
                        Teams[RandomCounter] = Temp;

                        Teams[SwapPos].PositionChangeSpeed = 0.01f * Shuffles;
                        Teams[RandomCounter].PositionChangeSpeed = 0.01f * Shuffles;

                        RandomCounter--;
                        if (RandomCounter == 0)
                            SoundManager.PlaySound("Open", 1, 0, 0);
                        else
                            SoundManager.PlaySound("Arive", 1, 0, 0);


                        Timer -= MaxTimer;

                        if (RandomCounter == 0 && Shuffles > 1)
                        {
                            RandomCounter = TeamCount - 1;
                            Shuffles--;
                        }
                    }
                }
            }
            else
            {
                Timer += gameTime.ElapsedGameTime.Milliseconds;
                if (Timer > MaxTimer)
                {
                    OverMap.SizeMult = Math.Min(1, OverMap.SizeMult + SizeBonusChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f);
                    OverTeamBar.SizeBonus -= gameTime.ElapsedGameTime.Milliseconds * SizeBonusChange * 60 / 1000f;
                    if (OverTeamBar.SizeBonus < 0)
                    {
                        OverTeamBar.SizeBonus = 0;
                        OverMap.SizeMult = 1;
                        WaveManager.SetState(WaveStartState.self);
                    }
                }
            }
            base.Update(gameTime);
        }
    }
}
