using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PathFindingManager : GameObject
    {
        public const int DeadCell = -1;
        public const int NeutralCell = 0;
        public const int StartingCell = 100000;

        public static PathFindingManager self;
        
        public PathFindingManager()
        {
            self = this;
        }

        public IntValue CellsX;
        public IntValue CellsY;

        public bool Drawing = false;
        public float[,] CellAlpha;

        public int[,] CellGrid;
        public int[,] AttackGrid;
        public static Queue<int> CellJobQue = new Queue<int>();
        Queue<int> AttackJobQue = new Queue<int>();
        LinkedList<PathfindingHotPoint> HotPoints = new LinkedList<PathfindingHotPoint>();

        Basic2DScene Parent2DScene;
        public Vector2 Divisor;

        public override void Create()
        {
            CellsX = new IntValue("CellsX", 100);
            CellsY = new IntValue("CellsY", 100);
            CellGrid = new int[100, 100];
            CellAlpha = new float[100, 100];

            CellsX.ChangeEvent = CellsChange;
            CellsY.ChangeEvent = CellsChange;

            AddTag(GameObjectTag.Update);
            AddTag(GameObjectTag._2DOverDraw);

            base.Create();

            Parent2DScene = (Basic2DScene)ParentScene;
            Divisor = (Parent2DScene.MaxBoundary.get() - Parent2DScene.MinBoundary.get()) / new Vector2(CellsX.get(), CellsY.get());
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            return;
            if (Drawing)
                for (int x = 0; x < CellsX.get(); x++)
                    for (int y = 0; y < CellsY.get(); y++)
                        if (CellAlpha[x, y] > 0)
                        {
                            Vector2 TransformedPosition = new Vector2(x, y) * Divisor + Parent2DScene.MinBoundary.get();

                            Vector3 Position3 = Game1.graphicsDevice.Viewport.Project(
                                   new Vector3(TransformedPosition.X, 0, TransformedPosition.Y), StarshipScene.CurrentCamera.ProjectionMatrix,
                                   StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);

                            Vector2 Position = new Vector2(Position3.X, Position3.Y) - Render.CurrentView.Position;
                            float Size = 6;

                            Render.DrawSolidRect(Position - new Vector2(Size), Position + new Vector2(Size), 
                                Color.White * CellAlpha[x, y]);

                            base.Draw2D(DrawTag);
                        }
        }

        public static void Rebuild()
        {
            self.rebuild();
            PathFindingNode.Rebuild();
        }

        public static void BuildAttackGrid()
        {
            self.buildAttackGrid();
        }

        public static void AddMineralRock(MiningPlatform r)
        {
            self.HotPoints.AddLast(new PathfindingHotPoint(r.Position.get(), r.GetTeam()));
            self.rebuild();
        }

        public static bool CollisionLine(Vector2 A, Vector2 B)
        {
            return self.collisionLine(A, B);
        }

        public static bool CollisionLine(Vector2 A, int DirectionX, int DirectionY, int Steps)
        {
            return self.collisionLine(A, DirectionX, DirectionY, Steps);
        }

        private bool collisionLine(Vector2 A, int DirectionX, int DirectionY, int Steps)
        {
            int AX = (int)((A.X - Parent2DScene.MinBoundary.X()) / Divisor.X);
            int AY = (int)((A.Y - Parent2DScene.MinBoundary.Y()) / Divisor.Y);

            AX = AX < Steps ? Steps : AX > CellsX.get() - 1 - Steps ? CellsX.get() - 1 - Steps : AX;
            AY = AY < Steps ? Steps : AY > CellsY.get() - 1 - Steps ? CellsY.get() - 1 - Steps : AY;

            for (int i = 0; i < Steps; i++)
                if (CellGrid[AX + DirectionX * Steps, AY + DirectionY * Steps] == DeadCell)
                    return true;

            return false;
        }

        private bool collisionLine(Vector2 A, Vector2 B)
        {
            int AX = (int)((A.X - Parent2DScene.MinBoundary.X()) / Divisor.X);
            int BX = (int)((B.X - Parent2DScene.MinBoundary.X()) / Divisor.X);
            int AY = (int)((A.Y - Parent2DScene.MinBoundary.Y()) / Divisor.Y);
            int BY = (int)((B.Y - Parent2DScene.MinBoundary.Y()) / Divisor.Y);

            int MinX = AX <= BX ? AX : BX;
            int MaxX = AX <= BX ? BX : AX;

            int div = (AX - BX);

            float LineSlope = (float)(AY - BY) / div;
            float LineOffset = AY - LineSlope * AX;

            if (AX != BX)
            {
                for (int x = MinX; x < MaxX; x++)
                    {
                        int y1 = (int)(LineSlope * x + LineOffset);
                        int y2 = (int)(LineSlope * (x + 1) + LineOffset + 1);

                        if (y1 == y2)
                            y1--;

                        int miny = y1 > y2 ? y1 : y2;

                        for (int y = y1 < y2 ? y1 : y2; y < miny; y++)
                            if ((x != AX || y != AY) && (x != BX || y != BY))
                                if (y > -1 && x > -1)
                                    if (CellGrid[x, y] == DeadCell)
                                        return true;
                    }
            }
            else
            {
                int MinY = AY <= BY ? AY : BY;
                int MaxY = AY <= BY ? BY : AY;

                for (int y = MinY; y < MaxY + 1; y++)
                {
                    if (CellGrid[AX, y] == DeadCell)
                        return true;
                }
            }

            return false;
        }

        private bool collisionLine(int AX, int AY, int BX, int BY)
        {
            int MinX = AX <= BX ? AX : BX;
            int MaxX = AX <= BX ? BX : AX;

            float LineSlope = (float)(AY - BY) / (AX - BX);
            float LineOffset = AY - LineSlope * AX;

            if (AX != BX)
            {
                for (int x = MinX; x < MaxX; x++)
                {
                    int y1 = (int)(LineSlope * x + LineOffset);
                    int y2 = (int)(LineSlope * (x + 1) + LineOffset + 1);

                    if (y1 == y2)
                        y1--;

                    int miny = y1 > y2 ? y1 : y2;

                    for (int y = y1 < y2 ? y1 : y2; y < miny; y++)
                        if ((x != AX || y != AY) && (x != BX || y != BY))
                            if (CellGrid[x, y] == DeadCell)
                                return true;
                }
            }
            else
            {
                int MinY = AY <= BY ? AY : BY;
                int MaxY = AY <= BY ? BY : AY;

                for (int y = MinY; y < MaxY + 1; y++)
                {
                    if (CellGrid[AX, y] == DeadCell)
                        return true;
                }
            }


            return false;
        }

        public static Vector2 TraceCellPoint(Vector2 A, int MaxJumps)
        {
            return self.traceCellPoint(A, MaxJumps);
        }

        private Vector2 traceCellPoint(Vector2 A, int MaxJumps)
        {
            int AX = (int)((A.X - Parent2DScene.MinBoundary.X()) / Divisor.X);
            int AY = (int)((A.Y - Parent2DScene.MinBoundary.Y()) / Divisor.Y);

            AX = AX < 1 ? 1 : AX > CellsX.get() - 2 ? CellsX.get() - 2 : AX;
            AY = AY < 1 ? 1 : AY > CellsY.get() - 2 ? CellsY.get() - 2 : AY; 

            int BX = AX;
            int BY = AY;
            int MaxValue;
            int PrevBX;
            int PrevBY;

            for (int i = 0; i < MaxJumps; i++)
            {
                MaxValue = Max(CellGrid[BX - 1, BY], CellGrid[BX + 1, BY], CellGrid[BX, BY - 1], CellGrid[BX, BY + 1]);

                PrevBX = BX;
                PrevBY = BY;

                if (MaxValue == CellGrid[BX - 1, BY])
                    BX--;
                if (MaxValue == CellGrid[BX + 1, BY])
                    BX++;
                if (MaxValue == CellGrid[BX, BY - 1])
                    BY--;
                if (MaxValue == CellGrid[BX, BY + 1])
                    BY++;

                BX = BX < 1 ? 1 : BX > CellsX.get() - 2 ? CellsX.get() - 2 : BX;
                BY = BY < 1 ? 1 : BY > CellsY.get() - 2 ? CellsY.get() - 2 : BY; 

                if (i != 0 && collisionLine(AX, AY, BX, BY))
                    return TransformToWorld(PrevBX, PrevBY);
            }
            return TransformToWorld(BX, BY);
        }

        public Vector2 WorldPosition(int X, int Y)
        {
            return new Vector2(X, Y) * Divisor + Parent2DScene.MinBoundary.get();
        }

        public Vector2 TransformToWorld(int X, int Y)
        {
            return new Vector2(X, Y) * Divisor + Parent2DScene.MinBoundary.get();
            /*
            int PushX = X;
            int PushY = Y;

            for (int x = X - 2; x < X + 2; x++)
                for (int y = Y - 2; y < Y + 2; y++)
                {
                    if (CellGrid[x, y] == DeadCell)
                    {
                        int OldPushX = PushX;
                        int OldPushY = PushY;

                        if (x > X)
                            PushX--;
                        else if (x < X)
                            PushX++;

                        if (y > Y)
                            PushY--;
                        else if (y < Y)
                            PushY++;

                        if (CellGrid[PushX, PushY] == DeadCell || collisionLine(X, Y, PushX, PushY))
                        {
                            PushX = OldPushX;
                            PushY = OldPushY;
                        }
                    }
                }
            X = PushX;
            Y = PushY;

            return new Vector2(X, Y) * Divisor + Parent2DScene.MinBoundary.get();
            */
        }

        int Max(int A, int B, int C, int D)
        {
            A = A > B ? A : B;
            B = C > D ? C : D;

            return A > B ? A : B;
        }

        public static int GetCellValue(Vector2 Position)
        {
            return self.getCellValue(Position);
        }

        public static int GetAttackValue(Vector2 Position)
        {
            int v = self.getAttackValue(Position);
            return v;
        }

        public int getCellValue(Vector2 Position)
        {
            if (Position.X > Parent2DScene.MinBoundary.X() && Position.Y > Parent2DScene.MinBoundary.Y() &&
                Position.X < Parent2DScene.MaxBoundary.X() && Position.Y < Parent2DScene.MaxBoundary.Y())
                return CellGrid[(int)((Position.X - Parent2DScene.MinBoundary.X()) / Divisor.X), (int)((Position.Y - Parent2DScene.MinBoundary.Y()) / Divisor.Y)];
            else
                return DeadCell;
        }

        public int getAttackValue(Vector2 Position)
        {
            if (Position.X > Parent2DScene.MinBoundary.X() && Position.Y > Parent2DScene.MinBoundary.Y() &&
                Position.X < Parent2DScene.MaxBoundary.X() && Position.Y < Parent2DScene.MaxBoundary.Y())
                return AttackGrid[(int)((Position.X - Parent2DScene.MinBoundary.X()) / Divisor.X), (int)((Position.Y - Parent2DScene.MinBoundary.Y()) / Divisor.Y)];
            else
                return DeadCell;
        }

        public static bool GetAreaClear(Vector2 Position)
        {
            return self.getAreaClear(Position);
        }

        private bool getAreaClear(Vector2 Position)
        {
            foreach (UnitBasic u in Parent2DScene.Enumerate(typeof(UnitBuilding)))
                if (Vector2.Distance(Position, u.Position.get()) < 250 &&
                    u.GetTeam() == WaveManager.ActiveTeam)
                    return false;

            foreach (PathfindingHotPoint h in HotPoints)
                if (h.Team == WaveManager.ActiveTeam && Vector2.Distance(Position, h.Position) < 350)
                    return false;

            return true;
        }

        private void commitWorldBlocker(Basic2DObject w)
        {
            Vector2 UpperLeftCorner = (w.getUpperLeftCorner() - Parent2DScene.MinBoundary.get()) / Divisor + new Vector2(0.5f);
            Vector2 LowerRightCorner = (w.getLowerRightCorner() - Parent2DScene.MinBoundary.get()) / Divisor + new Vector2(0.5f);
            Vector2 Center = (w.getLowerRightCorner() - Parent2DScene.MinBoundary.get()) / Divisor;

            int MinX = (int)UpperLeftCorner.X;
            int MinY = (int)UpperLeftCorner.Y;
            int MaxX = (int)LowerRightCorner.X;
            int MaxY = (int)LowerRightCorner.Y;

            for (int x = MinX; x < MaxX + 1; x++)
                for (int y = MinY; y < MaxY + 1; y++)
                {
                    CellGrid[x, y] = DeadCell;
                }

            if (w.GetType().Equals(typeof(WallNode)))
            {
                WallNode s = (WallNode)w;
                if (s.wallConnector != null)
                {
                    UpperLeftCorner = Logic.Min(UpperLeftCorner, (s.wallConnector.PositionNext - s.Size.get() / 2 - Parent2DScene.MinBoundary.get()) / Divisor + new Vector2(0.5f));
                    LowerRightCorner = Logic.Max(LowerRightCorner, (s.wallConnector.PositionNext + s.Size.get() / 2 - Parent2DScene.MinBoundary.get()) / Divisor + new Vector2(0.5f));

                    MinX = (int)UpperLeftCorner.X;
                    MinY = (int)UpperLeftCorner.Y;
                    MaxX = (int)LowerRightCorner.X;
                    MaxY = (int)LowerRightCorner.Y;

                    for (int x = MinX; x < MaxX + 1; x++)
                        for (int y = MinY; y < MaxY + 1; y++)
                        {
                            if (Logic.DistanceLineSegmentToPoint(s.Position.get(), s.wallConnector.PositionNext,
                                (new Vector2(x, y) - new Vector2(0.5f)) * Divisor + Parent2DScene.MinBoundary.get()) < w.Size.X())
                                CellGrid[x, y] = DeadCell;
                        }
                }
            }
        }

        public static Vector2 TraceAttackPoint(Vector2 A, int MaxJumps)
        {
            return self.traceAttackPoint(A, MaxJumps);
        }

        public Vector2 traceAttackPoint(Vector2 A, int MaxJumps)
        {
            int AX = (int)((A.X - Parent2DScene.MinBoundary.X()) / Divisor.X);
            int AY = (int)((A.Y - Parent2DScene.MinBoundary.Y()) / Divisor.Y);

            AX = AX < 1 ? 1 : AX > CellsX.get() - 2 ? CellsX.get() - 2 : AX;
            AY = AY < 1 ? 1 : AY > CellsY.get() - 2 ? CellsY.get() - 2 : AY; 

            int BX = AX;
            int BY = AY;
            int MaxValue;
            int PrevBX;
            int PrevBY;

            for (int i = 0; i < MaxJumps; i++)
            {
                MaxValue = Max(CellGrid[BX - 1, BY], CellGrid[BX + 1, BY], CellGrid[BX, BY - 1], CellGrid[BX, BY + 1]);

                PrevBX = BX;
                PrevBY = BY;

                if (MaxValue == CellGrid[BX - 1, BY])
                    BX--;
                if (MaxValue == CellGrid[BX + 1, BY])
                    BX++;
                if (MaxValue == CellGrid[BX, BY - 1])
                    BY--;
                if (MaxValue == CellGrid[BX, BY + 1])
                    BY++;

                BX = BX < 1 ? 1 : BX > CellsX.get() - 2 ? CellsX.get() - 2 : BX;
                BY = BY < 1 ? 1 : BY > CellsY.get() - 2 ? CellsY.get() - 2 : BY; 
            }
            return TransformToWorld(BX, BY);
        }

        public void buildAttackGrid()
        {
            if (AttackGrid == null)
                AttackGrid = new int[CellsX.get(), CellsY.get()];

            AttackJobQue.Clear();

            for (int x = 0; x < CellsX.get(); x++)
                for (int y = 0; y < CellsY.get(); y++)
                    AttackGrid[x, y] = NeutralCell;

            MiningPlatform forwardPlatform = PathFindingManager.TraceToMiningPlatform(NeutralManager.GetSpawnPosition(),
               WaveManager.ActiveTeam);

            foreach (MiningPlatform r in Parent2DScene.Enumerate(typeof(MiningPlatform)))
                if (!r.Dead && r.GetTeam() == WaveManager.ActiveTeam)
                {
                    Vector2 UpperLeftCorner = (r.getUpperLeftCorner() - Parent2DScene.MinBoundary.get()) / Divisor;
                    Vector2 LowerRightCorner = (r.getLowerRightCorner() - Parent2DScene.MinBoundary.get()) / Divisor;

                    int MinX = (int)UpperLeftCorner.X;
                    int MinY = (int)UpperLeftCorner.Y;
                    int MaxX = (int)LowerRightCorner.X;
                    int MaxY = (int)LowerRightCorner.Y;

                    for (int x = MinX; x < MaxX; x++)
                        for (int y = MinY; y < MaxY; y++)
                        {
                            AttackJobQue.Enqueue(x);
                            AttackJobQue.Enqueue(y);
                            AttackJobQue.Enqueue(r == forwardPlatform && WaveManager.CurrentWave > 5 ? StartingCell / 2 : StartingCell);
                        }
                }

            foreach (UnitTurret u in Parent2DScene.Enumerate(typeof(UnitTurret)))
                if (!u.Dead && u.GetTeam() == WaveManager.ActiveTeam &&
                    ((u.MyCard == null && !NeutralManager.MyPattern.CurrentCard.Type.Equals("Heavy")) ||
                    (u.MyCard != null && !u.MyCard.StrongVs.Equals(NeutralManager.MyPattern.CurrentCard.Type))))
                {
                    Vector2 UpperLeftCorner = (u.getUpperLeftCorner() - Parent2DScene.MinBoundary.get()) / Divisor;
                    Vector2 LowerRightCorner = (u.getLowerRightCorner() - Parent2DScene.MinBoundary.get()) / Divisor;

                    int MinX = (int)UpperLeftCorner.X;
                    int MinY = (int)UpperLeftCorner.Y;
                    int MaxX = (int)LowerRightCorner.X;
                    int MaxY = (int)LowerRightCorner.Y;

                    for (int x = MinX; x < MaxX; x++)
                        for (int y = MinY; y < MaxY; y++)
                        {
                            AttackJobQue.Enqueue(x);
                            AttackJobQue.Enqueue(y);
                            AttackJobQue.Enqueue(StartingCell);
                        }
                }
        }

        public void rebuild()
        {
            CellJobQue.Clear();

            for (int x = 0; x < CellsX.get(); x++)
                for (int y = 0; y < CellsY.get(); y++)
                    CellGrid[x, y] = NeutralCell;

            foreach (GameObject o in ParentScene.Children)
            {
                if (o.GetType().Equals(typeof(MineralRock)))
                {
                    MineralRock s = (MineralRock)o;
                    //if (s.miningPlatform == null)
                      //  commitWorldBlocker(s);
                }
                else if (o.GetType().IsSubclassOf(typeof(SolidStaticWorldObject)))
                {
                    SolidStaticWorldObject s = (SolidStaticWorldObject)o;
                    commitWorldBlocker(s);
                }
                else if (o.GetType().Equals(typeof(WallNode)))
                {
                    WallNode s = (WallNode)o;
                    commitWorldBlocker(s);
                }

            }

            foreach (MiningPlatform r in Parent2DScene.Enumerate(typeof(MiningPlatform)))
                if (!r.Dead)
                    addMineralRock(r);
        }

        private void addMineralRock(MiningPlatform r)
        {
            if (r.GetTeam() == WaveManager.ActiveTeam)
            {
                Vector2 UpperLeftCorner = (r.getUpperLeftCorner() - Parent2DScene.MinBoundary.get()) / Divisor;
                Vector2 LowerRightCorner = (r.getLowerRightCorner() - Parent2DScene.MinBoundary.get()) / Divisor;

                int MinX = (int)UpperLeftCorner.X;
                int MinY = (int)UpperLeftCorner.Y;
                int MaxX = (int)LowerRightCorner.X;
                int MaxY = (int)LowerRightCorner.Y;

                for (int x = MinX; x < MaxX; x++)
                    for (int y = MinY; y < MaxY; y++)
                    {
                        CellJobQue.Enqueue(x);
                        CellJobQue.Enqueue(y);
                        CellJobQue.Enqueue(StartingCell);
                    }
            }
        }

        public override void Update(GameTime gameTime)
        {
            int JobCount = CellJobQue.Count;

            while (JobCount > 0)
            {
                Drawing = true;

                int x = CellJobQue.Dequeue();
                int y = CellJobQue.Dequeue();
                int value = CellJobQue.Dequeue();

                if (x > -1 && y > -1 && x < CellsX.get() && y < CellsY.get() && CellGrid[x, y] < value && CellGrid[x, y] != DeadCell)
                {
                    CellGrid[x, y] = value;
                    CellAlpha[x, y] = 1;

                    MakeJob(x - 1, y, value - 1);
                    MakeJob(x + 1, y, value - 1);
                    MakeJob(x, y - 1, value - 1);
                    MakeJob(x, y + 1, value - 1);
                }
                JobCount -= 3;
            }

            JobCount = AttackJobQue.Count;

            while (JobCount > 0)
            {
                Drawing = true;

                int x = AttackJobQue.Dequeue();
                int y = AttackJobQue.Dequeue();
                int value = AttackJobQue.Dequeue();

                if (x > -1 && y > -1 && x < CellsX.get() && y < CellsY.get() && AttackGrid[x, y] < value)
                {
                    AttackGrid[x, y] = value;
                    int NewValue = value - 1;

                    foreach (UnitBasic u in FactionManager.SortedUnits[WaveManager.ActiveTeam])
                        if (u.GetType().IsSubclassOf(typeof(UnitTurret)))
                        {
                            UnitTurret t = (UnitTurret)u;
                            if (t.MyCard != null)
                            {
                                if ((!t.MyCard.StrongVs.Equals("Light") && !t.MyCard.StrongVs.Equals("")) &&
                                    Vector2.Distance(TransformToWorld(x, y), t.Position.get()) < t.MaxEngagementDistance)
                                    NewValue -= 10;
                            }
                            else if (Vector2.Distance(TransformToWorld(x, y), t.Position.get()) < t.MaxEngagementDistance)
                                NewValue -= 10;
                        }

                    MakeAttackJob(x - 1, y, NewValue);
                    MakeAttackJob(x + 1, y, NewValue);
                    MakeAttackJob(x, y - 1, NewValue);
                    MakeAttackJob(x, y + 1, NewValue);
                }
                JobCount -= 3;
            }

            if (Drawing)
            {
                Drawing = false;
                for (int x = 0; x < CellsX.get(); x++)
                    for (int y = 0; y < CellsY.get(); y++)
                        if (CellAlpha[x, y] > 0)
                        {
                            CellAlpha[x, y] -= gameTime.ElapsedGameTime.Milliseconds * 60 / 100000f;
                            Drawing = true;
                        }
            }

            base.Update(gameTime);
        }

        private void MakeJob(int x, int y, int value)
        {
            CellJobQue.Enqueue(x);
            CellJobQue.Enqueue(y);
            CellJobQue.Enqueue(value);
        }

        private void MakeAttackJob(int x, int y, int value)
        {
            AttackJobQue.Enqueue(x);
            AttackJobQue.Enqueue(y);
            AttackJobQue.Enqueue(value);
        }

        private void CellsChange()
        {
            CellGrid = new int[CellsX.get(), CellsY.get()];
            CellAlpha = new float[CellsX.get(), CellsY.get()];
            Divisor = (Parent2DScene.MaxBoundary.get() - Parent2DScene.MinBoundary.get()) / new Vector2(CellsX.get(), CellsY.get());
        }

        public MiningPlatform traceToMiningPlatform(Vector2 Position, int Team)
        {
            int AX = (int)((Position.X - Parent2DScene.MinBoundary.X()) / Divisor.X);
            int AY = (int)((Position.Y - Parent2DScene.MinBoundary.Y()) / Divisor.Y);

            AX = AX < 1 ? 1 : AX > CellsX.get() - 2 ? CellsX.get() - 2 : AX;
            AY = AY < 1 ? 1 : AY > CellsY.get() - 2 ? CellsY.get() - 2 : AY;

            int MaxValue;

            int reps = 0;

            while (CellGrid[AX, AY] != DeadCell && CellGrid[AX, AY] != StartingCell)
            {
                MaxValue = Max(CellGrid[AX - 1, AY], CellGrid[AX + 1, AY], CellGrid[AX, AY - 1], CellGrid[AX, AY + 1]);

                if (MaxValue == CellGrid[AX - 1, AY])
                    AX--;
                if (MaxValue == CellGrid[AX + 1, AY])
                    AX++;
                if (MaxValue == CellGrid[AX, AY - 1])
                    AY--;
                if (MaxValue == CellGrid[AX, AY + 1])
                    AY++;

                AX = AX < 1 ? 1 : AX > CellsX.get() - 2 ? CellsX.get() - 2 : AX;
                AY = AY < 1 ? 1 : AY > CellsY.get() - 2 ? CellsY.get() - 2 : AY;
                if (reps++ > 1000)
                    return null;
            }

            Vector2 ProjectedPosition = TransformToWorld(AX, AY);
            float BestDistance = 10000;
            MiningPlatform Result = null;

            foreach (UnitBasic u in FactionManager.SortedUnits[Team])
                if (u.GetType().IsSubclassOf(typeof(MiningPlatform)))
                {
                    float d = Vector2.Distance(u.Position.get(), ProjectedPosition);
                    if (d < BestDistance)
                    {
                        BestDistance = d;
                        Result = (MiningPlatform)u;
                    }
                }

            return Result;
        }

        public static MiningPlatform TraceToMiningPlatform(Vector2 Position, int Team)
        {
            return self.traceToMiningPlatform(Position, Team);
        }
    }
}
