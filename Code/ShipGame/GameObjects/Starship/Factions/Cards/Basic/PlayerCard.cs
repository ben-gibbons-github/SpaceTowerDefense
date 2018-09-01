using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class PlayerCard : FactionCard
    {
        public string AttackName = "";

        public virtual SpecialWeapon GetWeapon() { return null; }
        public virtual OffenseAbility GetOffenseAbility() { return null; }
        public virtual ShipAbility GetAbility() { return null; }
        public virtual float GetAcceleration() { return 1; }
        public virtual float GetRotationSpeed() { return 1; }
        public virtual float GetHullToughness() { return 1; }
        public virtual float GetShieldToughness() { return 1; }
        public virtual GunBasic GetGun() { return null; }
        public virtual GunBasic GetSuperGun() { return null; }
        public virtual AttackType GetWeakness() { return AttackType.None; }
        public virtual AttackType GetResistance() { return AttackType.None; }
    }
}