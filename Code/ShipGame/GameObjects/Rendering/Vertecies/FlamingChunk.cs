using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class FlamingChunk
    {
        const float ModelSize = 200;
        public const int LifeTime = 500;

        public int StartTime;
        public Matrix WorldMatrix;

        public Vector3 GlowPosition;
        Vector3 StartPosition;
        Vector3 StartVelocity;
        Vector3 StartingRotation;
        Vector3 RotationSpeed;
        Vector3 Gravity;
        public Vector3 MinColor;
        public Vector3 MaxColor;
        public float Size;
        public float GlowSizeMult;
        float ParticleSizeMult;
        int ParticleType;
        

        public FlamingChunk()
        {

        }

        public void Create(int StartTime, Vector3 StartPosition, Vector3 StartVelocity,
            Vector3 Gravity, Vector3 StartingRotation, Vector3 RotationSpeed, float Size,
            float GlowSizeMult, Vector3 MinColor, Vector3 MaxColor, int ParticleType,
            float ParticleSizeMult)
        {
            this.Size = Size;
            this.StartPosition = StartPosition;
            this.StartTime = StartTime;
            this.StartingRotation = StartingRotation;
            this.RotationSpeed = RotationSpeed;
            this.StartVelocity = StartVelocity;
            this.Gravity = Gravity;
            this.GlowSizeMult = GlowSizeMult;
            this.MinColor = MinColor;
            this.MaxColor = MaxColor;
            this.ParticleType = ParticleType;
            this.ParticleSizeMult = ParticleSizeMult;
        }

        private Vector3 GetPosition(int Timer)
        {
            int Age = Timer - StartTime;
            return StartPosition + StartVelocity / 3f * Age + Gravity * Age * Age / (float)LifeTime;
        }

        public void Update(int Timer, int PreviousTimer)
        {
            if (Timer < StartTime + LifeTime)
            {
                for (int i = PreviousTimer; i < Timer - Timer % 50; i += 50)
                    ParticleManager.CreateParticle(GetPosition(i), Vector3.Zero, new Color(Logic.RLerp(MinColor, MaxColor)), Size * ParticleSizeMult * (1 - (Timer - StartTime) / (float)LifeTime), ParticleType);

                Vector3 NewRotation = StartingRotation + RotationSpeed / 10 * (Timer - StartTime);
                WorldMatrix = Matrix.CreateFromYawPitchRoll(NewRotation.X, NewRotation.Y, NewRotation.Z) * Matrix.CreateScale(Size / ModelSize * (1 - (Timer - StartTime) / (float)LifeTime)) * Matrix.CreateTranslation(GlowPosition = GetPosition(Timer));
            }
        }
    }
}
