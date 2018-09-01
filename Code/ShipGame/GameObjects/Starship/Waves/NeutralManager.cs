using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShipGame.Wave;

namespace BadRabbit.Carrot
{
    public class NeutralManager : GameObject
    {
        public const int NeutralFaction = 100;
        public const int NeutralTeam = 8;
        public static LinkedList<NeutralSpawn> SpawnList = new LinkedList<NeutralSpawn>();
        private static LinkedListNode<NeutralSpawn> CurrentNode;
        public static bool WaveActive = false;

        public static WavePattern MyPattern;

        IntValue CardCount;
        StringValue PatternSource;

        public override void Create()
        {
            CardCount = new IntValue("Card Count", 20);
            PatternSource = new StringValue("Pattern Source");

            AddTag(GameObjectTag.Update);

            base.Create();
        }

        public override void CreateInGame()
        {
            if (PatternSource.get().Equals(""))
                MyPattern = new WavePattern(CardCount.get());
            else
                MyPattern = new WavePattern(CardCount.get(), PatternSource.get());

            base.CreateInGame();
        }

        public override void Destroy()
        {
            SpawnList.Clear();
            CurrentNode = null;
            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            if (MyPattern != null && WaveActive)
                MyPattern.Update(gameTime);
            base.Update(gameTime);
        }

        public override void UpdateEditor(GameTime gameTime)
        {
            SpawnList.Clear();
            CurrentNode = null;
            base.UpdateEditor(gameTime);
        }

        public static void EndWave()
        {
            if (MyPattern != null)
                MyPattern.EndWave();
        }

        public static void NewWave()
        {
            if (MyPattern != null)
                MyPattern.WaveStart();
        }

        public static void NewWaveEvent()
        {
            Stingray.MarkedTurrets.Clear();
            if (MyPattern != null)
                MyPattern.WaveEvent();
        }

        public static Vector2 GetSpawnPosition()
        {
            int Counter = 0;

            while (true)
            {
                if (CurrentNode == null)
                    CurrentNode = SpawnList.First;
                else
                    CurrentNode = CurrentNode.Next;

                Counter++;
                if (Counter > 100)
                    break;
                else if (CurrentNode != null &&
                (PathFindingManager.GetCellValue(CurrentNode.Value.Position.get()) > PathFindingManager.StartingCell - 50 &&
                PathFindingManager.GetCellValue(CurrentNode.Value.Position.get()) < PathFindingManager.StartingCell - 20) &&
                    PathFindingManager.GetAreaClear(CurrentNode.Value.Position.get()))
                    break;
            }

            if (CurrentNode != null)
            {
                Vector2 p = CurrentNode.Value.Position.get() + CurrentNode.Value.Offset;
                CurrentNode.Value.UpdateOffset(new Vector2(32));
                return p;
            }
            else
                return Vector2.Zero;
        }

        public static void SpawnUnit(UnitBasic b)
        {
            b.SetPosition(GetSpawnPosition());
            b.AddReward();
        }
    }
}
