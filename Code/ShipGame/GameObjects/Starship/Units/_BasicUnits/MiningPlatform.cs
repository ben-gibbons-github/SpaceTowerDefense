using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ShipGame.Wave;
using BadRabbit.Carrot.WaveFSM;

namespace BadRabbit.Carrot
{
    public class MiningPlatform : UnitBuilding
    {
        static Color ParticleColor = new Color(0.4f, 0.175f, 0.15f) * 2f;
        static Color ParticleColor2 = new Color(0.3f, 0.1f, 0.05f) * 2f;

        static int MaxDamageTime = 0;

        public static Texture2D MRockPointer;

        private MineralRock MyRock;
        public int BuildState = 0;
        int Bombs = 6;
        int MaxBombs = 6;
        float BulletExplosionDistance = 400;
        float BulletExplosionDamage = 2;
        int DamageTime = 0;
        bool BonusProduction = false;

        public float SizeMult = 1;
        float SizeMultAcceleration = 0;
        float SizeMultGravity = 0.01f;
        float MinSizeMult = 1;
        float SizeMultBounce = 1;

        float DrawRotation = 0;
        float TargetDrawRotation = 0;
        //float RotationOffset = 0;
        bool UnitCommited = false;

        SoundEffectInstance SoundInstance;

        public MiningPlatform(int FactionNumber) :
            base(FactionNumber)
        {
            UpgradeCost = 350;
            this.HullToughness = 800;
            this.ShieldToughness = 140;
            CommitToFaction(FactionNumber);
        }

        public override void DrawFromMiniMap(Vector2 Position, float Size, Vector2 Min, Vector2 Max)
        {
            if (this.Position.X() > Max.X || this.Position.Y() > Max.Y || this.Position.X() < Min.X || this.Position.Y() < Min.Y)
                return;

            Vector2 MapPosition = (this.Position.get() - Min) /
                (Max - Min) * Size + Position;

            if (SizeMult > 0)
                Render.DrawOutlineRect(MapPosition - new Vector2(SizeMult * 4) - new Vector2(3), MapPosition + new Vector2(SizeMult * 4) - new Vector2(3), 2, TeamInfo.GetColor(GetTeam()));
            Render.DrawSprite(Render.BlankTexture, MapPosition - new Vector2(2), new Vector2(4), 0, TeamInfo.GetColor(GetTeam()));
        }

        protected override void DrawHealthBar(float HealthMult, Vector2 Position, float Size)
        {
            if (WaveManager.ActiveTeam != GetTeam())
                return;

            if (StarshipScene.DrawingShip == null || StarshipScene.DrawingShip.GetTeam() != GetTeam())
                base.DrawHealthBar(HealthMult, Position, Size);
            else
            {
                Position.Y -= Size;
                float SizeMult2 = (1f + SizeMult) * (1.75f - HealthMult);
                if (Vector2.Distance(Render.CurrentView.Size / 2, Position) > Render.CurrentView.Size.Y * 0.4f)
                {
                    Vector2 NewPosition = Vector2.Normalize(Position - Render.CurrentView.Size / 2) * (Render.CurrentView.Size.Y * 0.4f) + Render.CurrentView.Size / 2;
                    Render.DrawSprite(MRockPointer, NewPosition, new Vector2(Size) * SizeMult2, DrawRotation, TeamInfo.GetColor(GetTeam()));
                    base.DrawHealthBar(HealthMult, NewPosition, Size);
                    TargetDrawRotation = -Logic.ToAngle((Position - NewPosition));
                }
                else
                {
                    TargetDrawRotation = 0;
                    Render.DrawSprite(MRockPointer, Position, new Vector2(Size) * SizeMult2, DrawRotation, TeamInfo.GetColor(GetTeam()));
                    base.DrawHealthBar(HealthMult, Position, Size);
                }
            }
        }

        public void SetAsStarting()
        {
            BuildState = 100;
            this.ShieldToughness *= 2;
        }

        public override void NewWave()
        {
            //CanRebuild = true;
            
            base.NewWave();
        }

        public override bool CanBeTargeted()
        {
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            SoundInstance = SoundManager.PlayLoopingSound(SoundInstance, "MiningRingHum", 
                new Vector3(Position.X(), Y, Position.Y()), 0.05f, 400, 2);

            RotationOffsetSpeed = new Vector3(0.001f, 0.005f, 0.0025f);
            //RotationOffset += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.1f;
            Size.set(new Vector2(110));
            ShutDownTime = 0;
            DrawRotation = Logic.Clerp(DrawRotation, TargetDrawRotation, 0.1f * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f);

            DamageTime -= gameTime.ElapsedGameTime.Milliseconds;
            SizeMult += SizeMultAcceleration * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f / 6;
            SizeMultAcceleration -= SizeMultGravity * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 3;

            if (SizeMult < MinSizeMult)
            {
                SizeMult = MinSizeMult;
                if (DamageTime > 0)
                    SizeMultAcceleration = SizeMultBounce;
            }

            base.Update(gameTime);
        }

        public void MakeMoney()
        {
            if (WaveManager.ActiveTeam == GetTeam())
            {
                UpdateMapPosition();

                int ProdBase = MyRock.ProductionAmount.get();
                if (IsUpdgraded)
                    ProdBase += 100;
                if (WaveManager.CurrentWave > 15)
                    ProdBase *= 2;

                if (BonusProduction)
                    ProdBase *= WaveManager.GameSpeed;
                else
                    BonusProduction = true;

                int Production = ProdBase;


                if (Production > 0)
                {
                    FactionManager.AddCells(FactionNumber, Production);
                    TextParticleSystem.AddParticle(new Vector3(Position.X(), Y, Position.Y()), Production.ToString(), (byte)GetTeam(), TextParticleSystemIcons.CellsTexture);
                }
            }
        }

        private void UpdateMapPosition()
        {
            if (GetTeam() == WaveManager.ActiveTeam)
            {
                OverMap.TargetMax = Logic.Max(Position.get() + new Vector2(1000), OverMap.TargetMax);
                OverMap.TargetMin = Logic.Min(Position.get() - new Vector2(1000), OverMap.TargetMin);
            }
        }

        public void setRock(MineralRock rock)
        {
            this.MyRock = rock;
        }

        public override void SmallBomb(BasicShipGameObject Damager)
        {

        }

        public override void Create()
        {
            base.Create();
            MaxDamageTime = 2000;
            Size.set(new Vector2(120));

            if (MRockPointer == null)
                MRockPointer = AssetManager.Load<Texture2D>("Textures/ShipGame/MRockPointer");

            ShieldColor = new Color(0.5f, 0.5f, 1);
            ThreatLevel = 1.25f;

            if (GetTeam() == WaveManager.ActiveTeam)
                UpdateMapPosition();

            FactionManager.Factions[FactionNumber].MiningPlatformCount++;
            if (!UnitCommited)
            {
                UnitCommited = true;
                FactionManager.AddUnit(this);
            }
        }

        public override void BlowUp()
        {
            if (!Dead)
            {
                SoundManager.PlaySound("MiningRingExplode2", 1, 0, 0);
                SoundManager.DeafTone();

                Dead = true;
                DoExplosion();

                foreach (Faction f in FactionManager.Factions)
                {
                    if (f.Team != GetTeam())
                    {
                        f.roundReport.MiningPlatformsLost++;
                        f.AddEvent("Mining Platform Destroyed!", new Color(1, 0.5f, 0.5f), FactionEvent.KillTexture);
                    }
                    else
                    {
                        f.roundReport.MiningPlatformsLost++;
                        f.AddEvent("Mining Platform Lost!", new Color(1, 0.5f, 0.5f), FactionEvent.LossTexture);
                    }
                }

                FactionManager.TeamStreak[GetTeam()] = -1;
                FactionManager.Factions[FactionNumber].MiningPlatformCount--;

                if (FactionManager.Factions[FactionNumber].MiningPlatformCount < 1)
                {
                    FactionManager.Factions[FactionNumber].Dead = true;

                    bool TeamAlive = false;
                    foreach (Faction f in FactionManager.Factions)
                        if (!f.Dead && f.Team == GetTeam())
                        {
                            TeamAlive = true;
                            break;
                        }

                    if (!TeamAlive)
                    {
                        if (FactionManager.TeamDead.ContainsKey(GetTeam()))
                            FactionManager.TeamDead[GetTeam()] = true;
                        else
                            FactionManager.TeamDead.Add(GetTeam(), true);

                        WaveManager.SetState(PlayerEliminatedState.self);
                    }
                }

                MyRock.setPlatform(null);
                BulletExplosionDistance = 1000;
                BulletExplosionDamage = 10;
            }
            Destroy();
        }

        public override void Destroy()
        {
            if (SoundInstance != null && !SoundInstance.IsDisposed)
            {
                SoundInstance.Dispose();
                SoundInstance = null;
            }

            if (UnitCommited)
            {
                UnitCommited = false;
                FactionManager.RemoveUnit(this);
            }

            base.Destroy();

            Dead = true;
            PathFindingManager.Rebuild();
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (WaveManager.SuperWave)
                damage /= 10;

            DamageTime = Math.Min(1000,(int)(MaxDamageTime * damage));

            float PreviousHullDamage = HullDamage;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);

            if (HullDamage != PreviousHullDamage && (1 - HullDamage / HullToughness) * MaxBombs < Bombs)
            {
                FactionManager.TeamStreak[GetTeam()] -= 1f;

                Bombs -= 1; 
                BulletExplosionDistance = 600;
                BulletExplosionDamage = 5;
                DoExplosion();
            }
        }

        private void DoExplosion()
        {
            if (!Dead)
            {
                BulletExplosionDistance = 300 * (1 + (MaxBombs - Bombs) / MaxBombs) / WaveCard.LevelMult;
                BulletExplosionDamage = 1f / WaveCard.LevelMult;

                Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
                for (int i = 0; i < 10; i++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * 6, ParticleColor, 20 / WaveCard.LevelMult, 5);

                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 4000 / WaveCard.LevelMult, 5);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 400 / WaveCard.LevelMult, 7);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 300 / WaveCard.LevelMult, 7);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 500 / WaveCard.LevelMult, 7);

                for (int i = 0; i < 10; i++)
                    FlamingChunkSystem.AddParticle(Position3, Rand.V3() / WaveCard.LevelMult, new Vector3(0, -0.25f, 0),
                        Rand.V3(), Rand.V3() / 10, 20 / WaveCard.LevelMult, 60, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 500 / WaveCard.LevelMult, 4);
                for (int i = 0; i < 30; i++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * 10, new Color(1, 0.75f, 0.5f), 200 / WaveCard.LevelMult, 5);
            
            }
            else
            {
                foreach(PlayerShip p in ParentScene.Enumerate(typeof(PlayerShip)))
                    p.ShakeScreen(250);

                Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
                for (int i = 0; i < 100; i++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * 20, ParticleColor, 60, 5);

                ParticleManager.CreateParticle(Position3, Vector3.Zero, ParticleColor, 4000, 5);

                for (int i = 0; i < 20; i++)
                    FlamingChunkSystem.AddParticle(Position3, Rand.V3() * 10, new Vector3(0, -0.25f, 0),
                        Rand.V3(), Rand.V3() / 10, 40, 30, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

                for (int i = 0; i < 50; i++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * 15, new Color(1, 0.75f, 0.5f), 200, 5);

                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 2000, 4);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 3000, 4);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 4000, 4);

                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 500, 5);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 1000, 5);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 2000, 5);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 3000, 5);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), 4000, 5);

                BulletExplosionDistance = 1200 / WaveCard.LevelMult;
                BulletExplosionDamage = 5f / WaveCard.LevelMult;
            }

            QuadGrid grid = Parent2DScene.quadGrids.First.Value;

            for (int i = 0; i < 2; i++)
                foreach (Basic2DObject o in grid.Enumerate(Position.get(), new Vector2(BulletExplosionDistance * 2)))
                    if (o.GetType().IsSubclassOf(typeof(UnitShip)))
                    {
                        BasicShipGameObject s = (BasicShipGameObject)o;
                        float dist = Vector2.Distance(s.Position.get(), Position.get()) - o.Size.X() / 2;

                        if (dist < BulletExplosionDistance && GetTeam() != s.GetTeam())
                        {
                            float DistMult = 1;
                            if (dist > 0)
                                DistMult = (BulletExplosionDistance - dist) / BulletExplosionDistance;
                            s.Damage(DistMult * BulletExplosionDamage, DistMult, Vector2.Normalize(s.Position.get() - Position.get()), this, AttackType.Explosion);
                        }
                    }
        }
    }
}
