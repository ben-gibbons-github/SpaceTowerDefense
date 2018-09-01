using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class WallInstancer
    {
        static LinkedList<DrawWall> Walls = new LinkedList<DrawWall>();

        public static void AddChild(WallItem Child)
        {
            foreach (DrawWall wall in Walls)
                if (Child.GetFname() == wall.Fname)
                {
                    wall.AddChild(Child);
                    return;
                }

            DrawWall wall2 = new DrawWall(Child.GetFname());
            Walls.AddLast(wall2);
            wall2.AddChild(Child);
        }

        public static void RemoveChild(WallItem Child)
        {
            foreach (DrawWall wall in Walls)
                if (Child.GetFname() == wall.Fname)
                {
                    wall.Children.Remove(Child);
                    return;
                }
        }

        public static void Clear()
        {
            Walls.Clear();
        }

        public static void Draw(Camera3D DrawCamera)
        {
            foreach (DrawWall w in Walls)
            {
                w.drawShip.DrawInstanced(w.Children, DrawCamera);
            }
        }
    }
}
