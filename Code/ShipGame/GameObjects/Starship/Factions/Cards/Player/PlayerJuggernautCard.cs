using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerJuggernautCard : PlayerCard
    {
        public PlayerJuggernautCard()
        {
            Name = "PlayerJuggernaut";
            AttackName = "Juggernaut";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("Cobra"));
            return ghost;
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new SpeedFieldCast();
        }

        public override ShipAbility GetAbility()
        {
            return new ShipAbiliyBlink();
        }

        public override float GetAcceleration()
        {
            return 1f;
        }

        public override float GetRotationSpeed()
        {
            return 0.0125f;
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
            return new PlayerJuggernautGun();
        }

        public override GunBasic GetSuperGun()
        {
            return new PlayerSuperShotGun();
        }

        public override AttackType GetWeakness()
        {
            return AttackType.Blue;
        }

        public override AttackType GetResistance()
        {
            return AttackType.Green;
        }
    }
}
