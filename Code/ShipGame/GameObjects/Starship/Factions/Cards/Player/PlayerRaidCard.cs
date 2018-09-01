using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerRaidCard : PlayerCard
    {
        public PlayerRaidCard()
        {
            Name = "PlayerRaid";
            AttackName = "Scout";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("Cobra"));
            return ghost;
        }

        public override ShipAbility GetAbility()
        {
            return new ShipAbiliyBlink();
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new EmpWeaponCast();
        }

        public override float GetAcceleration()
        {
            return 1f;
        }

        public override float GetRotationSpeed()
        {
            return 0.025f;
        }

        public override float GetHullToughness()
        {
            return 2f;
        }

        public override float GetShieldToughness()
        {
            return 2f;
        }

        public override GunBasic GetGun()
        {
            return new PlayerRaidGun();
        }

        public override GunBasic GetSuperGun()
        {
            return new PlayerSuperShotGun();
        }

        public override AttackType GetWeakness()
        {
            return AttackType.None;
        }

        public override AttackType GetResistance()
        {
            return AttackType.Green;
        }
    }
}
