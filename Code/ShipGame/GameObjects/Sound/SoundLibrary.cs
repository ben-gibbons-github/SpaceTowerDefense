using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace BadRabbit.Carrot
{
    public class SoundLibrary
    {
        static string BasePath = "Sounds/ShipGame/";

        static string[] InGameSounds =
        {
            "Death/CrystalTurretExplode",
            "Death/HumanEmpireTurretExplode2",
            "Death/MiningRingExplode2",
            "Death/SmallHumanExplode",
            "Death/MediumHumanExplode",
            "Death/HeavyHumanExplode",
            "Death/SmallCrystalExplode",
            "Death/MediumCrystalExplode",
            "Death/HeavyCrystalExplode",
            "Death/SmallMonsterExplode",
            "Death/HeavyMonsterExplode",
           
            "Environments/FieldCast",
            "Environments/FieldCollapse",
            "Environments/FieldHum",
            "Environments/MedicRevive",
            "Environments/MiningRingHum",
            "Environments/SignalTowerHum",
            "Environments/SlowFieldHum",
            "Environments/TurretRebuild",
            "Environments/UnitCloak",
            "Environments/UnitPowerUp",
            "Environments/UnitUnCloak",
            "Environments/DeafTone",

             "Impact/BigBombExplode2",
             "Impact/CrystalImpact",
             "Impact/DevourerBulletImpact",
             "Impact/EmpImpact",
             "Impact/HornetImpact",
             "Impact/ImmortalBulletImpact",
             "Impact/MineImpact",
             "Impact/PlasmaTurretImpact",
             "Impact/RailTurretImpact",
             "Impact/RelocationImpact",
             "Impact/ScatterTurretImpact",
             "Impact/ScramblerImpact",
             "Impact/ShieldFlare",
             "Impact/ShieldBounce",
             "Impact/SmallBombExplode",
             "Impact/SnapHit",
             "Impact/SplinterTurretImpact",
             "Impact/VampireImpact",
             
             "Menu/MenuBack",
             "Menu/MenuMove",
             "Menu/MenuSelect",
             "Menu/Ready",
             "Menu/MenuOpen",
             
             "Player/PlayerBuildMiningRing",
             "Player/PlayerBuildTurret",
             "Player/PlayerDash",
             "Player/PlayerDashReverse",
             "Player/PlayerDie",
             "Player/PlayerEngine2",
             "Player/PlayerGainAbility",
             "Player/PlayerInteract",
             "Player/PlayerLevelUp",
             "Player/ShieldRecharge",
             
             "PlayerFire/Grenade",
             "PlayerFire/Juggernaut",
             "PlayerFire/Laser",
             "PlayerFire/Raid",
             "PlayerFire/Rail",
             "PlayerFire/Rocket",
             "PlayerFire/SmallBomb",
             "PlayerFire/Snipe",
             "PlayerFire/Special",
             
             "Weapons/CollisionImpact",
             "Weapons/CrusherImpact",
             "Weapons/CrystalFire",
             "Weapons/CrystalKnightFire",
             "Weapons/DevourerImpact",
             "Weapons/FlameTurretFire",
             "Weapons/ForceFire",
             "Weapons/HeavyImpact",
             "Weapons/HornetFire",
             "Weapons/ImmortalFire",
             "Weapons/MineFieldTarget",
             "Weapons/OutcastFire",
             "Weapons/PitbullImpact",
             "Weapons/PlasmaTurretFire",
             "Weapons/RailTurretFire",
             "Weapons/ScatterTurretFire",
             "Weapons/SnapTurretFire",
             "Weapons/SpearFire",
             "Weapons/SplinterTurretFire",
             "Weapons/VampireFire",
             "Weapons/SiegeLaserFire",
        };

        public static Dictionary<string, SoundEffect> soundEffects;

        public static void Load()
        {
            if (soundEffects != null)
                return;

            soundEffects = new Dictionary<string, SoundEffect>();
            foreach (string soundName in InGameSounds)
            {
                soundEffects.Add(Path.GetFileNameWithoutExtension(soundName), Game1.content.Load<SoundEffect>(BasePath + soundName));
            }

            BasicMarker.SelectVolume = 1;
            BasicMarker.MoveVolume = 1;

            BasicMarker.MoveSound = soundEffects["MenuMove"];
            BasicMarker.SelectSound = soundEffects["MenuSelect"];

            FormFrame.OpenVolume = 0.25f;
            FormFrame.CloseVolume = 1;

            FormFrame.OpenSound = soundEffects["MenuOpen"];
            FormFrame.CloseSound = soundEffects["MenuBack"];
        }
    }
}
