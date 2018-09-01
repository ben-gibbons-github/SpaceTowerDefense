using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerSingularityCard : PlayerCard
    {
        public PlayerSingularityCard()
        {
            Name = "PlayerSingularity";
            AttackName = "Singularity";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("Outcast"));
            return ghost;
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new EmpCast();
        }

        public override ShipAbility GetAbility()
        {
            return new ShipAbiliyBlink();
        }

        public override float GetAcceleration()
        {
            return 0.75f;
        }

        public override float GetRotationSpeed()
        {
            return 0.05f;
        }

        public override float GetHullToughness()
        {
            return 1.5f;
        }

        public override float GetShieldToughness()
        {
            return 1.5f;
        }

        public override GunBasic GetGun()
        {
            return new PlayerSingularityGun();
        }

        public override AttackType GetWeakness()
        {
            return AttackType.None;
        }

        public override AttackType GetResistance()
        {
            return AttackType.White;
        }
    }
}
