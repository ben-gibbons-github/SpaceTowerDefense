using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class UnitShip : UnitBasic
    {
        public static int MaxSearchTime = 1000;
        public static float IdealDistanceFromTarget = 300;
        public static int MaxMoveChangeTime = 500;
        static int MaxAngerTime = 500;

        private static int MaxBulletDodgeTime = 100;
        private static int SearchTimeOffset = 0;
        static int MaxDisplaceTime = 1000;
        static int MaxNoShootTime = 800;
        public int InvTime = 0;

        public bool CanDeathSound = true;
        protected string DeathSound = "MediumHumanExplode";
        protected float DeathVolume = 1;
        protected float DeathDistance = 1000;
        protected float DeathExponenent = 1.5f;

        public float BulletDodgeDistance = 300;
        public Bullet BulletToDodge = null;
        public int BulletDodgeTime;
        public bool PreferPlayer;
        public bool Slowed = false;
        public bool IsGhostMode = false;
        public bool CanCloak = false;
        public bool HasCloaked = false;
        public bool HasSummoned = false;
        bool AppliedHuge = false;
        protected int NoShootTime = 0;
        int AngerTime = 0;

        public bool CanSummon = false;
        public UnitCard SummonCard = null;

        public virtual int GetCloakTime()
        {
            return (int)(2000 * UnitLevel);
        }

        public float MaxEngagementDistance = -1;
        public float MinEngagementDistance = -1;
        public bool IsHuge = false;

        protected Vector2 TargetUnitSize = Vector2.Zero;
        protected bool DodgesBullets = false;
        protected bool RunningAway = false;

        protected int SearchTime = 0;
        protected int MoveChangeTime = 0;
        protected BasicShipGameObject CurrentAttackTarget;
        protected BasicShipGameObject CurrentMoveTarget;
        protected int MaxGhostTime = 8000;

        private Vector2 TargetPosition;
        private bool DisplacedFromPath = false;
        private int GhostTime = 0;

        int DisplaceTime = 0;


        PathFindingNode TargetNode;
        bool CloakCommited = false;

        public UnitShip(int FactionNumber)
            : base(FactionNumber)
        {
            MaxGhostTime = 30000;
            SearchTime = SearchTimeOffset;
            SearchTimeOffset += MaxSearchTime / 20 + 1;
            if (SearchTimeOffset > MaxSearchTime)
                SearchTimeOffset -= MaxSearchTime;

            Moveable = true;
            
            RotationSpeed = 0.025f; // Degrees
            CommitToFaction(FactionNumber);
        }

        public virtual bool SnapBounce()
        {
            return true;
        }

        public override bool CanBeTargeted()
        {
            return base.CanBeTargeted() && (fieldState != FieldState.Cloaked || FieldStateTime < 1);
        }

        public override void Create()
        {
            base.Create();
            Size.ChangeEvent = SizeChange;
        }

        private void SizeChange()
        {
            TargetUnitSize = Size.get();
            Size.setNoPerform(Vector2.Zero);
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if ((InvTime < 1 && (FieldStateTime < 1 || fieldState != FieldState.Cloaked)) || attackType == AttackType.Explosion || attackType == AttackType.Melee)
            {
                if (CanCloak)
                {
                    SoundManager.Play3DSound("UnitCloak", new Vector3(Position.X(), Y, Position.Y()), 0.25f, 800, 2);

                    fieldState = FieldState.Cloaked;
                    HasCloaked = true;
                    CanCloak = false;
                    FieldStateTime = GetCloakTime();
                }
                else
                {
                    if (attackType != AttackType.White)
                    {
                        SummonUnits();
                    }

                    NoShootTime = MaxNoShootTime;
                    if (attackType == AttackType.White)
                        ThreatLevel *= 1.5f;

                    if (!PathFindingManager.CollisionLine(Position.get(), Damager.Position.get()))
                    {
                        AngerTime = MaxAngerTime;
                        CurrentAttackTarget = Damager;
                    }

                    base.Damage(damage, pushTime, pushSpeed, Damager, attackType);

                    if (HullDamage >= HullToughness && Damager.FactionNumber != NeutralManager.NeutralFaction && ScoreToGive > 0)
                    {
                        if (CanDeathSound)
                            SoundManager.Play3DSound(DeathSound, new Vector3(Position.X(), Y, Position.Y()), DeathVolume * 0.5f, DeathDistance, DeathExponenent * 2);

                        ScoreToGive = (int)(ScoreToGive * (0.75f + 0.25f * UnitLevel) * (IsHuge ? 1.25f : 1) * 
                            (HasCloaked || CanCloak ? 1.5f : 1) * (CanSummon || HasSummoned ? 4 : 1));
                        TextParticleSystem.AddParticle(new Vector3(Position.X(), Y, Position.Y()), ScoreToGive.ToString(), (byte)Damager.GetTeam());
                        FactionManager.AddScore(Damager.FactionNumber, ScoreToGive);
                        FactionManager.Factions[Damager.FactionNumber].roundReport.UnitKills++;
                        ScoreToGive = 0;
                    }
                }
            }
        }

        public override int GetUnitWeight()
        {
            if (CanSummon || HasSummoned)
                return base.GetUnitWeight() + 5;
            else
                return base.GetUnitWeight();
        }

        private void SummonUnits()
        {
            if (!CanSummon)
                return;

            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            CanSummon = false;
            HasSummoned = true;

            float Theta = 0;
            float Offset = Size.X() * 2;
            for (int i = 0; i < SummonCard.GhostCount; i++)
            {
                UnitShip u = (UnitShip)SummonCard.GetUnit(FactionNumber);
                ParentLevel.AddObject(u);

                while (!TestFree(Position.get(),Theta, Offset, Size.X()))
                {
                    Theta += (float)Math.PI / 10f;
                    if (Theta > Math.PI * 2)
                    {
                        Theta -= (float)Math.PI * 2;
                        Offset += Size.X();
                    }
                }
                Vector2 BestPosition = Logic.ToVector2(Theta) * Offset + Position.get();

                u.SetForGhost();
                u.SetLevel(UnitLevel, 1);
                u.Position.set(BestPosition);

                Position3 = new Vector3(BestPosition.X, 0, BestPosition.Y);
                for (int j = 0; j < 30; j++)
                    ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);
            }


            Position3 = new Vector3(Position.X(), 0, Position.Y());
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 5, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);
        }

        protected override void DrawHealthBar(float HealthMult, Vector2 Position, float Size)
        {
            Vector2 Offset = new Vector2(Size, 2.5f);

            Render.DrawSolidRect(Position - Offset, Position + Offset, HealthBackgroundColor);
            Render.DrawSolidRect(Position - Offset + Vector2.One, Position - Offset + new Vector2((Size * 2 - 2) * HealthMult, 4), TeamInfo.GetColor(GetTeam()));
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (ShieldDamage > 0 || HullDamage > 0)
            {
                float HealthMult = 1 - (((HullDamage > HullToughness ? HullToughness : HullDamage) +
                    (ShieldDamage > ShieldToughness ? ShieldToughness : ShieldDamage)) /
                    (ShieldToughness + HullToughness));
                float TimeMult = 1 - GhostTime / MaxGhostTime;
                if (TimeMult < HealthMult)
                    HealthMult = TimeMult;

                Vector3 Position3 = Game1.graphicsDevice.Viewport.Project(
                    new Vector3(this.Position.X(), Y, this.Position.Y()), StarshipScene.CurrentCamera.ProjectionMatrix,
                    StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);

                Vector3 Size3 = Game1.graphicsDevice.Viewport.Project(
                    new Vector3(this.Position.X() + this.Size.X(), Y, this.Position.Y()), StarshipScene.CurrentCamera.ProjectionMatrix,
                    StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);

                Vector2 Position = new Vector2(Position3.X, Position3.Y) - Render.CurrentView.Position;
                float Size = Vector2.Distance(Position, new Vector2(Size3.X, Size3.Y) - Render.CurrentView.Position) / 1.6f;
                Position.Y -= Size;

                DrawHealthBar(HealthMult, Position, Size);
            }
        }

        public override void Destroy()
        {
            SummonUnits();

            if (CloakCommited)
            {
                CloakCommited = false;
                InstanceManager.RemoveDisplacementChild(this);
            }

            base.Destroy();
        }

        public override void BlowUp()
        {
            if (CloakCommited)
            {
                CloakCommited = false;
                InstanceManager.RemoveDisplacementChild(this);
            }
            if (IsGhostMode)
            {
                IsGhostMode = false;
                FactionManager.NeutralUnitCount--;
            }

            base.BlowUp();
        }

        public void SetForSummoning(UnitCard f)
        {
            this.SummonCard = f;
            this.CanSummon = true;
        }

        public void SetBulletToDodge(Bullet b)
        {
            BulletToDodge = b;
            BulletDodgeTime = MaxBulletDodgeTime;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateFieldState(gameTime);

            if (IsHuge)
            {
                if (!AppliedHuge)
                {
                    TargetUnitSize *= 1.35f;
                    ShieldToughness *= 3;
                    HullToughness *= 3;
                    CollisionDamage *= 3;
                    Acceleration *= 0.5f;
                    AppliedHuge = true;
                }
                if (Guns != null && Guns[0] != null)
                {
                    Guns[0].Update(gameTime);
                    Guns[0].Update(gameTime);
                }
            }
            if (TargetUnitSize != Size.get())
            {
                Size.setNoPerform(Size.get() + Vector2.One * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f);
                if (Size.X() > TargetUnitSize.X)
                    Size.setNoPerform(TargetUnitSize);
            }

            if (IsGhostMode)
            {
                GhostTime += gameTime.ElapsedGameTime.Milliseconds;
                if (GhostTime > MaxGhostTime)
                    BlowUp();
            }

            if (FreezeTime < 0)
                AI(gameTime);

            if (!WaveFSM.WaveStepState.WeaponsFree)
                BlowUp();

            base.Update(gameTime);
        }

        public void SetForGhost()
        {
            if (!IsGhostMode)
            {
                IsGhostMode = true;
                FactionManager.NeutralUnitCount++;
            }
        }

        private void UpdateFieldState(GameTime gameTime)
        {
            InvTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (InvTime < 1)
            {
                if (FieldStateTime > 0)
                {
                    FieldStateTime -= gameTime.ElapsedGameTime.Milliseconds;
                    if (fieldState == FieldState.SpeedBoost)
                    {
                        if (MyColor.R > 64)
                            MyColor.R = (byte)Math.Max(MyColor.R - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);
                        if (MyColor.G < 255)
                            MyColor.G = (byte)Math.Min(MyColor.G + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 255);
                        if (MyColor.B > 64)
                            MyColor.B = (byte)Math.Max(MyColor.B - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);

                        ParticleManager.CreateParticle(new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, new Color(0.25f, 1, 0.5f), Size.X() * 10, 1);
                    }
                    if (fieldState == FieldState.DamageBoost)
                    {
                        if (MyColor.R < 255)
                            MyColor.R = (byte)Math.Max(MyColor.R + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 255);
                        if (MyColor.G > 64)
                            MyColor.G = (byte)Math.Min(MyColor.G - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);
                        if (MyColor.B > 64)
                            MyColor.B = (byte)Math.Max(MyColor.B - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);

                        ParticleManager.CreateParticle(new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, new Color(1, 0.5f, 0.25f), Size.X() * 10, 1);
                    }
                    if (fieldState == FieldState.Cloaked)
                    {
                        if (CloakAlpha < 1)
                        {
                            if (!CloakCommited && !Dead)
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
                        if (FieldStateTime < 0)
                            SoundManager.Play3DSound("UnitUnCloak", new Vector3(Position.X(), Y, Position.Y()), 0.2f, 700, 3);
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
                    else
                    {
                        //if (IsGhostMode)
                        //    ParticleManager.CreateParticle(new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, new Color(0.5f, 0.25f, 0.5f), Size.X() * 10, 1);

                        int TargetR = 64;
                        int TargetB = 64;

                        if (MyColor.R < TargetR)
                            MyColor.R = (byte)Math.Min(MyColor.R + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, TargetR);
                        else if (MyColor.R > TargetR)
                            MyColor.R = (byte)Math.Max(MyColor.R - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, TargetR);
                        if (MyColor.G < 64)
                            MyColor.G = (byte)Math.Min(MyColor.G + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);
                        else if (MyColor.G > 64)
                            MyColor.G = (byte)Math.Max(MyColor.G - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);
                        if (MyColor.B < TargetB)
                            MyColor.B = (byte)Math.Min(MyColor.B + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, TargetB);
                    }
                }
            }
            else
            {
                if (MyColor.R < 255)
                    MyColor.R = (byte)Math.Min(MyColor.R + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 255);
                if (MyColor.G < 255)
                    MyColor.G = (byte)Math.Min(MyColor.G + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 255);
                if (MyColor.B < 255)
                    MyColor.B = (byte)Math.Min(MyColor.B + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 255);
            }
        }

        public override void Update2(GameTime gameTime)
        {
            base.Update2(gameTime);
            Position.set(Vector2.Clamp(Position.get(), Parent2DScene.MinBoundary.get(), Parent2DScene.MaxBoundary.get()));

            if (CrystalWall.SortedWalls.ContainsKey(WaveManager.ActiveTeam))
                foreach (CrystalWall n in CrystalWall.SortedWalls[WaveManager.ActiveTeam])
                    if (!n.Dead)
                        foreach (CrystalWallConnection n2 in n.ConnectedWalls)
                            if (!n2.wall.Dead)
                            {
                                float MoveAmount = (Size.X() + n.getSize().X * 0.75f) / 2 - Logic.DistanceLineSegmentToPoint(n.Position.get(), n2.wall.Position.get(), Position.get());

                                if (MoveAmount > 0)
                                {
                                    Vector2 MoveVector;

                                    if (!n2.LineIsVertical)
                                    {
                                        if (n2.LineSlope == 0)
                                        {
                                            MoveVector = new Vector2(0, -1);
                                            if (Position.Y() > n2.LineSlope * Position.X() + n2.LineIntercept)
                                                MoveAmount = -MoveAmount;
                                        }
                                        else
                                        {
                                            MoveVector = new Vector2(1, -1 / n2.LineSlope);
                                            if (!(Position.Y() > n2.LineSlope * Position.X() + n2.LineIntercept ^ n2.LineSlope > 0))
                                                MoveAmount = -MoveAmount;
                                        }

                                        MoveVector.Normalize();
                                    }
                                    else
                                    {
                                        MoveVector = new Vector2(Position.X() > n.Position.X() ? 1 : -1, 0);
                                    }

                                    Position.set(Position.get() + MoveVector * MoveAmount);


                                    float d1 = Vector2.Distance(Position.get(), n.Position.get());
                                    float d2 = 0;
                                    if (!PathFindingManager.CollisionLine(Position.get(), n.Position.get()))
                                        Vector2.Distance(Position.get(), n2.wall.Position.get());

                                    CurrentAttackTarget = d1 > d2 ? n : n2.wall;
                                    AngerTime = MaxAngerTime;
                                }
                            }
        }

        protected virtual void AI(GameTime gameTime)
        {
            SearchTime += gameTime.ElapsedGameTime.Milliseconds;
            if (SearchTime > MaxSearchTime || (CurrentAttackTarget != null && CurrentAttackTarget.Dead))
            {
                SearchTime -= MaxSearchTime;
                AISearch(gameTime);
            }

            AIMove(gameTime);
            AIFireGuns(gameTime);
        }

        protected virtual bool AIFireGuns(GameTime gameTime)
        {
            NoShootTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (NoShootTime < 1 && Guns != null && CurrentAttackTarget != null && Vector2.Distance(Position.get(), CurrentAttackTarget.Position.get()) < GetEngagementDistance())
            {
                Vector2 CurrentAttackTargetPosition = CurrentAttackTarget.getPosition();
                /*
                if (CurrentAttackTarget.GetType().IsSubclassOf(typeof(UnitBasic)))
                {
                    UnitBasic b = (UnitBasic)CurrentAttackTarget;
                    CurrentAttackTargetPosition += Vector2.Distance(CurrentAttackTarget.getPosition(), getPosition()) / Guns[0].GetFireSpeed() *
                        b.Speed * (b.MaxDragTime - b.DragTime) / b.MaxDragTime * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000;
                }
                */
                float TargetRotation = Logic.ToAngle(CurrentAttackTargetPosition - Position.get());

                foreach (GunBasic g in Guns)
                    if (g != null)
                        g.SetRotation(TargetRotation);
                foreach (GunBasic g in Guns)
                    if (g != null)
                {
                    g.AutoFire(gameTime);
                    if (!g.HasAmmo())
                        MaxEngagementDistance = -1;
                }
                return true;
            }
            return false;
        }

        private void AIMove(GameTime gameTime)
        {
            MoveChangeTime += gameTime.ElapsedGameTime.Milliseconds;
            if (MoveChangeTime > MaxMoveChangeTime)
            {
                MoveChangeTime -= MaxMoveChangeTime;

                TargetPosition = Position.get();

                if (!DodgesBullets || BulletToDodge == null || BulletToDodge.TimeAlive > BulletToDodge.LifeTime)
                {
                    if (BulletToDodge != null)
                        BulletToDodge = null;

                    if (CurrentAttackTarget != null)
                    {
                        float d = Vector2.Distance(CurrentAttackTarget.getPosition(), getPosition());

                        if (!RunningAway)
                        {
                            if (d > MinEngagementDistance)
                                TargetPosition = CurrentAttackTarget.getPosition();
                            else if (d < MinEngagementDistance)
                                TargetPosition = Position.get();
                        }
                        else
                            TargetPosition = Position.get() + (Position.get() - CurrentAttackTarget.getPosition());

                        DisplacedFromPath = true;
                    }
                    else
                    {
                        if (TargetNode == null)
                        {
                            TargetNode = PathFindingNode.GetBestNode(Position.get());
                            DisplacedFromPath = false;
                        }
                        else
                        {
                            DisplaceTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (DisplaceTime > MaxDisplaceTime)
                            {
                                DisplaceTime -= MaxDisplaceTime;
                                DisplacedFromPath = true;
                            }

                            if (DisplacedFromPath)
                            {
                                if (PathFindingManager.CollisionLine(Position.get(), TargetNode.Position.get()))
                                    TargetNode = PathFindingNode.GetBestNode(Position.get());
                                DisplacedFromPath = false;
                            }
                        }

                        TargetPosition = TargetNode.Position.get();

                        if (TargetNode.GetNext() != null && !PathFindingManager.CollisionLine(Position.get(), TargetNode.GetNext().Position.get()) ||
                            (Vector2.Distance(getPosition(), TargetPosition) < 16))
                            TargetNode = TargetNode.GetNext();
                    }
                }
                else
                {
                    Vector2 bPos = BulletToDodge.getPosition();
                    bPos += BulletToDodge.Speed / BulletToDodge.Speed.Length() * Vector2.Distance(bPos, Position.get());

                    TargetPosition = (Position.get() - (bPos - Position.get()));

                    BulletDodgeTime -= gameTime.ElapsedGameTime.Milliseconds;

                    if (BulletDodgeTime < 0)
                    {
                        TargetPosition = CurrentMoveTarget == null ? TargetPosition : CurrentMoveTarget.getPosition();
                        BulletToDodge = null;
                    }

                    DisplacedFromPath = true;
                }
            }

            if (Vector2.Distance(getPosition(), TargetPosition) > 16)
                Accelerate(gameTime, TargetPosition - getPosition());
            else if (CurrentAttackTarget != null)
            {
                Rotation.set(MathHelper.ToDegrees(Logic.Clerp(Rotation.getAsRadians(), Logic.ToAngle(CurrentAttackTarget.Position.get() - Position.get()), RotationSpeed * gameTime.ElapsedGameTime.Milliseconds * 60.0f / 1000.0f)));
                RotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z);
            }
        }

        protected float GetEngagementDistance()
        {
            return MaxEngagementDistance * GetDamageMult();
        }

        protected virtual void AISearch(GameTime gameTime)
        {
            AngerTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (AngerTime > 0 && CurrentAttackTarget != null && CurrentAttackTarget.CanBeTargeted())
                return;

            CurrentAttackTarget = null;
            float BestDistance = 1000000;

            StarshipScene scene = (StarshipScene)Parent2DScene;

            if (GetTeam() != WaveManager.ActiveTeam)
            {
                foreach (Basic2DObject o in FactionManager.SortedUnits[WaveManager.ActiveTeam])
                    if (o != this)
                    {
                        float d = Vector2.Distance(Position.get(), o.getPosition());
                        if (d < BestDistance && o.GetType().IsSubclassOf(typeof(UnitBasic)))
                        {
                            UnitBasic s = (UnitBasic)o;
                            if (s.GetTeam() == WaveManager.ActiveTeam && s.CanBeTargeted())
                                if (d / s.ThreatLevel < BestDistance && !PathFindingManager.CollisionLine(Position.get(), s.Position.get()))
                                {
                                    BestDistance = d / s.ThreatLevel;
                                    CurrentAttackTarget = s;
                                }
                        }
                    }
            }
            else
            {
                foreach (Basic2DObject o in Parent2DScene.GetList(GameObjectTag._2DSolid))
                    if (o != this && o.GetType().IsSubclassOf(typeof(UnitBasic)) && !o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                    {
                        float d = Vector2.Distance(Position.get(), o.getPosition());
                        
                        if (d < BestDistance)
                        {
                            UnitBasic s = (UnitBasic)o;
                            if (!IsAlly(s) && s.CanBeTargeted() &&
                                d / s.ThreatLevel < BestDistance && !PathFindingManager.CollisionLine(Position.get(), s.Position.get()))
                            {
                                BestDistance = d / s.ThreatLevel;
                                CurrentAttackTarget = s;
                            }
                        }
                    }
            }
        }


        protected virtual PlayerShip AISearchForPlayers()
        {
            PlayerShip ReturnPlayer = null;
            float BestDistance = 1000000;

            QuadGrid grid = Parent2DScene.quadGrids.First.Value;

            foreach (Basic2DObject o in grid.Enumerate(getPosition(), new Vector2(BestDistance) * 2))
                if (o.GetType().IsSubclassOf(typeof(PlayerShip)))
                {
                    PlayerShip s = (PlayerShip)o;
                    if (s.GetTeam() == WaveManager.ActiveTeam && !s.Dead && !PathFindingManager.CollisionLine(Position.get(), o.Position.get()))
                        ReturnPlayer = s;
                }

            return ReturnPlayer;
        }

    }
}
