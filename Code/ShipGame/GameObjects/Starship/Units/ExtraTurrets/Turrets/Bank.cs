using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Bank : UnitTurret
    {
        static Color ParticleColor = new Color(0.3f, 0.175f, 0.1f);
        
        int Production = 50;

        public Bank(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 20;
            HullToughness = 30;
            MaxBuildTime = 5000;
            Resistence = AttackType.None;
            Weakness = AttackType.None;
            ShieldColor = ShieldInstancer.WhiteShield;
        }

        public override bool AllowInteract(PlayerShip p)
        {
            return false;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(BankCard.STurretSize));
        }

        public void MakeMoney()
        {
            if (WaveManager.ActiveTeam == GetTeam())
            {
                FactionManager.AddEnergy(FactionNumber, Production);
                FactionManager.AddCells(FactionNumber, Production);
                TextParticleSystem.AddParticle(new Vector3(Position.X(), Y, Position.Y()), Production.ToString(), (byte)GetTeam(), TextParticleSystemIcons.CellsTexture);
            }
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 4;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("ExtraTurrets/Turret5");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Blue || attackType == AttackType.Red)
                damage *= 0.2f;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
