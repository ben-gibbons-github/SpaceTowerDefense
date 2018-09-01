using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class DrawWall
    {
        public DrawShip drawShip;
        public string Fname;
        public LinkedList<WallItem> Children;

        public DrawWall(string Fname)
        {
            Children = new LinkedList<WallItem>();
            drawShip = new DrawShip(Fname);
            this.Fname = Fname;
        }

        public void AddChild(WallItem Child)
        {
            Children.AddLast(Child);
        }

    }
}
