using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class UndoManager
    {
        private static LinkedList<UndoAction> UndoActions = new LinkedList<UndoAction>();
        private static LinkedList<UndoAction> RedoActions = new LinkedList<UndoAction>();


        public static void AddUndoAction(UndoAction Action)
        {
            UndoActions.AddFirst(Action);
            RedoActions.Clear();
        }

        public static void Undo()
        {
            UndoActions.First.Value.Perform();
            RedoActions.AddFirst(UndoActions.First);
            UndoActions.RemoveFirst();
        }
    }
}
