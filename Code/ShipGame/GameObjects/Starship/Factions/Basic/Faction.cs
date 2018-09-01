using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Faction
    {
        public static float MaxDamage()
        {
            return 10;
        }

        public int GetSmallBombValue()
        {
            return 500 + SmallBombValue;
        }

        const int MaxEvents = 12;
        int EventCounter = 0;
        FactionEvent[] Events = new FactionEvent[MaxEvents];
        public RoundReport roundReport = new RoundReport();

        private int SmallBombValue = 0;
        private int SmallBombIncreament = 500;

        public void IncreamentSmallBombValue()
        {
            SmallBombValue += SmallBombIncreament;
            SmallBombIncreament += 500 * FactionManager.TeamCount;
        }

        public PlayerProfile Owner = null;
        public int FactionNumber = 0;
        public int Team;

        public int BestSurvivedWave = 0;

        public int SurvivedMilliSeconds = 0;
        public int SurvivedSeconds = 0;
        public int SurvivedMinutes = 0;

        public void AddMilliSeconds(int MilliSeconds)
        {
            SurvivedMilliSeconds += MilliSeconds;
            while (SurvivedMilliSeconds > 1000)
            {
                SurvivedMilliSeconds -= 1000;
                SurvivedSeconds++;
                if (SurvivedSeconds > 60)
                {
                    SurvivedSeconds -= 60;
                    SurvivedMinutes++;
                    if (SurvivedMinutes > 60)
                        SurvivedMinutes = 60;
                }
            }
        }

        public MiningPlatform BuildingPlatform = null;
        public int MiningPlatformCounter = 0;
        public int MaxMiningPlatformCounter = 2;
        public int MiningPlatformCount = 0;

        public int Cells;
        public int OldCells;
        public int Score;
        public int Energy;
        public int SmallBombGiveCounter = 1;

        public float DamageCounter = 0;

        public int VictoryPoints;
        public string VictoryPointsString = "";
        public Vector2 VictoryPointsStringSize;

        public List<FactionCard> Cards;
        public static int MaxCards = 5;

        public int CardPickPosition = 0;


        public bool PickingCards = true;
        public bool Dead = true;

        public Faction(int Team)
        {
            this.Team = Team;
            MiningPlatformCounter = 2;

            Cards = new List<FactionCard>();

            for (int i = 0; i < MaxEvents; i++)
                Events[i] = new FactionEvent();
        }

        public void WaveEnd()
        {
            if (roundReport.ScoreGained > 0)
                AddEvent("Score Gained: " + roundReport.ScoreGained.ToString());
            if (roundReport.DamageDone > 0)
                AddEvent("Damage Done: " + roundReport.DamageDone.ToString());
            if (roundReport.EnergyGained > 0)
                AddEvent("Energy Gained: " + roundReport.EnergyGained.ToString());
            if (roundReport.EnergyUsed > 0)
                AddEvent("Energy Used: " + roundReport.EnergyUsed.ToString());
            if (roundReport.DamageTaken > 0)
                AddEvent("Damage Taken: " + roundReport.DamageTaken.ToString());
            if (roundReport.SmallBombsGained > 0)
                AddEvent("Small Bombs Gained: " + roundReport.SmallBombsGained.ToString());
            if (roundReport.SmallBombsUsed > 0)
                AddEvent("Small Bombs Used: " + roundReport.SmallBombsUsed.ToString());
            if (roundReport.PlayerKills > 0)
                AddEvent("Player Kills: " + roundReport.PlayerKills.ToString());
            if (roundReport.PlayerAssists > 0)
                AddEvent("Player Assists: " + roundReport.PlayerAssists.ToString());
            if (roundReport.Deaths > 0)
                AddEvent("Deaths: " + roundReport.Deaths.ToString());
            if (roundReport.UnitKills > 0)
                AddEvent("Unit Kills: " + roundReport.UnitKills.ToString());
            if (roundReport.UnitsSpawned > 0)
                AddEvent("Units Spawned: " + roundReport.UnitsSpawned.ToString());
            if (roundReport.MiningPlatformsLost > 0)
                AddEvent("Mining Platforms Lost: " + roundReport.MiningPlatformsLost.ToString());
            if (roundReport.TurretsLost > 0)
                AddEvent("Turrets Lost: " + roundReport.TurretsLost.ToString());
            if (roundReport.MiningPlatformsDestroyed > 0)
                AddEvent("Mining Platforms Destroyed: " + roundReport.MiningPlatformsDestroyed.ToString());
            if (roundReport.TurretsKilled > 0)
                AddEvent("Turrets Destroyed: " + roundReport.TurretsKilled.ToString());

            roundReport.Reset();
        }

        public void Update(GameTime gameTime)
        {
            if (EventCounter > 0 && Events[0].Update(gameTime))
            {
                FactionEvent temp = Events[0];
                Events[0] = Events[EventCounter - 1];
                Events[EventCounter - 1] = temp;
                EventCounter--;
            }
        }

        public void AddEvent(string EventString, Color color, Texture2D Texture)
        {
            if (EventCounter < MaxEvents)
            {
                Events[EventCounter].Create(EventString, color, Texture);
                EventCounter++;
            }
        }

        public void AddEvent(string EventString)
        {
            if (EventCounter < MaxEvents)
            {
                Events[EventCounter].Create(EventString, Color.White, null);
                EventCounter++;
            }
        }

        public virtual MiningPlatform GetMiningPlatform() 
        {
            return new HumanMiningPlatform(FactionNumber); 
        }

        public virtual int GetMiningPlatformCost() 
        {
            return 0;
        }

        protected void Add(FactionCard card)
        {
            Cards.Add(card);

            if (card.GetType().IsSubclassOf(typeof(TurretCard)))
            {
                TurretCard c = (TurretCard)card;
                c.Reset();
            }
        }

        public void Draw(Vector2 Position)
        {
            if (EventCounter > 0)
                Events[0].Draw(Position);
        }
    }
}
