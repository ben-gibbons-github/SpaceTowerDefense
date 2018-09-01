using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class UIParticleBasic
    {
        bool FadingIn = true;
        PlayerUIManager Parent;

        protected float Alpha = 0;
        protected PlayerShip ParentShip;
        protected float AlphaFadeIn = 0.1f;
        protected float AlphaFadeOut = 0.01f;

        public void Create(PlayerUIManager Parent, PlayerShip ParentShip)
        {
            this.Parent = Parent;
            this.ParentShip = ParentShip;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (FadingIn)
            {
                Alpha += AlphaFadeIn * gameTime.ElapsedGameTime.Milliseconds / 1000f * 60f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    FadingIn = false;
                }
            }
            else
            {
                Alpha -= AlphaFadeOut * gameTime.ElapsedGameTime.Milliseconds / 1000f * 60f;
                if (Alpha < 0)
                    Destroy();
            }
        }

        public void Destroy()
        {
            Parent.RemoveParticle(this);
        }

        public virtual void Draw()
        {

        }
    }
}
