using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BadRabbit.Carrot.WaveFSM;
using ShipGame.Wave;

namespace BadRabbit.Carrot
{
    public class OverCardPicker : SizeNodeObject
    {
        public static bool CanPick = false;
        public static List<WaveCard> CurrentCards = new List<WaveCard>();
        public static Dictionary<int, int> TeamSelectedNodes = new Dictionary<int, int>();
        static Dictionary<int, float> TeamFloatingSelection = new Dictionary<int, float>();
        static Dictionary<int, bool> TeamReady = new Dictionary<int, bool>();
        public static bool SinglePlayer = false;
        public static bool Ready = false;

        public static float OtherAlpha = 1;
        static float OtherAlphaChange = 0.05f;

        static int SingleSelectedNode = 0;

        public static float SizeBonus = 0;

        static OverCardPicker self;
        static int SelectedSlot = 0;

        SizeNode RectL;
        SizeNode RectR;

        SizeNode LargeRectL;
        SizeNode LargeRectR;

        Vector2 InterpolatedPositionL;
        Vector2 InterpolatedPositionR;
        Vector2 InterpolatedDifference;

        int ShiftOffset;
        

        public OverCardPicker()
        {
            self = this;
        }

        public static void Reset(int CardCount)
        {
            SingleSelectedNode = 0;
            CurrentCards.Clear();
            TeamSelectedNodes.Clear();
            TeamFloatingSelection.Clear();
            TeamReady.Clear();

            LinkedList<string> TypeList = new LinkedList<string>();

            for (int i = 0; i < CardCount; i++)
            {
                WaveCard newCard = CardLoader.GetRandomCard(WaveManager.CurrentWave);

                int c = 0;
                while (newCard.Used && c++ < 100)
                    newCard = CardLoader.GetRandomCard(WaveManager.CurrentWave);
                CurrentCards.Add(newCard);
                newCard.Used = true;
                //TypeList.AddLast(newCard.Type);
            }

            foreach (int Team in FactionManager.Teams.Keys)
                if (Team != WaveManager.ActiveTeam)
                {
                    TeamSelectedNodes.Add(Team, 0);
                    TeamReady.Add(Team, false);
                    TeamFloatingSelection.Add(Team, 0);
                }
        }

        public static bool TeamIsReady(int Team)
        {
            return WaveManager.ActiveTeam == Team ? true : TeamReady.ContainsKey(Team) ? TeamReady[Team] : false;
        }

        public static void TeamMoveRight(int Team)
        {
            if (TeamSelectedNodes.ContainsKey(Team))
                TeamSelectedNodes[Team] = TeamSelectedNodes[Team] == CurrentCards.Count - 1 ? 0 : TeamSelectedNodes[Team] + 1;
        }

        public static void TeamMoveLeft(int Team)
        {
            if (TeamSelectedNodes.ContainsKey(Team))
                TeamSelectedNodes[Team] = TeamSelectedNodes[Team] == 0 ? CurrentCards.Count - 1 : TeamSelectedNodes[Team] - 1;
        }

        public static void SingleMove()
        {
            SingleSelectedNode = SingleSelectedNode == CurrentCards.Count - 1 ? 0 : SingleSelectedNode + 1;
        }

        public static void ReadySingle()
        {
            NeutralManager.MyPattern.CurrentCard = CurrentCards[SingleSelectedNode];
            Ready = true;

            for (int i = 0; i < CurrentCards.Count; i++)
                if (CurrentCards[i] == NeutralManager.MyPattern.CurrentCard)
                {
                    SelectedSlot = i;
                    return;
                }
        }

        public static void ReadyTeamNow()
        {
            int Best = 0;
            int BestVotes = -1;

            for (int i = 0; i < CurrentCards.Count; i++)
            {
                int Votes = 0;
                foreach (int key in TeamSelectedNodes.Keys)
                    if (TeamReady[key] && i == TeamSelectedNodes[key])
                        Votes++;

                if (Votes > BestVotes)
                {
                    BestVotes = Votes;
                    Best = i;
                }
            }

            NeutralManager.MyPattern.CurrentCard = CurrentCards[Best];
            OverMap.UnitSubtitle = CurrentCards[Best].Name;
            Ready = true;

            for (int i = 0; i < CurrentCards.Count; i++)
                if (CurrentCards[i] == NeutralManager.MyPattern.CurrentCard)
                {
                    SelectedSlot = i;
                    return;
                }
        }

        public static void ReadyTeam(int Team)
        {
            if (!CanPick)
                return;

            TeamReady[Team] = true;

            bool AllReady = true;
            foreach (bool b in TeamReady.Values)
                if (!b)
                    AllReady = false;

            if (AllReady)
            {
                ReadyTeamNow();
            }
        }

        public override void Create()
        {
            AddTag(GameObjectTag.OverDrawViews);
            AddTag(GameObjectTag.Update);

            RectL = AddSizeNode(new SizeNode(new Vector2(150, 500)));
            RectR = AddSizeNode(new SizeNode(new Vector2(200, 500)));

            LargeRectL = AddSizeNode(new SizeNode(new Vector2(200, 300)));
            LargeRectR = AddSizeNode(new SizeNode(new Vector2(800, 700)));

            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (WaveCard c in CurrentCards)
                c.UpdatePicker(gameTime);

            if (Ready)
            {
                OtherAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * OtherAlphaChange;
                if (OtherAlpha < 0)
                    OtherAlpha = 0;
            }
            else
                OtherAlpha = 1;

            InterpolatedPositionL = (RectL.Position + (LargeRectL.Position - RectL.Position) * SizeBonus) /
                OverFormat.BaseScreenSize * MasterManager.FullScreenSize;
            InterpolatedPositionR = (RectR.Position + (LargeRectR.Position - RectR.Position) * SizeBonus) /
                OverFormat.BaseScreenSize * MasterManager.FullScreenSize;
            InterpolatedDifference = InterpolatedPositionR - InterpolatedPositionL;

            if (NeutralManager.MyPattern.CurrentCard != null)
            {
                ShiftOffset += gameTime.ElapsedGameTime.Milliseconds / 2;
                if (ShiftOffset > SelectedSlot * 200)
                    ShiftOffset = SelectedSlot * 200;
            }
            else
            {
                ShiftOffset -= gameTime.ElapsedGameTime.Milliseconds / 2;
                if (ShiftOffset < 0)
                    ShiftOffset = 0;
            }

            LargeRectL.TargetPosition = new Vector2(200 - ShiftOffset, 300);
            LargeRectR.TargetPosition = new Vector2(800 - ShiftOffset, 700);

            foreach (int i in TeamSelectedNodes.Keys)
            {
                TeamFloatingSelection[i] += (TeamSelectedNodes[i] - TeamFloatingSelection[i]) * 0.05f * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
            }

            base.Update(gameTime);
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (SizeBonus == 0) 
                return;

            Render.DrawSolidRect(InterpolatedPositionL, InterpolatedPositionR, new Color(0, 0, 0, 0.5f));

            int i;

            for (i = 0; i < CurrentCards.Count; i++)
                CurrentCards[i].Draw(InterpolatedPositionL + new Vector2(InterpolatedDifference.X * ((0.1f + (float)i) / CurrentCards.Count), InterpolatedDifference.Y * 0.1f),
                    InterpolatedPositionL + new Vector2(InterpolatedDifference.X * ((0.9f + (float)i) / CurrentCards.Count), InterpolatedDifference.Y * 0.9f), i == SelectedSlot ? 1 : OtherAlpha);


            if (SinglePlayer || WaveManager.CurrentWave < PickEnemyState.RandomRounds + 1)
            {
                Render.DrawSprite(WaveCard.UnitPicker, InterpolatedPositionL + new Vector2((SingleSelectedNode + 0.5f) * InterpolatedDifference.X / CurrentCards.Count, InterpolatedDifference.Y + i * 15),
                    new Vector2(InterpolatedDifference.X / CurrentCards.Count * 1.5f, 90), 0, new Color(1, 0.5f, 0.5f));
            }
            else
            {
                i = 0;
                foreach (int key in TeamFloatingSelection.Keys)
                    Render.DrawSprite(WaveCard.UnitPicker, InterpolatedPositionL + new Vector2((TeamFloatingSelection[key] + 0.5f) * InterpolatedDifference.X / CurrentCards.Count, InterpolatedDifference.Y + i * 15)
                        , new Vector2(InterpolatedDifference.X / CurrentCards.Count * 1.5f, 90), 0, TeamInfo.GetColor(key));
            }

            base.Draw2D(DrawTag);
        }
    }
}
