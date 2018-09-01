using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerRocketCard : PlayerCard
    {
        public PlayerRocketCard()
        {
            Name = "PlayerRocket";
            AttackName = "Rocketier";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("Hornet"));
            return ghost;
        }

        public override ShipAbility GetAbility()
        {
            return new ShipAbiliyBlink();
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new DamageFieldCast();
        }

        public override float GetAcceleration()
        {
            return 0.6f;
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
            return new PlayerRocketGun();
        }

        public override GunBasic GetSuperGun()
        {
            return new PlayerSuperGrenadeGun();
        }

        public override AttackType GetWeakness()
        {
            return AttackType.Green;
        }

        public override AttackType GetResistance()
        {
            return AttackType.Red;
        }
    }
}
