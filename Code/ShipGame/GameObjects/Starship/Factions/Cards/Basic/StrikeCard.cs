using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class StrikeCard : FactionCard
    {
        Vector2 StrikePosition;

        int StrikeTimer;
        int MaxStrikeTimer = 1000;

        public virtual bool CardPick()
        {
            return false; // returns true if no strike
        }

        public virtual void Trigger(Vector2 Position)
        {
            StrikePosition = Position;
        }

        public virtual bool UpdateStrike(GameTime gameTime)
        {
            StrikeTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (StrikeTimer > MaxStrikeTimer)
            {
                Strike(StrikePosition);
                return true;
            }

            return false;
        }

        protected virtual void Strike(Vector2 Position)
        {

        }
    }
}
