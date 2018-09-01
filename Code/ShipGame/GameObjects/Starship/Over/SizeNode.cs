using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SizeNode
    {
        public Vector2 Position;
        public Vector2 TargetPosition;
        float MoveSpeed = 0.05f;

        public SizeNode()
        {

        }

        public SizeNode(Vector2 Position)
        {
            this.Position = Position;
            TargetPosition = Position;
        }

        public void Update(GameTime gameTime)
        {
            Position += (TargetPosition - Position) * MoveSpeed * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
        }
    }
}
