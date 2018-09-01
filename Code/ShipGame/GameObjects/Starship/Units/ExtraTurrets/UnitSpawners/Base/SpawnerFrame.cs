using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SpawnerFrame : BasicShipGameObject
    {
        public SpawnerFrame(Vector2Value Size, Vector2Value Position, FloatValue Rotation)
            : base(-1)
        {
            this.Size = Size;
            this.Position = Position;
            this.Rotation = Rotation;
            InstanceManager.AddBasicChild(this);
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 5;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("ExtraTurrets/Turret6");
        }
    }
}
