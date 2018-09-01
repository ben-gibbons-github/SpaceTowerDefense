using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BasicParticleSystem
    {
        public virtual void Ready() { }
        public virtual void ReadyDraw() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(Camera3D DrawCamera) { }
        public virtual void AddParticle(Vector3 position, Vector3 velocity, Color color, float Size) { }
        public virtual void LoadTexture() { }
        public virtual void Clear() { }
    }
}
