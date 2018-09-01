using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class CrystalWallConnection
    {
        public float LineIntercept;
        public float LineSlope;
        public bool LineIsVertical;
        public CrystalWall wall;

        public CrystalWallConnection(CrystalWall wall, CrystalWall wall2)
        {
            this.wall = wall;

            if (wall.Position.X() == wall2.Position.X())
            {
                LineSlope = 0;
                LineIsVertical = true;
            }
            else
                LineSlope = (wall.Position.Y() - wall2.Position.Y()) / (wall.Position.X() - wall2.Position.X());

            LineIntercept = wall.Position.Y() - LineSlope * wall.Position.X();
        }
    }
}
