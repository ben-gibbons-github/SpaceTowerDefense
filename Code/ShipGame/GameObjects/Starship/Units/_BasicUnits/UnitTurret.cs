using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BadRabbit.Carrot.WaveFSM;

namespace BadRabbit.Carrot
{
    public class UnitTurret : UnitBuilding
    {
        public static int RebuildSoundTime = 0;

        public bool ShouldDeathSound = true;

        protected string DeathSound = "HumanTurretExplode2";
        protected float DeathVolume = 0.66f;

        public TurretCard MyCard;
        public static int MaxSearchTime = 200;

        new protected static bool Loaded = false;
        protected static Color BuildBarColor = Color.White;
        protected static Texture2D LevelUpIcon;

        private static float BrightnessChangeSpeed = 0.1f;
        private static int SearchTimeOffset = 0;
        private static Color PowerUpColor = new Color(0.3f, 0.15f, 0.1f);

        public static void TurretPlaceEvent()
        {
            MedicTurret.TurretPlaceEvent();
        }

        public int Lives = Settings.TurretLivesCount;
        public bool IsCrushed = false;

        protected float ShutDownBrightness = 1;
        protected int MaxPowerUpTime = 30000;
        protected int PowerUpTime = 0;
        public float MaxEngagementDistance = 500;
        protected int MaxBuildTime = 10000;
        public bool WorthAttacking = true;

        private int SearchTime = 0;
        private int BuildTimer = 0;
        private int HealTimer = 0;
        private BustedTurret MyBustedTurret;
        private BasicShipGameObject CurrentAttackTarget;
        private Vector2 RelocationPosition = Vector2.Zero;
        private bool IsRelocating = false;
        bool CloakCommited = false;

        public UnitTurret(int FactionNumber)
            : base(FactionNumber)
        {
            SearchTime = SearchTimeOffset;
            SearchTimeOffset += MaxSearchTime / 20 + 1;
            if (SearchTimeOffset > MaxSearchTime)
                SearchTimeOffset -= MaxSearchTime;

            CommitToFaction(FactionNumber);
            RotationSpeed = 10;

            Load();
        }

        public virtual void Relocate()
        {
            IsRelocating = true;
            RelocationPosition = Position.get() + Rand.NV2() * 300;
        }

        new private static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                LevelUpIcon = AssetManager.Load<Texture2D>("Textures/ShipGame/LevelUpIcon");
            }
        }

        public override void Interact(BadRabbit.Carrot.PlayerShip p)
        {
            Lives = 1;
            ShutDownTime = 0;
            Rebuild();

            base.Interact(p);
        }

        public override BasicShipGameObject ReturnCollision()
        {
            if ((fieldState == FieldState.Cloaked && FieldStateTime > 0) || WaveManager.ActiveTeam != GetTeam())
                return null;
            else
                return base.ReturnCollision();
        }

        public override void EmpStrike()
        {
            StunState = AttackType.Blue;
            FreezeTime = 1000000;
            base.EmpStrike();
        }

        public override void PlasmaCannonStrike()
        {
            Destroy();
        }

        public override bool StopsBullet(BasicShipGameObject Other)
        {
            return base.StopsBullet(Other) && Y < Size.X() / 4;
        }

        public override void Update(GameTime gameTime)
        {
            DeathSound = "HumanEmpireTurretExplode2";
            RebuildSoundTime += gameTime.ElapsedGameTime.Milliseconds;
            UpdateFieldState(gameTime);
            if (!Dead)
            {
                RotationOffset += RotationOffsetSpeed;

                if (IsRelocating)
                {
                    float RelocationSpeed = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                    if (Vector2.Distance(Position.get(), RelocationPosition) > RelocationSpeed * 1.5f)
                    {
                        if (Y < 200)
                        {
                            Y += RelocationSpeed;
                            if (Y > 200)
                                Y = 200;
                        }
                        else
                            Position.add(Vector2.Normalize(RelocationPosition - Position.get()) * RelocationSpeed);
                    }
                    else
                    {
                        if (Y > 0)
                            Y -= RelocationSpeed;
                        else
                        {
                            Position.set(RelocationPosition);
                            IsRelocating = false;
                        }
                    }
                }
                else if (GetTeam() == WaveManager.ActiveTeam)
                {
                    if (VirusTime > 0)
                    {
                        VirusTime -= gameTime.ElapsedGameTime.Milliseconds;
                        VirusParticles();
                        ShutDownTime = -1;
                    }

                    if (ShutDownTime < 1)
                    {
                        if (ShutDownBrightness < 1)
                        {
                            ShutDownBrightness += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * BrightnessChangeSpeed;
                            if (ShutDownBrightness > 1)
                                ShutDownBrightness = 1;

                            MyColor = new Color(ShutDownBrightness * 0.25f, ShutDownBrightness * 0.25f, ShutDownBrightness * 0.25f, 1);
                        }

                        if (!Building(gameTime))
                            AI(gameTime);

                        if (Guns != null)
                        {
                            if (PowerUpTime > 0)
                            {
                                PowerUpTime -= gameTime.ElapsedGameTime.Milliseconds;
                                Guns[0].Update(gameTime);
                                Guns[0].Update(gameTime);
                                Guns[0].Update(gameTime);
                                ParticleManager.CreateParticle(new Vector3(Position.X(), 0, Position.Y()), Vector3.Zero, PowerUpColor, Size.X() * (1 + Rand.F()), 1);
                            }
                            if (IsUpdgraded)
                            {
                                Guns[0].Update(gameTime);
                            }
                        }
                    }
                    else
                    {
                        ShutDownTime -= gameTime.ElapsedGameTime.Milliseconds;
                        if (ShutDownBrightness > 0)
                        {
                            ShutDownBrightness -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * BrightnessChangeSpeed;
                            if (ShutDownBrightness < 0)
                                ShutDownBrightness = 0;

                            MyColor = new Color(ShutDownBrightness * 0.25f, ShutDownBrightness * 0.25f, ShutDownBrightness * 0.25f, 1);
                        }
                    }
                }

                base.Update(gameTime);
            }
            else
            {
                PowerUpTime = 0;
                if (!Building(gameTime))
                    Rebuild();
            }
        }

        public override void Update2(GameTime gameTime)
        {
            if (!Dead)
                base.Update2(gameTime);
        }

        private void UpdateFieldState(GameTime gameTime)
        {
            if (FieldStateTime > 0)
            {
                if (fieldState == FieldState.Cloaked)
                {
                    if (CloakAlpha < 1)
                    {
                        if (!CloakCommited)
                        {
                            CloakCommited = true;
                            InstanceManager.AddDisplacementChild(this);
                        }

                        CloakAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed;
                        if (CloakAlpha > 1)
                        {
                            CloakAlpha = 1;
                            MyColor = new Color(0.25f, 0.25f, 0.25f, 0) * 0;
                        }
                        else
                            MyColor = new Color(0.25f, 0.25f, 0.25f, 1) * (1 - CloakAlpha);
                    }
                }
            }
            else
            {
                if (CloakAlpha > 0)
                {
                    CloakAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed;
                    if (CloakAlpha < 0)
                    {
                        CloakAlpha = 0;
                        MyColor = new Color(0.25f, 0.25f, 0.25f, 1);
                        if (CloakCommited)
                        {
                            CloakCommited = false;
                            InstanceManager.RemoveDisplacementChild(this);
                        }
                    }
                    else
                        MyColor = new Color(0.25f, 0.25f, 0.25f, 1) * (1 - CloakAlpha);
                }
            }
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (FreezeTime < 3000)
            {
                FreezeTime = 3000;
                StunState = AttackType.Blue;
                LastDamager = Damager;
            }
        }

        private bool Building(GameTime gameTime)
        {
            if (Lives < 1)
                return true;

            if (!Dead)
                return false;
            if (HealTimer < MaxBuildTime)
                HealTimer += gameTime.ElapsedGameTime.Milliseconds;
            else
            {
                HullDamage = 0;
                ShieldDamage = 0;
            }

            if (BuildTimer < MaxBuildTime)
            {
                BuildTimer += gameTime.ElapsedGameTime.Milliseconds;
                return BuildTimer <= MaxBuildTime;
            }
            else
                return false;
        }

        public override void NewWaveEvent()
        {
            UpdateMapPosition();

            bool WasDead = Dead;

            if (ShouldRebuild)
            {
                Lives = Settings.TurretLivesCount;
                IsCrushed = false;
                ShieldDamage = 0;
                HullDamage = 0;
                Rebuild();
                ShouldRebuild = false;
            }

            if (!IsCrushed)
            {
                Rebuild();
                base.NewWaveEvent();
            }

            VirusTime = 0;
            FieldStateTime = 0;
            fieldState = FieldState.None;

            ShutDownTime = -1;

            if (!Dead && WasDead && RebuildSoundTime > 1000)
            {
                RebuildSoundTime = 0;
                SoundManager.PlaySound("TurretRebuild", 0.5f, 0, 0);
            }
        }

        public override void PowerUp()
        {
            for (int i = 0; i < 3; i++)
                ParticleManager.CreateParticle(new Vector3(Position.X(), 0, Position.Y()), Vector3.Zero, PowerUpColor, Size.X() * 2.5f, 0);
            PowerUpTime = MaxPowerUpTime;
            base.PowerUp();
        }

        public void Rebuild()
        {
            if (!Dead || IsCrushed || 
                //!FactionManager.CanAfford(FactionNumber, RebuildCost) || 
                Lives < 1)
                return;

            ShouldDeathSound = true;

            //FactionManager.AddCells(FactionNumber, -RebuildCost);
            //TextParticleSystem.AddParticle(new Vector3(Position.X(), Y, Position.Y()), '-' + RebuildCost.ToString(), (byte)GetTeam());
            for (int i = 0; i < 10; i++)
                ParticleManager.CreateParticle(new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, Color.White, Size.X(), 1);
            
            BuildTimer = 0;
            Dead = false;
            MyBustedTurret.Deactivate();
            InstanceManager.AddChild(this);
            AddTag(GameObjectTag._2DSolid);
            AddTag(GameObjectTag._2DForward);
            ShieldDamage = 0;
            HullDamage = 0;
            FreezeTime = 0;
        }

        public override void BlowUp()
        {
            if (Dead || Lives < 1)
                return;

            Vector3 P3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 10; i++)
                LineParticleSystem.AddParticle(P3, P3 + Rand.V3() * MaxEngagementDistance, TeamInfo.GetColor(GetTeam()));

            foreach (Faction f in FactionManager.Factions)
            {
                if (MyCard == null)
                    MyCard = (TurretCard)FactionCard.FactionTurretDeck[0];

                if (f.Team != GetTeam())
                {
                    f.roundReport.TurretsKilled++;
                    f.AddEvent(MyCard.Name + " Destroyed", new Color(1, 0.5f, 0.5f), FactionEvent.KillTexture);
                }
                else
                {
                    f.roundReport.TurretsLost++;
                    f.AddEvent(MyCard.Name + " Lost", new Color(1, 0.5f, 0.5f), FactionEvent.LossTexture);
                }
            }

            if (ShouldDeathSound)
            {
                SoundManager.Play3DSound(DeathSound, new Vector3(Position.X(), Y, Position.Y()), DeathVolume, 1000, 1);
                ShouldDeathSound = false;
            }


            FreezeMult = 0;
            VirusTime = 0;
            DeathParticles();
            Lives--;

            Dead = true;
            if (MyBustedTurret == null)
            {
                MyBustedTurret = new BustedTurret(this);
                ParentLevel.AddObject(MyBustedTurret);
                MyBustedTurret.SetPosition(getPosition());
            }

            MyBustedTurret.WorldMatrix = WorldMatrix;
            MyBustedTurret.Activate();
            InstanceManager.RemoveChild(this);
            RemoveTag(GameObjectTag._2DSolid);
            RemoveTag(GameObjectTag._2DForward);

            LinkedList<GameObject> GList = Parent2DScene.GetList(GameObjectTag._2DSolid);
            if (GList.Contains(this))
                GList.Remove(this);

            BuildTimer = 0;

            float BulletExplosionDistance = 200;
            float BulletExplosionDamage = 1f;
            QuadGrid grid = Parent2DScene.quadGrids.First.Value;


            for (int i = 0; i < 2; i++)
            {
                bool ActivateDeathSound = true;

                foreach (Basic2DObject o in grid.Enumerate(Position.get(), new Vector2(BulletExplosionDistance * 2)))
                    if (o.GetType().IsSubclassOf(typeof(UnitShip)))
                    {
                        BasicShipGameObject s = (BasicShipGameObject)o;
                        float dist = Vector2.Distance(s.Position.get(), Position.get()) - o.Size.X() / 2;

                        if (dist < BulletExplosionDistance && GetTeam() != s.GetTeam() && s.CanBeTargeted())
                        {
                            float DistMult = 1;
                            if (dist > 0)
                                DistMult = (BulletExplosionDistance - dist) / BulletExplosionDistance;

                            if (s.GetType().IsSubclassOf(typeof(UnitShip)))
                            {
                                UnitShip ship = (UnitShip)s;
                                ship.CanDeathSound = ActivateDeathSound;
                            }
                            s.Damage(DistMult * BulletExplosionDamage, DistMult, Vector2.Normalize(s.Position.get() - Position.get()), this, AttackType.Explosion);

                            if (s.Dead)
                                ActivateDeathSound = false;
                            else if (s.GetType().IsSubclassOf(typeof(UnitShip)))
                            {
                                UnitShip ship = (UnitShip)s;
                                ship.CanDeathSound = true;
                            }
                        }
                    }
            }

            if (ShieldAlpha > 0)
            {
                ShieldInstancer.Remove(this);
                ShieldAlpha = -1;
            }
        }

        protected virtual void AI(GameTime gameTime)
        {
            SearchTime += gameTime.ElapsedGameTime.Milliseconds;
            if (SearchTime > MaxSearchTime)
            {
                AISearch(gameTime);
                SearchTime = 0;
            }
            AIGuns(gameTime);
        }

        private void AIGuns(GameTime gameTime)
        {
            if (!WaveStepState.WeaponsFree || Guns == null)
                return;

            if (VirusTime < 1)
            {
                if (CurrentAttackTarget == null)
                    return;

                Vector2 CurrentAttackTargetPosition = CurrentAttackTarget.getPosition();

                float TargetRotation = Logic.ToAngle(CurrentAttackTargetPosition - getPosition());
                if (Rotation.get() != TargetRotation)
                {
                    Rotation.set(MathHelper.ToDegrees(Logic.Clerp(Rotation.getAsRadians(), TargetRotation, MathHelper.ToRadians(RotationSpeed) * gameTime.ElapsedGameTime.Milliseconds / 1000 * 60)));
                    RotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z);
                }
            }
            else
            {
                Rotation.set(Rotation.get() + MathHelper.ToRadians(gameTime.ElapsedGameTime.Milliseconds * 20));
                RotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z);
            }

            foreach (GunBasic g in Guns)
                g.SetRotation(Rotation.getAsRadians());

            AutoFire(gameTime);
        }

        private void AISearch(GameTime gameTime)
        {
            CurrentAttackTarget = null;

            QuadGrid grid = Parent2DScene.quadGrids.First.Value;

            float BestDistance = MaxEngagementDistance;

            foreach (Basic2DObject o in grid.Enumerate(Position.get(), new Vector2(MaxEngagementDistance * 2)))
            {
                if (o.GetType().IsSubclassOf(typeof(UnitBasic)) && !o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)o;
                    if (!s.IsAlly(this) && s.CanBeTargeted())
                    {
                        float d = Vector2.Distance(Position.get(), o.Position.get()) - o.Size.X() / 2;

                        if (d / s.ThreatLevel < BestDistance)
                        {
                            BestDistance = d / s.ThreatLevel;
                            CurrentAttackTarget = s;
                        }
                    }
                }
            }
        }

        public override void Create()
        {
            DeathSound = "HumanEmpireTurretExplode2";
            Add(UnitTag.Turret);
            base.Create();
            TurretPlaceEvent();

            if (GetTeam() == WaveManager.ActiveTeam)
                UpdateMapPosition();
        }

        private void UpdateMapPosition()
        {
            OverMap.TargetMax = Logic.Max(Position.get() + new Vector2(500), OverMap.TargetMax);
            OverMap.TargetMin = Logic.Min(Position.get() - new Vector2(500), OverMap.TargetMin);
        }

        public override bool CanBeTargeted()
        {
            return base.CanBeTargeted() && ShutDownTime < 1 && GetTeam() == WaveManager.ActiveTeam;
        }

        public override void Destroy()
        {
            TurretPlaceEvent();
            if (MyBustedTurret != null)
                MyBustedTurret.Destroy();
            base.Destroy();
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (fieldState == FieldState.Cloaked && FieldStateTime > 0)
                return;

            if (Y > Size.X() / 4)
                return;

            if (GetTeam() != WaveManager.ActiveTeam)
                return;

            HealTimer = 0;
            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        protected override void DrawHealthBar(float HealthMult, Vector2 Position, float Size)
        {
            if (!Dead)
            {
                if (IsUpdgraded)
                    Render.DrawSprite(LevelUpIcon, Position - new Vector2(0, Size), new Vector2(Size), 0);
            }

            base.DrawHealthBar(HealthMult, Position, Size);
        }

        public void Eliminate()
        {
            DeathParticles();
            InstanceManager.RemoveChild(this);
            Destroy();
        }

        public virtual float GetWeight()
        {
            return 1;
        }
    }
}
