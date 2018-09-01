using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class AbilityTarget
    {
        Vector2 TargetPosition;
        BasicShipGameObject TargetObject;

        public Vector2 GetPosition()
        {
            if (TargetObject == null)
                return TargetPosition;
            else
                return TargetObject.getPosition();
        }

        public void Set(Vector2 Pos)
        {
            TargetPosition = Pos;
            TargetObject = null;
        }

        public void Set(BasicShipGameObject Obj)
        {
            TargetPosition = Vector2.Zero;
            TargetObject = Obj;
        }
    }
}
