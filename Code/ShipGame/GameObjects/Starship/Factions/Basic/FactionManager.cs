using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FactionManager
    {
        public static int TeamStreakDivider = 40;

        public static List<Faction> Factions = new List<Faction>();
        public static Dictionary<int, LinkedList<Faction>> Teams = new Dictionary<int, LinkedList<Faction>>();
        public static Dictionary<int, LinkedList<BasicShipGameObject>> SortedUnits =
            new Dictionary<int, LinkedList<BasicShipGameObject>>();
        public static Dictionary<int, float> TeamStreak = new Dictionary<int, float>();
        public static Dictionary<int, bool> TeamDead = new Dictionary<int, bool>();
        public static int TeamCount = 0;

        public static int NeutralUnitCount = 0;
        public static int UnitCount = 0;

        public static int LastTimedTeam = -1;

        public static void Init()
        {
            // PlayerProfile.AddPlayer(new AI.StarShipAiController(), null, PlayerProfile.ControllerIndex.AI);
            //PlayerProfile.AddPlayer(new AI.StarShipAiController(), null, PlayerProfile.ControllerIndex.AI);
            Clear();
        }

        public static void AddDamage(float Damage)
        {
            for (int i = 0; i < Factions.Count; i++)
                AddDamage(Factions[i], Damage);
        }

        public static void AddDamage(int Team, float Damage)
        {
            for (int i = 0; i < Factions.Count; i++)
            {
                if (Factions[i].Team == Team)
                    AddDamage(Factions[i], Damage);
                else if (Factions[i].Team == WaveManager.ActiveTeam)
                    Factions[i].roundReport.DamageTaken += Damage;
            }
        }

        static void AddDamage(Faction f, float Damage)
        {
            if (f.Team != WaveManager.ActiveTeam)
            {
                f.DamageCounter += Damage;
                if (f.DamageCounter > Faction.MaxDamage())
                {
                    f.DamageCounter -= Faction.MaxDamage();
                    AddEnergy(f.FactionNumber, 1);
                    AddScore(f.FactionNumber, 2);
                }
            }
            else
                f.roundReport.DamageTaken += Damage;
        }

        public static void Update(GameTime gameTime)
        {
            foreach (Faction f in Factions)
            {
                f.Update(gameTime);
                if (f.Team == LastTimedTeam && !f.Dead)
                    f.AddMilliSeconds(gameTime.ElapsedGameTime.Milliseconds);
            }
        }

        public static void NewWave(SceneObject Scene)
        {
            foreach (GameObject o in Scene.Children)
                if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)o;
                    s.NewWave();
                }
        }

        public static void NewWaveEvent(SceneObject Scene)
        {
            foreach (GameObject o in Scene.Children)
                if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)o;
                    s.NewWaveEvent();
                }

            LastTimedTeam = WaveManager.ActiveTeam;

            foreach (Faction f in Factions)
                if (f.Team == WaveManager.ActiveTeam)
                {
                    f.SurvivedMilliSeconds = 0;
                    f.SurvivedMinutes = 0;
                    f.SurvivedSeconds = 0;

                    f.BestSurvivedWave = WaveManager.CurrentWave;
                    f.MiningPlatformCounter += WaveManager.GameSpeed;
                    if (f.MiningPlatformCounter >= f.MaxMiningPlatformCounter)
                        f.AddEvent("Mining Ring Ready!", new Color(0.5f, 0.5f, 1), FactionEvent.ReadyTexture);
                }
        }

        public static void MarkMoney()
        {
            foreach (Faction f in Factions)
                f.OldCells = f.Cells;
        }

        public static void EventMoney()
        {
            foreach (Faction f in Factions)
                if (f.Cells != f.OldCells)
                {
                    f.AddEvent("Money Made: " + (f.Cells - f.OldCells).ToString());
                }
        }

        public static void WaveEnd(SceneObject Scene)
        {
            foreach (Faction f in Factions)
                f.WaveEnd();
        }

        public static void Clear()
        {
            TeamCount = 0;
            Factions.Clear();
            Teams.Clear();
        }

        public static void Remove(int FactionNumber)
        {
            if (Teams.ContainsKey(Factions[FactionNumber].Team))
            {
                Teams[Factions[FactionNumber].Team].Remove(Factions[FactionNumber]);
                if (Teams[Factions[FactionNumber].Team].Count < 1)
                {
                    TeamCount--;
                }
            }

            Factions.Remove(Factions[FactionNumber]);

            foreach (BasicShipGameObject s in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(BasicShipGameObject)))
                if (s.FactionNumber > FactionNumber)
                    s.FactionNumber--;
        }

        public static int Add(Faction f)
        {
            f.FactionNumber = Factions.Count;
            Factions.Add(f);

            if (!Teams.ContainsKey(f.Team))
            {
                Teams[f.Team] = new LinkedList<Faction>();
                TeamCount++;
            }
            Teams[f.Team].AddLast(f);

            return Factions.Count - 1;
        }

        public static void AddCells(int FactionNumber, int Cells)
        {
            Factions[FactionNumber].Cells += Cells;
        }

        public static void AddEnergy(int FactionNumber, int Energy)
        {
            Factions[FactionNumber].Energy += Energy;
            if (Energy > 0)
                Factions[FactionNumber].roundReport.EnergyGained += Energy;
            else
                Factions[FactionNumber].roundReport.EnergyUsed -= Energy;
        }

        public static void AddScore(int FactionNumber, int Score)
        {
            Factions[FactionNumber].Score += Score;
            Factions[FactionNumber].roundReport.ScoreGained += Score;

            if (Factions[FactionNumber].Score < 0)
                Factions[FactionNumber].Score = 0;

            if (Factions[FactionNumber].Score > Factions[FactionNumber].GetSmallBombValue())
            {
                Factions[FactionNumber].roundReport.SmallBombsGained++;
                Factions[FactionNumber].SmallBombGiveCounter++;
                foreach (PlayerShip s in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(PlayerShip)))
                    if (s.FactionNumber == FactionNumber)
                    {
                        s.addSmallBomb();
                        Factions[FactionNumber].IncreamentSmallBombValue();
                    }
            }
        }

        public static void AddVictoryPoints(int FactionNumber, int VPS)
        {
            Factions[FactionNumber].VictoryPoints += VPS;
            Factions[FactionNumber].VictoryPointsString = Factions[FactionNumber].VictoryPoints.ToString();
        }

        public static void AddUnit(BasicShipGameObject s)
        {
            if (s.FactionNumber == NeutralManager.NeutralFaction)
                NeutralUnitCount += s.GetUnitWeight();
            else
            {
                if (!SortedUnits.ContainsKey(GetTeam(s.FactionNumber)))
                    SortedUnits.Add(GetTeam(s.FactionNumber), new LinkedList<BasicShipGameObject>());
                SortedUnits[GetTeam(s.FactionNumber)].AddLast(s);
            }
            UnitCount++;
        }

        public static void RemoveUnit(BasicShipGameObject s)
        {
            if (s.FactionNumber == NeutralManager.NeutralFaction)
                NeutralUnitCount -= s.GetUnitWeight();
            else
            {
                SortedUnits[s.GetTeam()].Remove(s);
            }

            UnitCount--;
        }

        public static bool CanAfford(int FactionNumber, int Cells)
        {
            return Factions[FactionNumber].Cells >= Cells;
        }

        public static bool CanAfford(int FactionNumber, int Cells, int Energy)
        {
            return Factions[FactionNumber].Cells >= Cells && Factions[FactionNumber].Energy >= Energy;
        }

        public static void Set(Faction f, int FactionNumber)
        {
            Factions[FactionNumber] = f;
        }

        public static Faction GetFaction(int FactionNumber)
        {
            return Factions[FactionNumber];
        }

        public static int GetTeam(int FactionNumber)
        {
            return FactionNumber < Factions.Count && FactionNumber != -1 ? Factions[FactionNumber].Team : NeutralManager.NeutralTeam;
        }

        public static int GetFreeTeam()
        {
            int CurrentTeam = 0;
            bool Found = false;
            while (!Found)
            {
                Found = true;
                foreach (Faction f in Factions)
                    if (f.Team == CurrentTeam)
                    {
                        Found = false;
                        CurrentTeam++;

                        if (CurrentTeam > TeamInfo.MaxTeams)
                            throw new Exception("Team Outside of Max Teams");
                    }
            }

            return CurrentTeam;
        }

        public static MiningPlatform GetMiningPlatform(int FactionNumber)
        {
            return Factions[FactionNumber].GetMiningPlatform();
        }

        public static int GetMiningPlatformCost(int FactionNumber)
        {
            return Factions[FactionNumber].GetMiningPlatformCost();
        }

        public static MiningPlatform GetBuildingPlatform(int FactionNumber)
        {
            return Factions[FactionNumber].BuildingPlatform;
        }

        public static bool CanBuildMiningPlatform(int FactionNumber)
        {
            return Factions[FactionNumber].MiningPlatformCounter >= Factions[FactionNumber].MaxMiningPlatformCounter;
        }

        public static void SetBuildingPlatform(int FactionNumber, MiningPlatform p)
        {
            Factions[FactionNumber].MiningPlatformCounter -= Factions[FactionNumber].MaxMiningPlatformCounter;

            Factions[FactionNumber].MaxMiningPlatformCounter += 2;
            if (Factions[FactionNumber].MaxMiningPlatformCounter > 8)
                Factions[FactionNumber].MaxMiningPlatformCounter = 8;

            Factions[FactionNumber].BuildingPlatform = p;
        }
    }
}
