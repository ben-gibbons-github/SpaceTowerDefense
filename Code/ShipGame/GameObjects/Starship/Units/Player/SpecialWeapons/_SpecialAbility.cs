using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SpecialWeapon
    {
        public int MaxRechargeTime = 0;
        public int RechargeTime;

        protected PlayerShip ParentShip;

        public void Reset()
        {
            RechargeTime = 0;
        }

        public virtual void Arm()
        {

        }

        public virtual float GetProgress()
        {
            return 0;
        }

        public virtual void Create(PlayerShip ParentShip)
        {
            this.ParentShip = ParentShip;
        }

        public virtual void Update(GameTime gameTime)
        {
            RechargeTime += gameTime.ElapsedGameTime.Milliseconds;
        }

        public virtual bool ShipCanMove()
        {
            return true;
        }

        public virtual bool ShipIsVisible()
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

        public virtual void Trigger()
        {

        }

        public virtual void Draw()
        {

        }
    }
}
