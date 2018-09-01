using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public interface HierarchyParent
    {
        void AddHierarchyObject(GameObject NewObject);
        void RemoveHierarchyObject(GameObject Remove);

        void ModifyCollection();
        LinkedList<GameObject> GetChildren();
    }
}
