using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class WorldFlare : GameObject
    {
        float Interpolation;
        float Distance;
        float Speed;

        Color color;

        Vector3 To;
        Vector3 From;
        Vector3 SpawnPosition;

        public WorldFlare(float Distance, Vector3 SpawnPosition, Color color, float Speed)
        {
            this.color = color;
            this.SpawnPosition = SpawnPosition;
            this.From = SpawnPosition;
            this.To = Rand.V3() * Distance + SpawnPosition;
            this.Interpolation = Rand.F();

            this.Speed = Speed;
            this.Distance = Distance;
        }

        public override void Create()
        {
            AddTag(GameObjectTag.Update);
            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            Interpolation += gameTime.ElapsedGameTime.Milliseconds / (Math.Max(1, Vector3.Distance(To, From))) * Speed;
            while (Interpolation > 1)
            {
                Interpolation -= 1;
                From = To;
                To = Rand.V3() * Distance + SpawnPosition;
            }

            Vector3 InterpolatedPosition = Vector3.Lerp(From, To, Interpolation);

            ParticleManager.CreateParticle(InterpolatedPosition, Vector3.Zero, color * 0.4f, 75, 1);
        }
    }
}
