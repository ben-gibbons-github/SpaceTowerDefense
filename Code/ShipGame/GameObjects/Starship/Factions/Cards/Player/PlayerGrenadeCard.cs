using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerGrenadeCard : PlayerCard
    {
        public PlayerGrenadeCard()
        {
            Name = "PlayerGrenade";
            AttackName = "Grenadier";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("Hornet"));
            return ghost;
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new VirusWeaponCast();
        }

        public override ShipAbility GetAbility()
        {
            return new ShipAbiliyBlink();
        }

        public override float GetAcceleration()
        {
            return 0.7f;
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
            return new PlayerGrenadeGun();
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
