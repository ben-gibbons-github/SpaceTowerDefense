using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace BadRabbit.Carrot
{
    public class QuadGrid : IEnumerable
    {
#if EDITOR && WINDOWS
        public bool UseQuadGrids = true;
#endif

        public QuadCell[,] Children;
        public Vector2 Min;
        public Vector2 Max;
        public Vector2 CellSize;
        public int CellsX;
        public int CellsY;

        private bool RingMode = false;
        private int MinX, MinY, MaxX, MaxY;

        public void SetDimensions(Vector2 Min, Vector2 Max, Vector2 Cells)
        {
            this.Min = Min;
            this.Max = Max;
            CellsX = (int)Cells.X;
            CellsY = (int)Cells.Y;

            if (CellsX == 0 || CellsY == 0)
                return;

            CellSize = (Max - Min) / new Vector2(CellsX, CellsY);
            Children = new QuadCell[CellsX, CellsY];

            for (int x = 0; x < CellsX; x++) for (int y = 0; y < CellsY; y++)
                    Children[x, y] = new QuadCell();
        }

        public void SetObjects(LinkedList<GameObject> Objects)
        {
            for (int x = 0; x < CellsX; x++) for (int y = 0; y < CellsY; y++)
                {
                    if (Children[x, y].ChildCount > Children[x, y].ArraySize / 2)
                    {
                        Children[x, y].ArraySize = Children[x, y].ArraySize * 2;
                        Children[x, y].Children = new Basic2DObject[Children[x, y].ArraySize];
                    }

                    Children[x, y].ChildCount = 0;
                }

            foreach (Basic2DObject g in Objects)
                for (int x = g.QuadGridXMin; x <= g.QuadGridXMax; x++)
                    for (int y = g.QuadGridYMin; y <= g.QuadGridYMax; y++)
                    {
                        if (Children[x, y].ChildCount >= Children[x, y].ArraySize - 1)
                        {
                            Children[x, y].ArraySize = Children[x, y].ArraySize * 2;
                            Basic2DObject[] NewChildren = new Basic2DObject[Children[x, y].ArraySize];
                            for (int i = 0; i < Children[x, y].ChildCount; i++)
                                NewChildren[i] = Children[x, y].Children[i];
                            Children[x, y].Children = NewChildren;
                        }

                        Children[x, y].Children[Children[x, y].ChildCount++] = g;
                    }
        }

        public Basic2DObject CheckCollision(Basic2DObject Tester)
        {
            Vector2 MinPosition = (Tester.getUpperLeftCorner() - Min) / CellSize;
            Vector2 MaxPosition = (Tester.getLowerRightCorner() - Min) / CellSize;

            int MinCellX = (int)MathHelper.Clamp(MinPosition.X, 0, CellsX);
            int MinCellY = (int)MathHelper.Clamp(MinPosition.Y, 0, CellsY);
            int MaxCellX = (int)MathHelper.Clamp(MaxPosition.X, 0, CellsX) + 1;
            int MaxCellY = (int)MathHelper.Clamp(MaxPosition.Y, 0, CellsY) + 1;

            for (int x = MinCellX; x < MaxCellX; x++)
                for (int y = MinCellY; y < MaxCellY; y++)
                {
                    Basic2DObject g = null;
                    g = Children[x, y].CheckCollision(Tester);

                    if (g != null)
                        return g;
                }

            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public QuadGrid Enumerate()
        {
            RingMode = false;

            MinX = 0;
            MinY = 0;
            MaxX = CellsX - 1;
            MaxY = CellsY - 1;

            return this;
        }


        public QuadGrid Enumerate(Vector2 Position, Vector2 Size)
        {
            /*
            if (float.IsNaN(Position.X))
            {
                Console.WriteLine("QuadGrid NAN Error");
                return this;
            }
            */

            RingMode = false;
            Vector2 UpperLeftCorner = Position - Size / 2;
            Vector2 LowerRightCorner = Position + Size / 2;
            /*
            this.MinX = (int)MathHelper.Clamp((UpperLeftCorner.X - Min.X) / CellSize.X, 0, CellsX - 1);
            this.MaxX = (int)MathHelper.Clamp((LowerRightCorner.X - Min.X) / CellSize.X, 0, CellsX - 1);
            this.MinY = (int)MathHelper.Clamp((UpperLeftCorner.Y - Min.Y) / CellSize.Y, 0, CellsY - 1);
            this.MaxY = (int)MathHelper.Clamp((LowerRightCorner.Y - Min.Y) / CellSize.Y, 0, CellsY - 1);
            */


            this.MinX = (int)((UpperLeftCorner.X - Min.X) / CellSize.X);
            this.MaxX = (int)((LowerRightCorner.X - Min.X) / CellSize.X);
            this.MinY = (int)((UpperLeftCorner.Y - Min.Y) / CellSize.Y);
            this.MaxY = (int)((LowerRightCorner.Y - Min.Y) / CellSize.Y);

            this.MinX = MinX > CellsX - 1 ? CellsX - 1 : MinX > 0 ? MinX : 0;
            this.MaxX = MaxX > CellsX - 1 ? CellsX - 1 : MaxX > 0 ? MaxX : 0;
            this.MinY = MinY > CellsY - 1 ? CellsY - 1 : MinY > 0 ? MinY : 0;
            this.MaxY = MaxY > CellsY - 1 ? CellsY - 1 : MaxY > 0 ? MaxY : 0;

            return this;
        }

        public QuadGrid Enumerate(int MinX, int MinY, int MaxX, int MaxY)
        {
            RingMode = false;

            this.MinX = MinX;
            this.MaxX = MaxX;
            this.MinY = MinY;
            this.MaxY = MaxY;
            /*
            this.MinX = MinX > CellsX - 1 ? CellsX - 1 : MinX > 0 ? MinX : 0;
            this.MaxX = MaxX > CellsX - 1 ? CellsX - 1 : MaxX > 0 ? MaxX : 0;
            this.MinY = MinY > CellsY - 1 ? CellsY - 1 : MinY > 0 ? MinY : 0;
            this.MaxY = MaxY > CellsY - 1 ? CellsY - 1 : MaxY > 0 ? MaxY : 0;
            
            this.MinX = (int)MathHelper.Clamp(MinX, 0, CellsX - 1);
            this.MaxX = (int)MathHelper.Clamp(MaxX, 0, CellsX - 1);
            this.MinY = (int)MathHelper.Clamp(MinY, 0, CellsY - 1);
            this.MaxY = (int)MathHelper.Clamp(MaxY, 0, CellsY - 1);*/
            return this;
        }

        public QuadGrid EnumerateRing(int MinX, int MinY, int MaxX, int MaxY)
        {
            RingMode = true;
            this.MinX = MinX;
            this.MinY = MinY;
            this.MaxX = MaxX;
            this.MaxY = MaxY;
            return this;
        }

        public QuadEnum GetEnumerator()
        {
            if (!RingMode)
                return new QuadEnum(Children, MinX, MinY, MaxX, MaxY);
            else
                return new QuadRingEnum(Children, MinX, MinY, MaxX, MaxY);
        }

        public class QuadEnum : IEnumerator
        {
            public QuadCell[,] Cells;
            public int X, MaxX, Y, MaxY;
            public int position = -1;

            public QuadEnum(QuadCell[,] Cells,int MinX,int MinY, int MaxX, int MaxY)
            {
                this.X = MinX;
                this.Y = MinY;
                this.MaxX = MaxX;
                this.MaxY = MaxY;
                this.Cells = Cells;
            }

            public virtual bool MoveNext()
            {
                position++;
                while(position >= Cells[X,Y].ChildCount)
                {
                    position = 0;
                    if (X < MaxX )
                        X++;
                    else if (Y < MaxY)
                    {
                        Y++;
                        X = 0;
                    }
                    else
                        return false;
                }
                return true;
            }

            public void Reset()
            {
                position = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public Basic2DObject Current
            {
                get
                {
                    try
                    { 
                        return Cells[X, Y].Children[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public class QuadRingEnum : QuadEnum
        {
            int MinX, MinY;

            public QuadRingEnum(QuadCell[,] Cells, int MinX, int MinY, int MaxX, int MaxY)
                : base(Cells,MinX,MinY,MaxX,MaxY)
            {
                this.MinX = MinX;
                this.MinY = MinY;
            }

            public override bool MoveNext()
            {
                position++;
                if (position >= Cells[X, Y].ChildCount)
                {
                    if (X < MaxX - 1)
                    {
                        if (Y == MinY || Y == MinY)
                            X++;
                        else
                            X = MaxX - 1;
                    }
                    else if (Y < MaxY - 1)
                    {
                        Y++;
                        X = 0;
                    }
                    else
                        return false;
                }
                return true;
            }
        }
    }
}
