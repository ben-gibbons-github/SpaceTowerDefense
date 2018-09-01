using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShipGame.Wave;
using BadRabbit.Carrot.WaveFSM;

namespace BadRabbit.Carrot
{
    public class GhostCast : SpecialWeapon
    {
        static int MaxPauseTime = 1000;

        public UnitCard MyCard;
        int PauseTime = 0;
        bool Triggered = false;

        public override void Create(PlayerShip ParentShip)
        {
            MaxRechargeTime = 5000;
            PauseTime = MaxPauseTime;
            base.Create(ParentShip);
        }

        public override float GetProgress()
        {
            return Triggered ? (float)PauseTime / MaxPauseTime : 0;
        }

        public void SetCard(FactionCard c)
        {
            MyCard = (UnitCard)c;
        }

        public override bool ShipCanMove()
        {
            return !Triggered || PauseTime > MaxPauseTime;
        }

        public override void Update(GameTime gameTime)
        {
            if (Triggered)
            {
                PauseTime += gameTime.ElapsedGameTime.Milliseconds;
                if (ParentShip.FreezeTime > 0)
                {
                    Triggered = false;
                    ParentShip.ShakeScreen(50);
                    ParentShip.FreezeTime = 500;
                }
                else if (PauseTime > MaxPauseTime && ParentShip.shipAbility.ShipCanShoot())
                {
                    Activate();
                    Triggered = false;
                }
            }

            base.Update(gameTime);
        }

        public override void Trigger()
        {
            if (FactionManager.CanAfford(ParentShip.FactionNumber, 0, 150) && ParentShip.GetOffenseProgress() == 0)
            {
                Triggered = true;
                PauseTime = 0;
            }
            base.Trigger();
        }

        private void Activate()
        {
            if (!WaveStepState.WeaponsFree)
                return;

            FactionManager.AddEnergy(ParentShip.FactionNumber, -150);

            Vector3 Position3 = new Vector3(ParentShip.Position.X(), 0, ParentShip.Position.Y());
            float Theta = 0;
            float Offset = ParentShip.Size.X() * 2;

            FactionManager.Factions[ParentShip.FactionNumber].roundReport.UnitsSpawned += MyCard.GhostCount;

            for (int i = 0; i < MyCard.GhostCount; i++)
            {
                UnitShip u = (UnitShip)MyCard.GetUnit(ParentShip.FactionNumber);
                ParentShip.ParentLevel.AddObject(u);

                while (!ParentShip.TestFree(ParentShip.Position.get(), Theta, Offset, ParentShip.Size.X()))
                {
                    Theta += (float)Math.PI / 10f;
                    if (Theta > Math.PI * 2)
                    {
                        Theta -= (float)Math.PI * 2;
                        Offset += ParentShip.Size.X();
                    }
                }
                Vector2 BestPosition = Logic.ToVector2(Theta) * Offset + ParentShip.Position.get();

                u.SetForGhost();
                u.Position.set(BestPosition);
                u.SetLevel(WaveManager.CurrentWave / 5f * WaveCard.LevelMult, 1);

                Position3 = new Vector3(BestPosition.X, 0, BestPosition.Y);
                for (int j = 0; j < 30; j++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);
            }


            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), ParentShip.Size.X() * 5, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);
        }
    }
}
