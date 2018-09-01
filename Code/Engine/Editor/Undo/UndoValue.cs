using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class UndoValue : UndoAction
    {
        LinkedList<Value> ReferenceValues;

        public UndoValue(LinkedList<Value> ReferenceValeus)
        {
            this.ReferenceValues = ReferenceValeus;
        }
    }
}
