using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlayerMenuManager
    {
        public TechTreeMenu TechTree;
        private PlayerMenu CurrentMenu;

        public PlayerMenuManager(PlayerShip ParentShip)
        {
            TechTree = new TechTreeMenu(ParentShip);
        }

        public void SetMenu(PlayerMenu m, Vector2 Position)
        {
            CurrentMenu = m;
            if (m != null)
                m.SetMenu(Position);
        }

        public bool Update(GameTime gameTime, BasicController MyController)
        {
            if (CurrentMenu != null)
            {
                CurrentMenu.Update(gameTime, MyController);
                return true;
            }
            else
                return false;
        }

        public void Draw()
        {
            if (CurrentMenu != null)
                CurrentMenu.Draw();
        }

        public void Draw3D(Camera3D DrawCamera)
        {
            if (CurrentMenu != null)
                CurrentMenu.Draw3D(DrawCamera);
        }
    }
}
