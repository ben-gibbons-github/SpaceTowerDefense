using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class LightningPoint
    {
        public const int LifeTime = 1000;

        public int StartTime = 0;
        Vector3 Position;
        Vector3 MinColor;
        Vector3 MaxColor;
        float Size;
        float Spread;
        int Lines;
        int LinePop;

        public void Create(Vector3 Position, Vector3 MinColor, Vector3 MaxColor,
            float Size, float Spread, int Lines, int LinePop)
        {
            this.Position = Position;
            this.MinColor = MinColor;
            this.MaxColor = MaxColor;
            this.Size = Size;
            this.Spread = Spread;
            this.Lines = Lines;
            this.LinePop = LinePop;
        }

        public void Update(int Timer)
        {
            FlareSystem.AddLightning(Position,
                new Color(Logic.RLerp(MinColor, MaxColor) * (1 - (Timer - StartTime) / (float)LifeTime)),
                Size, Spread, Lines, LinePop);
        }
    }
}
