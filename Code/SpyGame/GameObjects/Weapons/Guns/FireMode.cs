using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpyGame
{
    public class FireMode
    {
        private BasicSpyUnit Parent = null;

        //Counters
        private int ROF = 0;
        private int BurstTime = 0;
        private int ReloadTime = 0;
        private int ClipSize = 0;
        private int BurstSize = 0;

        //Ammo
        protected int MaxROF;
        protected int MaxBurstTime;
        protected int MaxReloadTime;
        protected int MaxClipSize = 1;
        protected int MaxBurstSize = 1;

        //Creation
        protected float Accuracy;
        protected int BulletCount = 1;
        protected float Damage;
        protected int LifeTime = 900;


        public virtual void Fire(GameTime gameTime, float Direction)
        {
            if (ReloadTime <= 0)
            {
                bool CreatedMatrix = false;
                Matrix RotationMatrix = Matrix.Identity;

                while (ROF >= MaxROF && ClipSize > 0 && BurstSize > 0)
                {
                    if (!CreatedMatrix)
                    {
                        CreatedMatrix = true;
                        RotationMatrix = Matrix.CreateRotationZ(-Direction);
                    }

                    ROF -= MaxROF;
                    BurstTime = 0;
                    ClipSize--;
                    BurstSize--;

                    for (int i = 0; i < BulletCount; i++)
                    {
                        Bullet b = getBullet();
                        Parent.ParentLevel.AddObject(b);

                        b.Position.set(Parent.Position.get() + Vector3.Transform(getPositionPattern(i), RotationMatrix));
                        b.Damage = Damage;
                    }
                }
                if (ClipSize <= 0)
                    ReloadTime += gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public virtual float getDirectionPattern(int BulletNumb)
        {
            return 0;
        }

        public virtual Vector3 getPositionPattern(int BulletNumb)
        {
            return Vector3.Zero;
        }

        public virtual Bullet getBullet()
        {
            return null;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (ReloadTime > 0)
            {
                ReloadTime += gameTime.ElapsedGameTime.Milliseconds;
                if (ReloadTime > MaxReloadTime)
                {
                    ReloadTime = 0;
                    ClipSize = MaxClipSize;
                }
            }

            BurstTime += gameTime.ElapsedGameTime.Milliseconds;
            if (BurstTime > MaxBurstTime)
            {
                BurstTime = 0;
                BurstSize = MaxBurstSize;
            }

            ROF = Math.Min(MaxROF, ROF + gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}
