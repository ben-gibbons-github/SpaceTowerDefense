
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerEngineerCard : PlayerCard
    {
        public PlayerEngineerCard()
        {
            Name = "PlayerEngineer";
            AttackName = "Engineer";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("Stingray"));
            return ghost;
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new TurretCast();
        }

        public override ShipAbility GetAbility()
        {
            return new ShipAbiliyBlink();
        }

        public override float GetAcceleration()
        {
            return 1;
        }

        public override float GetRotationSpeed()
        {
            return 0.05f;
        }

        public override float GetHullToughness()
        {
            return 1;
        }

        public override float GetShieldToughness()
        {
            return 1f;
        }

        public override GunBasic GetGun()
        {
            return new PlayerEngineerGun();
        }

        public override AttackType GetWeakness()
        {
            return AttackType.White;
        }

        public override AttackType GetResistance()
        {
            return AttackType.Green;
        }
    }
}
