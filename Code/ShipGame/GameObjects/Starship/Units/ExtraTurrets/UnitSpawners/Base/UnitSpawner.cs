using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class UnitSpawner : UnitTurret
    {
        public UnitCard SpawnCard;
        int UnitCount = 0;
        int UnitDelay = 0;
        int MaxUnitDelay = 0;
        SpawnerFrame frame;

        public UnitSpawner(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 50;
            HullToughness = 50;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(64));
            frame = new SpawnerFrame(Size, Position, Rotation);

            CreateUnits();
        }

        public override void Destroy()
        {
            InstanceManager.RemoveChild(frame);
            base.Destroy();
        }

        public override void NewWaveEvent()
        {
            CreateUnits();

            base.NewWaveEvent();
        }

        private void CreateUnits()
        {
            UnitCount = SpawnCard.GhostCount;
            MaxUnitDelay = 300000 / (SpawnCard.GhostCount * 10);

            if (IsUpdgraded)
            {
                UnitCount *= 2;
                MaxUnitDelay /= 2;
            }
        }

        public override void Update(GameTime gameTime)
        {
            frame.WorldMatrix = WorldMatrix;
            if (UnitCount > 0 && !Dead && WaveFSM.WaveStepState.WeaponsFree)
            {
                UnitDelay += gameTime.ElapsedGameTime.Milliseconds;
                if (UnitDelay > MaxUnitDelay)
                {
                    UnitCount--;
                    UnitDelay -= MaxUnitDelay;

                    UnitShip s = (UnitShip)SpawnCard.GetUnit(FactionNumber);
                    ParentLevel.AddObject(s);
                    s.SetLevel((IsUpdgraded ? 3 : 2) * WaveManager.DifficultyMult, 1);

                    if (GetTeam() == WaveManager.ActiveTeam)
                    {
                        float Theta = 0;
                        float Offset = 0;
                        Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());

                        while (!TestFree(Position.get(),Theta, Offset, Size.X()))
                        {
                            Theta += (float)Math.PI / 10f;
                            if (Theta > Math.PI * 2)
                            {
                                Theta -= (float)Math.PI * 2;
                                Offset += Size.X() / 2;
                            }
                        }

                        Vector2 BestPosition = Position.get() + Logic.ToVector2(Theta) * Offset;

                        s.Position.set(BestPosition);

                        Position3 = new Vector3(BestPosition.X, 0, BestPosition.Y);
                        for (int j = 0; j < 30; j++)
                            ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);

                        Position3 = new Vector3(Position.X(), 0, Position.Y());
                        ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 5, 4);
                        for (int i = 0; i < 30; i++)
                            ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);
                    }
                    else
                        s.Position.set(NeutralManager.GetSpawnPosition());
                }
            }
                
            base.Update(gameTime);
        }
    }
}
