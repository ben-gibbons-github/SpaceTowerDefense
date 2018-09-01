using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace BadRabbit.Carrot
{
    public class SignalTower : UnitTurret
    {
        SoundEffectInstance SoundInstance;

        public SignalTower(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 500;
            HullToughness = 500;
            MaxEngagementDistance = 0;
            Resistence = AttackType.None;
            Weakness = AttackType.None;
            ShieldColor = new Color(1, 1, 1);
            ThreatLevel = 10f;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(SignalTowerCard.STurretSize));
        }

        public override void Destroy()
        {
            if (SoundInstance != null && !SoundInstance.IsDisposed)
            {
                SoundInstance.Dispose();
                SoundInstance = null;
            }

            base.Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
                SoundInstance = SoundManager.PlayLoopingSound(SoundInstance, "SignalTowerHum",
                    new Vector3(Position.X(), Y, Position.Y()), 0.05f, 400, 2);
            else
                SoundInstance.Volume = 0;

            base.Update(gameTime);
        }

        protected override void Upgrade()
        {
            ShieldToughness *= 3;
            HullToughness *= 3;

            base.Upgrade();
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.EmpireUnitIndex + 3;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Empire/Turret4");
        }
    }
}
