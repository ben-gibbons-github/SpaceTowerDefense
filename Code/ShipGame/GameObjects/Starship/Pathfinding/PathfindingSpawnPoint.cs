using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PathfindingHotPoint
    {
        public Vector2 Position;
        public int Team;

        public PathfindingHotPoint(Vector2 Position, int Team)
        {
            this.Position = Position;
            this.Team = Team;
        }
    }
}
