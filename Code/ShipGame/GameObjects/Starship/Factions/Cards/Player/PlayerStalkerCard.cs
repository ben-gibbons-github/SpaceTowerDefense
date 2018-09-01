using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerStalkerCard : PlayerCard
    {
        public PlayerStalkerCard()
        {
            Name = "PlayerStalker";
            AttackName = "Stalker";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("Immortal"));
            return ghost;
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new ImmortalityCast();
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
            return new PlayerStalkerGun();
        }

        public override AttackType GetWeakness()
        {
            return AttackType.Red;
        }

        public override AttackType GetResistance()
        {
            return AttackType.Green;
        }
    }
}
