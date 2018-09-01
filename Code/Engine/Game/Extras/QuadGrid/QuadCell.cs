using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace BadRabbit.Carrot
{
    public class QuadCell
    {
        public Basic2DObject[] Children;
        public int ChildCount = 0;
        public int ArraySize = 16;

        public QuadCell()
        {
            Children = new Basic2DObject[ArraySize];
        }

        public Basic2DObject CheckCollision(Basic2DObject Tester)
        {
            foreach (Basic2DObject g in Children)
                if (g.CheckCollision(Tester))
                    return g;
            return null;
        }

        public void Add(Basic2DObject g)
        {
            if (ChildCount >= ArraySize - 1)
            {
                ArraySize = ArraySize * 2;
                Basic2DObject[] NewChildren = new Basic2DObject[ArraySize];
                for (int i = 0; i < ChildCount; i++)
                    NewChildren[i] = Children[i];
                Children = NewChildren;
            }

            Children[ChildCount++] = g;
        }

        public void Reset()
        {
            if (ChildCount > ArraySize / 2)
            {
                ArraySize = ArraySize * 2;
                Children = new Basic2DObject[ArraySize];
            }

            ChildCount = 0;
        }
    }
}
