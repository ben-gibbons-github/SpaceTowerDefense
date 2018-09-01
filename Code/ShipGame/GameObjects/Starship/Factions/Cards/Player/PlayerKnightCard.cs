using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerKnightCard : PlayerCard
    {
        public PlayerKnightCard()
        {
            Name = "PlayerKnight";
            AttackName = "Knight";
        }

        public override SpecialWeapon GetWeapon()
        {
            GhostCast ghost = new GhostCast();
            ghost.SetCard(FactionCard.GetFactionUnitCard("CrystalKnight"));
            return ghost;
        }

        public override OffenseAbility GetOffenseAbility()
        {
            return new ProtectionCast();
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
            return new PlayerKnightGun();
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
