using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class Settings : GameObject
    {
        public static bool TurretLives = false;
        public static int TurretLivesCount = 3;
        public static bool NewProduction = false;
        public static bool NewWaves = false;
        public static bool IgnoreTurretBuild = false;
        public static bool ShipsCanTargetShips = false;
        public static bool TurretsCanTargetTurrets = true;
        public static float CellsCostMult = 1;
        public static float EnergyCostMult = 1;
        public static int PlayerRespawnTime = 0;
        public static float StartingTurretDistance = 350;

        BoolValue turretLives;
        IntValue turretLivesCount;
        BoolValue newProduction;
        BoolValue newWaves;
        BoolValue ignoreTurretBuild;
        BoolValue unitsCanTargetUnits;
        BoolValue turretsCanTargetTurrets;
        FloatValue cellsCostMult;
        FloatValue energyCostMult;
        IntValue playerRespawnTime;
        FloatValue startingTurretDistance;

        public override void Create()
        {
            turretLives = new BoolValue("Turret Lives", false);
            turretLives.ChangeEvent = TurretLivesChange;

            newProduction = new BoolValue("New Production", false);
            newProduction.ChangeEvent = NewProductionChange;

            newWaves = new BoolValue("New Waves", false);
            newWaves.ChangeEvent = NewWavesChange;

            ignoreTurretBuild = new BoolValue("Turret Build", false);
            ignoreTurretBuild.ChangeEvent = IgnoreTurretBuildChange;

            turretLivesCount = new IntValue("Turret Lives Count", 3);
            turretLivesCount.ChangeEvent = TurretLivesCountChange;

            unitsCanTargetUnits = new BoolValue("Units Can Target Units", false);
            unitsCanTargetUnits.ChangeEvent = UnitsCanTargetUnitsChange;

            turretsCanTargetTurrets = new BoolValue("Turrets Can Target Turrets", true);
            turretsCanTargetTurrets.ChangeEvent = TurretsCanTargetTurretsChange;

            cellsCostMult = new FloatValue("Cells Cost Mult");
            cellsCostMult.ChangeEvent = CellsCostMustChange;

            energyCostMult = new FloatValue("Energy Cost Mult");
            energyCostMult.ChangeEvent = EnergyCostMultChange;

            playerRespawnTime = new IntValue("Player Respawn Time");
            playerRespawnTime.ChangeEvent = PlayerRespawnTimeChange;

            startingTurretDistance = new FloatValue("Starting Turret Distance", StartingTurretDistance);
            startingTurretDistance.ChangeEvent = StartingTurretDistanceChange;

            base.Create();
        }

        private void StartingTurretDistanceChange()
        {
            StartingTurretDistance = startingTurretDistance.get();
        }

        private void PlayerRespawnTimeChange()
        {
            PlayerRespawnTime = playerRespawnTime.get();
        }

        private void EnergyCostMultChange()
        {
            EnergyCostMult = energyCostMult.get();
        }

        private void CellsCostMustChange()
        {
            CellsCostMult = cellsCostMult.get();
        }

        private void TurretsCanTargetTurretsChange()
        {
            TurretsCanTargetTurrets = turretsCanTargetTurrets.get();
        }

        private void UnitsCanTargetUnitsChange()
        {
            ShipsCanTargetShips = unitsCanTargetUnits.get();
        }

        private void TurretLivesCountChange()
        {
            TurretLivesCount = turretLivesCount.get();
        }

        private void IgnoreTurretBuildChange()
        {
            IgnoreTurretBuild = ignoreTurretBuild.get();
        }

        private void NewWavesChange()
        {
            NewWaves = newWaves.get();
        }

        private void NewProductionChange()
        {
            NewProduction = newProduction.get();
        }

        private void TurretLivesChange()
        {
            TurretLives = turretLives.get();
        }
    }
}
