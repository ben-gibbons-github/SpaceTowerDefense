using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BarTeam
    {
        public int Team;
        public float ListPosition;
        public float ListTargetPosition;
        public float X;
        public bool SideLined;
        public float PositionChangeSpeed = 0.05f;

        public BarTeam(int Team, float ListPosition)
        {
            this.Team = Team;
            this.ListPosition = ListPosition;
            this.ListTargetPosition = ListPosition;
        }

        public void Update(GameTime gameTime)
        {
            if (!SideLined || 1 - X < PositionChangeSpeed)
            {
                if (SideLined)
                    X = 1;

                if (Math.Abs(ListPosition - ListTargetPosition) < PositionChangeSpeed)
                {
                    SideLined = false;
                    ListPosition = ListTargetPosition;
                    if (X > PositionChangeSpeed)
                        X -= PositionChangeSpeed * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000;
                    else
                        X = 0;
                }
                else
                {
                    if (ListPosition > ListTargetPosition)
                        ListPosition -= PositionChangeSpeed * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000;
                    else
                        ListPosition += PositionChangeSpeed * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000;
                }
            }
            else
            {
                X += PositionChangeSpeed * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000;
            }
            //ListPosition += (ListTargetPosition - ListPosition) * PositionChangeSpeed * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
        }
    }
}
