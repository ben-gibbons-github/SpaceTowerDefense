using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ShipAbility
    {
        public int MaxRechargeTime = 0;
        public int RechargeTime = 0;
        protected PlayerShip ParentShip;
        public Texture2D Icon;

        public void Create(PlayerShip ParentShip)
        {
            this.ParentShip = ParentShip;
        }

        public virtual bool Trigger()
        {
            return false;
        }

        public virtual void Update(GameTime gameTime, BasicController MyController)
        {

        }

        public virtual bool ShipIsSolid()
        {
            return true;
        }

        public virtual bool ShipCanMove()
        {
            return true;
        }

        public virtual bool ShipCanShoot()
        {
            return true;
        }

        public virtual bool ShipCanTakeDamage()
        {
            return true;
        }

        public virtual bool ShipVisible()
        {
            return true;
        }

        public virtual float GetCharge()
        {
            return 0;
        }

        public virtual void Draw()
        {

        }
    }
}
