using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    class Formation
    {
        private UnitBasic[] Children;
        private int ChildCount = 0;
        private int ArraySize = 0;

        public void Commit(UnitBasic[] Children, int ChildCount)
        {
            if (ChildCount > this.ArraySize)
            {
                this.Children = Children;
                this.ChildCount = ChildCount;
                this.ArraySize = ChildCount;
            }
            else
            {
                this.ChildCount = ChildCount;
                for (int i = 0; i < ChildCount; i++)
                    this.Children[i] = Children[i];
            }
        }
    }
}
