using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PathfindingFlare : GameObject
    {
        Color LineColor;

        PathFindingManager Parent;
        float Interpolation;

        Vector3 To;
        Vector3 From;

        int CellX;
        int CellY;

        public PathfindingFlare(PathFindingManager Parent, int CellX, int CellY, int CellXOffset, int CellYOffset, Color LineColor)
        {
            this.Parent = Parent;
            this.CellX = CellX;
            this.CellY = CellY;
            this.LineColor = LineColor;

            Vector2 Position2 = Parent.WorldPosition(CellX, CellY);
            From = new Vector3(Position2.X, 0, Position2.Y);
            Position2 = Parent.WorldPosition(CellXOffset + CellX, CellYOffset + CellY);
            To = new Vector3(Position2.X, 0, Position2.Y);
        }

        public override void Create()
        {
            AddTag(GameObjectTag.Update);
            base.Create();
        }

        int Max(int A, int B, int C, int D)
        {
            A = A > B ? A : B;
            B = C > D ? C : D;

            return A > B ? A : B;
        }

        public override void Update(GameTime gameTime)
        {
            Interpolation += gameTime.ElapsedGameTime.Milliseconds / (4f * Math.Max(1, Vector3.Distance(To, From)));
            while (Interpolation > 1)
            {
                Interpolation -= 1;
                //LineParticleSystem.AddParticle(To, From, LineColor);
                From = To;

                if (CellX < 1 || CellY < 1 ||
                    CellX > Parent.CellsX.get() - 2 || CellY > Parent.CellsY.get() - 2 ||
                    Parent.CellGrid[CellX, CellY] == PathFindingManager.StartingCell)
                {
                    Destroy();
                    return;
                }

                for (int i = 0; i < 5; i++)
                {
                    int MaxValue = Max(Parent.CellGrid[CellX - 1, CellY], Parent.CellGrid[CellX + 1, CellY],
                        Parent.CellGrid[CellX, CellY - 1], Parent.CellGrid[CellX, CellY + 1]);

                    if (MaxValue == Parent.CellGrid[CellX - 1, CellY])
                        CellX--;
                    if (MaxValue == Parent.CellGrid[CellX + 1, CellY])
                        CellX++;
                    if (MaxValue == Parent.CellGrid[CellX, CellY - 1])
                        CellY--;
                    if (MaxValue == Parent.CellGrid[CellX, CellY + 1])
                        CellY++;

                    if (MaxValue == PathFindingManager.DeadCell || CellX < 1 || CellY < 1 ||
                        CellX > Parent.CellsX.get() - 2 || CellY > Parent.CellsY.get() - 2)
                    {
                        Destroy();
                        return;
                    }
                }

                Vector2 Position2 = Parent.WorldPosition(CellX, CellY);
                To = new Vector3(Position2.X - 50 + Rand.F() * 100, 0, Position2.Y - 50 + Rand.F() * 100);
            }

            Vector3 InterpolatedPosition = Vector3.Lerp(From, To, Interpolation);

            ParticleManager.CreateParticle(InterpolatedPosition, Vector3.Zero, LineColor, 50, 1);
            ParticleManager.CreateParticle(InterpolatedPosition, Vector3.Zero, LineColor * 0.1f, 250, 1);
            
            base.Update(gameTime);
        }
    }
}
