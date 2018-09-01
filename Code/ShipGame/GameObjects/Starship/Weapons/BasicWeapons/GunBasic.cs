using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class GunBasic
    {
        public FireMode[] FireModes;
        private UnitBasic Parent;
        public float Rotation; //radians
        public float IdealEngagementDistance = 500;

        public bool HasAmmo()
        {
            foreach (FireMode f in FireModes)
                if (f.Ammo > 0 || f.Ammo == -100)
                    return true;

            return false;
        }

        public float GetFireSpeed()
        {
            return FireModes[0].BulletSpeed;
        }

        public void SetLevel(float Level)
        {
            foreach (FireMode f in FireModes)
                f.SetLevel(Level);
        }

        protected void SetFireModes(params FireMode[] FireModes)
        {
            this.FireModes = FireModes;
            foreach (FireMode fmode in FireModes)
                fmode.SetParent(this);
        }

        public float getRotation()
        {
            return Rotation;
        }

        public void SetRotation(float Rotation)
        {
            this.Rotation = Rotation;
        }
        
        public UnitBasic getParent()
        {
            return Parent;
        }

        public void AutoFire(GameTime gameTime)
        {
            foreach (FireMode fmode in FireModes)
                fmode.Fire(gameTime);
        }

        public void Fire(GameTime gameTime, int firemode)
        {
            if (firemode < FireModes.Count())
                FireModes[firemode].Fire(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            foreach (FireMode fireMode in FireModes)
                fireMode.Update(gameTime);
        }

        public void SetParent(UnitBasic Parent)
        {
            this.Parent = Parent;
        }

        internal void ReadyROF()
        {
            foreach (FireMode f in FireModes)
                f.ReadyROF();
        }
    }
}
