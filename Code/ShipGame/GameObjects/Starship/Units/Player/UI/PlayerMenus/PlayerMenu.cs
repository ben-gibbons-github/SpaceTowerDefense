using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerMenu
    {
        public static float CloseOpenSpeed = 0.005f;

        public PlayerShip ParentShip;
        public Color color;
        public bool Closing;
        public float Alpha = 0;
        public Vector2 Position;

        public PlayerMenu(PlayerShip ParentShip)
        {
            this.ParentShip = ParentShip;
        }

        public virtual void SetMenu(Vector2 Position)
        {
            this.Position = Position;
            Closing = false;
            Alpha = 0;
        }

        public virtual void Update(GameTime gameTime, BasicController MyController)
        {
        }

        public virtual void Draw()
        {

        }

        public virtual void Draw3D(Camera3D DrawCamera)
        {

        }
    }
}
