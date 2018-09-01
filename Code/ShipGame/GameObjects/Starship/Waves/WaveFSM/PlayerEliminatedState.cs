using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class PlayerEliminatedState : WaveState
    {
        public static PlayerEliminatedState self;
        public static int LastEliminatedTeam = -1;
        public static string EliminatedString;
        public static Vector2 EliminatedStringSize;

        static PlayerEliminatedState()
        {
            self = new PlayerEliminatedState();
        }

        int Timer;
        int MaxTimer;


        public override void Enter()
        {
            Timer = 0;
            MaxTimer = 3000;
            EliminatedString = "";
            int FactionCount = 0;

            foreach (Faction f in FactionManager.Factions)
                if (f.Team == LastEliminatedTeam)
                    FactionCount++;

            int MaxFactions = FactionCount;

            foreach (Faction f in FactionManager.Factions)
                if (f.Team == LastEliminatedTeam)
                {
                    if (FactionCount == 2)
                    {
                        if (MaxFactions == 2)
                            EliminatedString += " and ";
                        else
                            EliminatedString += " and, ";
                    }
                    else
                        EliminatedString += ", ";

                    EliminatedString += f.Owner.PlayerName;
                    FactionCount--;
                }

            EliminatedString += " eliminated!";
            EliminatedStringSize = FactionEvent.FeedFont.MeasureString(EliminatedString);

            base.Enter();
        }

        public override void Exit()
        {
            LastEliminatedTeam = -1;
            base.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            if (Timer > MaxTimer)
                WaveManager.SetState(WaveEndState.self);
        }
    }
}
