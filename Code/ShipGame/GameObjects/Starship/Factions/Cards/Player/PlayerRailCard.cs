﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerRailCard : PlayerCard
    {
        public PlayerRailCard()
        {
            Name = "PlayerRail";
            AttackName = "Railgunner";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("Vampire"));
            return ghost;
        }

        public override ShipAbility GetAbility()
        {
            return new ShipAbiliyBlink();
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new CloakFieldCast();
        }

        public override float GetAcceleration()
        {
            return 0.9f;
        }

        public override float GetRotationSpeed()
        {
            return 0.025f;
        }

        public override float GetHullToughness()
        {
            return 1f;
        }

        public override float GetShieldToughness()
        {
            return 1f;
        }

        public override GunBasic GetGun()
        {
            return new PlayerRailGun();
        }

        public override GunBasic GetSuperGun()
        {
            return new PlayerSuperRailGun();
        }

        public override AttackType GetWeakness()
        {
            return AttackType.Red;
        }

        public override AttackType GetResistance()
        {
            return AttackType.Blue;
        }
    }
}
