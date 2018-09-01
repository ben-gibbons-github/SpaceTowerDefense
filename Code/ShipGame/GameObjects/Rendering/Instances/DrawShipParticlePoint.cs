using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class DrawShipParticlePoint
    {
        int Timer = 0;
        public bool CanProduce = false;
        public bool CinematicOnly = false;

        public Vector3 Position;
        public int Layer;
        public int CinematicDelay;
        public int GameDelay;
        public int ParticleType;
        public Vector3 MinVelocity;
        public Vector3 MaxVelocity;
        public Vector4 MinColor;
        public Vector4 MaxColor;
        public float MinSize;
        public float MaxSize;

        public void AddTime(GameTime gameTime, bool Cinematic)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            if (Cinematic)
                CanProduce = Timer > CinematicDelay;
            else
                CanProduce = Timer > GameDelay;
            if (CanProduce)
                Timer = 0;
        }

        public void ProduceParticle(ref Vector3 Position, ref Matrix Rotation, float Scale, float ColorMult)
        {
            if (CanProduce)
            {
                ParticleManager.CreateParticle(Position + Vector3.Transform(this.Position * Scale / 100, Rotation)
                    , Vector3.Transform(Logic.RLerp(MinVelocity, MaxVelocity) * Scale / 100, Rotation), new Color(Logic.RLerp(MinColor, MaxColor) * ColorMult / 3), MathHelper.Lerp(MinSize, MaxSize, Rand.F()) * Scale / 100, ParticleType);
            }
        }
        
    }
}
