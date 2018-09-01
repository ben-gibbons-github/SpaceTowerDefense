using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BadRabbit.Carrot.WaveFSM;
using Microsoft.Xna.Framework.Audio;

namespace BadRabbit.Carrot
{
    public enum ViewMode
    {
        Ship,
        Admiral
    }

    public class PlayerShip : UnitBasic, WorldViewer3D
    {
        enum TurretSelectionState
        {
            Light,
            Medium,
            Heavy,
            Extra1,
            Extra2,
        }

        #region StaticFields
        public static Texture2D PlayerPointerTexture;
        private static float PointerDistance = 200;
        private static float FloatingCameraMoveMult = 0.075f;
        private static float AdmiralTargetsMoveSpeed = 4f;
        private static float FloatCameraZoomMult = 0.05f;
        private static int MaxRespawnInvTime = 5000;
        private static int MaxInteractionAGlowTime = 200;
        private static float InteractionDistance = 50;
        private static float InteractionXAlphaChange = 0.1f;
        static int MaxHoldRightTriggerTime = 500;
        static int MaxWeaponBoxCenterTime = 3000;
        int HoldRightTriggerTime = 0;

        public static int GetRespawnCost()
        {
            return 200;
        }

        public static Color TeleportColor = new Color(0.5f, 0.75f, 1);
        public static Color TeleportColor2 = new Color(0.25f, 0.5f, 1);

        static Texture2D AimPointerTexture;
        static Texture2D AimBarsTexture;

        static bool Loaded = false;

        static PlayerShip()
        {
            PlayerPointerTexture = AssetManager.Load<Texture2D>("Textures/ShipGam/PlayerPointer");
        }

        #endregion

        const int MaxEngineCycle = 2180;
        int EngineCycle;

        const int MaxShieldBeepCycle = 2000;
        int ShieldBeepCycle;

        public int WeaponBoxCenterTime = 0;
        private int RespawnTime = 0;

        public Camera2D drawCamera = new Camera2D();
        public Camera3D drawCamera3D = new Camera3D();
        public SceneView sceneView;
        public ViewMode viewMode = ViewMode.Ship;
        private int ViewFloatingTime = 0;
        private Vector2 FloatingViewTarget;
        public Vector2 FloatingViewPosition;
        private Vector2 AimPointer;
        public Vector2 AdjustedAimPointer;
        public float PlayerSize = 80;
        public Vector2 ScreenShake = Vector2.Zero;
        public Vector2 ShakeOffset;
        bool OffenseTriggered = false;
        int OffenseTimer = 0;

        public float ViewZoom = 1;
        private float ViewTargetZoom = 1;
        public int InvTime = 0;
        float NoShootTime = 0;

        public SpecialWeapon CurrentSpecialWeapon;
        public ShipAbility shipAbility;
        PlayerUIManager UIManager;
        public BombManager bombManager;
        public float DamageAlpha = 0;

        public BasicShipGameObject InteractionObject;
        private int InteractionTime;
        private bool InteractionAGlow = false;
        private int InteractionAGlowTime = 0;
        private float InteractionXAlpha = 0;
        public PlayerProfile MyProfile;

        public bool PlacedStartingMineralRock = false;
        public bool PlacedStartingTurrets = false;
        private Vector2 StartingTurretsVector = new Vector2(0, 1);
        private Vector2 NormalizedRStickVector = new Vector2(0, 1);
        public MineralRock StartingMineralRock;
        private bool CloakCommited = false;
        PlayerCard LastOffenseCard;

        public bool Attacking = false;
        private bool OutOfBattle = false;
        private bool WorldViewerReady = false;

        public bool IsTeleporting = false;
        private Vector2 TeleportTarget = Vector2.Zero;
        public LinkedList<MiningPlatform> LastPlacedPlatform = new LinkedList<MiningPlatform>();
        public OffenseAbility offenseAbility = null;
        float AimPointerRotation = 0;

        Dictionary<int, float> DamageDictionary = new Dictionary<int, float>();


        SoundEffectInstance SoundInstance;
        bool UnitCommited = false;

        public PlayerShip(int Team, PlayerProfile MyProfile)
            : base(-1)
        {
            if (MyProfile.MyController.GetType().Equals(typeof(AI.StarShipAiController)))
            {
                AI.StarShipAiController s = (AI.StarShipAiController)MyProfile.MyController;
                s.ParentShip = this;
            }

            FactionNumber = FactionManager.Add(new Faction(Team == -1 ? FactionManager.GetFreeTeam() : Team));
            FactionManager.Factions[FactionNumber].Owner = MyProfile;
            if (!UnitCommited)
            {
                FactionManager.AddUnit(this);
                UnitCommited = true;
            }

            Moveable = true;

            this.ThreatLevel = 1;
            
            Add(UnitTag.Player);
            Add(UnitTag.Light);

            if (!CameraFlybyState.BlockPlayerWorldViewer)
                ReadyWorldViewer();

            HullToughness = 1;
            ShieldToughness = 1;

            Acceleration = 1f;
            bombManager = new BombManager(this);
            CollisionDamage = 0;
            CommitToFaction(FactionNumber);
            this.MyProfile = MyProfile;
        }

        public int getSmallBombs()
        {
            return bombManager.BombLauncher.Ammo;
        }

        public void addSmallBomb()
        {
            bombManager.BombLauncher.Ammo++;
            SoundManager.Play3DSound("PlayerGainAbility",
                new Vector3(Position.X(), Y, Position.Y()), 1, 1000, 2);
        }

        public void GetTurretSelection()
        {
            TurretSelectionState state = TurretSelectionState.Light;

            for (int i = 0; i < Faction.MaxCards; i++)
            {
                List<TurretCard> CurrentSelection = new List<TurretCard>();
                foreach (TurretCard card in FactionCard.FactionTurretDeck)
                    {
                        if (state.ToString().Equals(card.StrongVs))
                            CurrentSelection.Add(card);
                        else if ((int)state > 2 && card.StrongVs.Equals(""))
                            CurrentSelection.Add(card);
                    }
                FactionManager.Factions[FactionNumber].Cards.Add(CurrentSelection[Rand.r.Next(CurrentSelection.Count)]);
                state++;
            }

            ReadyMenu();
        }

        public int getBigBombs()
        {
            return bombManager.Bombs.Count;
        }

        public int getUnitCasts()
        {
            if (CurrentSpecialWeapon == null)
                return 0;
            else
            {
                GhostCast g = (GhostCast)CurrentSpecialWeapon;
                return (FactionManager.GetFaction(FactionNumber).Energy / 150);
            }
        }

        public override bool StopsBullet(BasicShipGameObject Other)
        {
            return base.StopsBullet(Other) && !OutOfBattle;
        }

        public void ReadyMenu()
        {
            if (FactionManager.GetFaction(FactionNumber).PickingCards)
            {
                FactionManager.GetFaction(FactionNumber).PickingCards = false;
                ChooseStartState.Activate();
                if (UIManager != null)
                {
                    TurretForm.BuildSelectedTurrets(UIManager.TurretPickerFrame, FactionNumber);
                    UIManager.TurretPickerFrame.DeActivate();
                    PlayerMarker m = (PlayerMarker)UIManager.TurretPickerFrame.TargetMarkers.First.Value;
                    m.UseLTrig = false;
                }
            }
        }

        public void ReadyWorldViewer()
        {
            AddTag(GameObjectTag.WorldViewer);
            AddTag(GameObjectTag._2DOverDraw);
            WorldViewerReady = true;
        }

        public override int GetIntType()
        {
            return 2 + InstanceManager.WorldIndex;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Player");
        }

        public void PlaceTurret(BasicShipGameObject b)
        {
            ParentLevel.AddObject(b);
            Vector2 p = GetPlacePosition(b.Size.X() / 2);
            b.SetPosition(p);


            Vector3 P3 = new Vector3(p.X, 0, p.Y);
            for (int i = 0; i < 10; i++)
                LineParticleSystem.AddParticle(P3, P3 + Rand.V3() * 300, TeamInfo.GetColor(GetTeam()));


            ParentLevel.AddObject(new PathfindingFlare(PathFindingManager.self,
                (int)((p.X - Parent2DScene.MinBoundary.X()) / PathFindingManager.self.Divisor.X),
                (int)((p.Y - Parent2DScene.MinBoundary.Y()) / PathFindingManager.self.Divisor.Y),
                1, 0, TeamInfo.GetColor(GetTeam())));
            ParentLevel.AddObject(new PathfindingFlare(PathFindingManager.self,
                (int)((p.X - Parent2DScene.MinBoundary.X()) / PathFindingManager.self.Divisor.X),
                (int)((p.Y - Parent2DScene.MinBoundary.Y()) / PathFindingManager.self.Divisor.Y),
                -1, 0, TeamInfo.GetColor(GetTeam())));
            ParentLevel.AddObject(new PathfindingFlare(PathFindingManager.self,
                (int)((p.X - Parent2DScene.MinBoundary.X()) / PathFindingManager.self.Divisor.X),
                (int)((p.Y - Parent2DScene.MinBoundary.Y()) / PathFindingManager.self.Divisor.Y),
                0, 1, TeamInfo.GetColor(GetTeam())));
            ParentLevel.AddObject(new PathfindingFlare(PathFindingManager.self,
                (int)((p.X - Parent2DScene.MinBoundary.X()) / PathFindingManager.self.Divisor.X),
                (int)((p.Y - Parent2DScene.MinBoundary.Y()) / PathFindingManager.self.Divisor.Y),
                0, 1, TeamInfo.GetColor(GetTeam())));

            SoundManager.Play3DSound("PlayerBuildTurret", new Vector3(p.X, 0, p.Y), 0.25f, 1000, 2);
        }

        public bool CanPlaceTurret(float Size)
        {
            foreach (GameObject o in ParentScene.Children)
                if (o.GetType().IsSubclassOf(typeof(StaticShipGameObject)) || o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)o;
                    if (Vector2.Distance(GetPlacePosition(Size / 2), s.Position.get()) < (Size + s.Size.X()) / 1.5f)
                        return false;
                    if (s.GetType().IsSubclassOf(typeof(UnitTurret)))
                    {
                        UnitTurret t = (UnitTurret)s;
                        if (!t.IsAlly(this) && Vector2.Distance(GetPlacePosition(Size / 2), s.Position.get()) < (Size + s.Size.X()) / 2 + 200)
                            return false;
                    }
                }
                else if (o.GetType().Equals(typeof(WallNode)))
                {
                    WallNode n = (WallNode)o; 
                    if (n.wallConnector != null)
                    {
                        float MoveAmount = (Size + n.getSize().X) / 2 - Logic.DistanceLineSegmentToPoint(n.Position.get(), n.wallConnector.PositionNext, GetPlacePosition(Size / 2));

                        if (MoveAmount > 0)
                        {
                            return false;
                        }
                    }
                }
            return true;
        }

        public Vector2 GetPlacePosition(float OffsetY)
        {
            if (viewMode == ViewMode.Ship && !Dead)
                return getPosition() + new Vector2(0, Size.Y() / 2 + OffsetY);
            else
                return FloatingViewPosition;
        }

        public Camera3D getCamera()
        {
            return drawCamera3D;
        }

        public Vector2 getViewTL()
        {
            return sceneView.Position;
        }

        public SceneView getSceneView()
        {
            return sceneView;
        }

        public void setSceneView(SceneView sceneView)
        {
            this.sceneView = sceneView;
        }

        public bool Active()
        {
            return true;
        }

        public void SetForDefense()
        {
            if (Attacking || WaveManager.CurrentWave < 2)
            {
                WeaponBoxCenterTime = MaxWeaponBoxCenterTime;
                FactionManager.Factions[FactionNumber].AddEvent("Defending", new Color(0.5f, 0.5f, 1), FactionEvent.DefenseTexture);
            }

            offenseAbility = null;
            RotationSpeed = 0.1f;
            OutOfBattle = false;
            Attacking = false;
            Acceleration = 0.6f;

            HullDamage = 0;
            ShieldDamage = 0;

            HullToughness = 10 + WaveManager.CurrentWave / 2f;
            ShieldToughness = 5 + WaveManager.CurrentWave / 5f;

            GunBasic g = null;
            if (WaveManager.SuperWave)
                g = new PlayerSuperLaserGun();
            else
                g = new PlayerLaserGun();

            if (Guns == null)
                Add(g);
            else
                SetGun(g);

            SetSpecialWeapon(null);
            SetShipAbility(new ShipAbiliyBlink());

            Weakness = AttackType.None;
            Resistence = AttackType.None;

            if (WaveManager.SuperWave)
            {
                HullToughness *= 2;
                ShieldToughness *= 2;
                Acceleration *= 2;
            }

            UnitLevel = WaveManager.CurrentWave / 15f * WaveManager.DifficultyMult;
        }

        public void SetForOffense(PlayerCard Card)
        {
            Attacking = true;

            if (Card != null)
            {
                FactionManager.Factions[FactionNumber].AddEvent("Attacking as " + Card.AttackName, new Color(1, 0.5f, 0.5f), FactionEvent.KillTexture);
                LastOffenseCard = Card;
                WeaponBoxCenterTime = MaxWeaponBoxCenterTime;

                OutOfBattle = false;
                Acceleration = Card.GetAcceleration() * 0.8f;

                HullDamage = 0;
                ShieldDamage = 0;

                HullToughness = ((Card.GetHullToughness() + Card.GetShieldToughness()) / 2f + 7) * (1.5f + WaveManager.CurrentWave / 10f);
                ShieldToughness = 0;

                GunBasic g = null;
                if (WaveManager.SuperWave)
                    g = Card.GetSuperGun();
                else
                    g = Card.GetGun();

                if (Guns == null)
                    Add(g);
                else
                    SetGun(g);

                SetSpecialWeapon(Card.GetWeapon());
                ShipAbiliyBlink b = new ShipAbiliyBlink();
                b.MaxDashTime /= 2;
                SetShipAbility(b);
                offenseAbility = Card.GetOffenseAbility();

                Weakness = Card.GetWeakness();
                Resistence = Card.GetResistance();

                if (WaveManager.SuperWave)
                {
                    HullToughness *= 2;
                    Acceleration *= 2;
                }
            }
            else
            {
                offenseAbility = null;
                OutOfBattle = true;
                Acceleration = 1.5f;
                SetGun(null);
                SetSpecialWeapon(null);
                SetShipAbility(null);
            }
        }

        new public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                AimPointerTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/AimPointer");
                AimBarsTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/AimBars");
            }
        }

        public override void Create()
        {
            Load();
            base.Create();
            PlayerSize = 70;
            Size.set(new Vector2(PlayerSize));

            //if (MyProfile.MyController.IsLocal())
                UIManager = new PlayerUIManager(this);

            ControllerLoader.Load();
            Weakness = AttackType.None;
            Resistence = AttackType.None;
            ShieldColor = new Color(0.5f, 0.5f, 1f);
            SetShipAbility(new ShipAbiliyBlink());
            ViewTargetZoom = 0.5f;
        }

        public override void NewWave()
        {
            //if (WaveManager.GameSpeed < 2 || WaveManager.CurrentWave % 2 > 0)
              //  bombManager.AddSmallBombs();
        }

        public override void NewWaveEvent()
        {
            RespawnTime += 10000;

            if (FactionManager.TeamCount > 1)
            {
                if (WaveManager.ActiveTeam != GetTeam())
                    Teleport(NeutralManager.GetSpawnPosition());
                else
                {
                    while (LastPlacedPlatform.First != null && LastPlacedPlatform.First.Value != null && LastPlacedPlatform.First.Value.Dead)
                        LastPlacedPlatform.Remove(LastPlacedPlatform.First);
                    if (LastPlacedPlatform.Count > 0)
                        Teleport(LastPlacedPlatform.First.Value.Position.get());
                }
            }

            base.NewWaveEvent();
        }

        private void Teleport(Vector2 Target)
        {
            if (!IsTeleporting)
            {
                SoundManager.Play3DSound("PlayerDash",
                    new Vector3(Position.X(), Y, Position.Y()), 0.4f, 300, 2);
            }

            IsTeleporting = true;
            TeleportTarget = Target;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (Weakness != AttackType.Blue)
                return;
            else
                base.EMP(Damager, Level);
        }

        public void AddPosition(Vector2 Position)
        {
            this.Position.add(Position);
        }

        public void SetSpecialWeapon(SpecialWeapon spec)
        {
            CurrentSpecialWeapon = spec;
            if (spec != null)
                CurrentSpecialWeapon.Create(this);
        }

        public override bool CanBeTargeted()
        {
            return (shipAbility == null || shipAbility.ShipCanTakeDamage()) && !OutOfBattle && InvTime < 0;
        }

        public override void Collide(GameTime gameTime, BasicShipGameObject Other)
        {
            if (Other == null || Other.GetType().Equals(typeof(PlayerShip)))
                return;

            if (Other.GetType().IsSubclassOf(typeof(UnitBuilding))  || InvTime > 0 || (shipAbility != null && !shipAbility.ShipCanTakeDamage()))
                CollisionDamage = 0;
            else
                CollisionDamage = 2.5f;

            base.Collide(gameTime, Other);
        }

        private void UpdateFieldState(GameTime gameTime)
        {
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
                    else
                    {
                        int TargetR = 64;
                        int TargetB = 64;

                        if (MyColor.R < TargetR)
                            MyColor.R = (byte)Math.Min(MyColor.R + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, TargetR);
                        else if (MyColor.R > 64)
                            MyColor.R = (byte)Math.Max(MyColor.R - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, TargetR);
                        
                        if (MyColor.G < 64)
                            MyColor.G = (byte)Math.Min(MyColor.G + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);
                        else if (MyColor.G > 64)
                            MyColor.G = (byte)Math.Max(MyColor.G - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);
                        
                        if (MyColor.B < TargetB)
                            MyColor.B = (byte)Math.Min(MyColor.B + gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, TargetB);
                        else if (MyColor.B > 64)
                            MyColor.B = (byte)Math.Max(MyColor.B - gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * CloakAlphaChangeSpeed * 255, 64);
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

        public override void Update(GameTime gameTime)
        {
            AimPointerRotation += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.1f;
            WeaponBoxCenterTime -= gameTime.ElapsedGameTime.Milliseconds;

            DamageAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.05f;
            if (DamageAlpha < 0)
                DamageAlpha = 0;

            CloakAlpha = 0;
            UpdateFieldState(gameTime);

            if (FreezeTime > 400)
                FreezeTime = 400;
            if (VirusTime > 0)
                VirusTime = 0;
            if (NoShootTime > 1000)
                NoShootTime = 1000;

            ShutDownTime -= gameTime.ElapsedGameTime.Milliseconds;
            NoShootTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (ShutDownTime > 50)
                ShutDownTime = 50;

            if (ShutDownTime < 0)
                ShutDownTime = 0;

            if (CameraFlybyState.BlockPlayerWorldViewer)
                return;

            bool TeleportSound = UpdateTeleport(gameTime);
            UpdateCamera(gameTime);
            UpdateController(gameTime);

            if (!Dead)
            {
                if (!TeleportSound)
                    SoundInstance = SoundManager.PlayLoopingSound(SoundInstance, "PlayerEngine2",
                        new Vector3(Position.X(), Y, Position.Y()), Math.Max(0, 0.05f * (MaxDragTime - DragTime) / MaxDragTime), 400, 2);
                else
                    SoundInstance = SoundManager.PlayLoopingSound(SoundInstance, "PlayerEngine2",
                        new Vector3(Position.X(), Y, Position.Y()), Math.Max(0, 0.15f * (MaxDragTime - DragTime) / MaxDragTime), 400, 2);
                
                if (InvTime > -1)
                    InvTime -= gameTime.ElapsedGameTime.Milliseconds;

                Position.set(Vector2.Clamp(Position.get(), Parent2DScene.MinBoundary.get(), Parent2DScene.MaxBoundary.get()));
                base.Update(gameTime);
            }
            else
            {
                if (SoundInstance != null)
                    SoundInstance.Volume = 0;
                UpdateRespawn(gameTime);
            }

            if (UIManager != null)
                UIManager.Update(gameTime);
        }

        private bool UpdateTeleport(GameTime gameTime)
        {
            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());

            if (IsTeleporting)
            {
                IgnoresWalls = true;
                Solid = false;
                float PreviousAccell = Acceleration;
                float PreviousTurn = RotationSpeed;
                Acceleration = 4.5f;

                if (Vector2.Distance(Position.get(), TeleportTarget) < 100)
                {
                    SoundManager.Play3DSound("PlayerDashReverse",
                        new Vector3(Position.X(), Y, Position.Y()), 0.2f, 300, 2);

                    IsTeleporting = false;

                    ParticleManager.CreateParticle(Position3, Vector3.Zero, TeleportColor2, Size.X() * 5, 4);
                    for (int i = 0; i < 30; i++)
                        ParticleManager.CreateParticle(Position3, Rand.V3() * 200, TeleportColor2, 20, 5);
                }
                else
                    Position.set(Position.get() + Vector2.Normalize(TeleportTarget - Position.get()) * Acceleration * 5);

                Acceleration = PreviousAccell;
                RotationSpeed = PreviousTurn;
            }
            else
            {
                Solid = shipAbility == null || shipAbility.ShipIsSolid();
                IgnoresWalls = shipAbility != null && !shipAbility.ShipIsSolid();
            }

            float TargetSize = IgnoresWalls ? PlayerSize / 2 : PlayerSize;
            float SizeChange = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 2;
            if (IgnoresWalls)
                SizeChange *= 2;

            if (Math.Abs(Size.X() - TargetSize) < SizeChange)
                Size.setNoPerform(new Vector2(TargetSize));
            else
            {
                if (Size.X() < TargetSize)
                    Size.setNoPerform(Size.get() + Vector2.One * SizeChange);
                else
                    Size.setNoPerform(Size.get() - Vector2.One * SizeChange);
            }

            if (IgnoresWalls)
            {
                float ParticleSize = PlayerSize;
                ParticleManager.CreateParticle(Position3, Vector3.Zero, TeleportColor, ParticleSize * (1 + Rand.F() / 3) * 2, 1);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, TeleportColor, ParticleSize * (1 + Rand.F() / 3) * 4, 1);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, TeleportColor, ParticleSize * (1 + Rand.F() / 3) * 8, 1);
                ParticleManager.CreateParticle(Position3, Vector3.Zero, TeleportColor2, ParticleSize * (1 + Rand.F() / 4) * 3, 0);
            }
            return IsTeleporting;
        }

        private void UpdatePickEnemies(GameTime gameTime, BasicController MyController)
        {
            Vector2 MenuStick = MyController.MenuStick(false, false, true, false, false);
            if (Math.Abs(MenuStick.X) > 0.15f)
            {
                if (MenuStick.X > 0)
                    OverCardPicker.TeamMoveRight(GetTeam());
                else
                    OverCardPicker.TeamMoveLeft(GetTeam());
            }

            if (MyController.AButton() && !MyController.AButtonPrevious())
                OverCardPicker.ReadyTeam(GetTeam());
        }

        public void SetCameraTarget(Vector2 Target, int Time)
        {
            if (Target == Vector2.Zero)
                return;

            FloatingViewTarget = Target;
            ViewFloatingTime = Time;
        }

        protected void SetShipAbility(ShipAbility a)
        {
            if (a != null)
                a.Create(this);
            shipAbility = a;
        }

        private void UpdateHUD(GameTime gameTime, BasicController MyController, bool MenuOpen)
        {
            if (MyController.YButton() && !MyController.YButtonPrevious())
            {
                viewMode = viewMode == ViewMode.Admiral ? ViewMode.Ship : ViewMode.Admiral;
                InteractionObject = null;

                if (viewMode == ViewMode.Admiral)
                {
                    ViewTargetZoom = 0.25f;
                    if (UIManager != null)
                        UIManager.AddParticle(new UIZoomParticle(1));
                }
                else
                {
                    ViewTargetZoom = 0.5f;
                    if (UIManager != null)
                        UIManager.AddParticle(new UIZoomParticle(0));
                }
            }

            if (UIManager != null && (((MyController.XButton() && !MyController.XButtonPrevious()) ||
                (MyController.BButton() && !MyController.BButtonPrevious())) ||
                FactionManager.Factions[FactionNumber].PickingCards))
            {
                if (UIManager.TurretPickerFrame.Active && !FactionManager.Factions[FactionNumber].PickingCards)
                    UIManager.TurretPickerFrame.DeActivate();
                else if (InteractionObject == null)
                    UIManager.TurretPickerFrame.Cycle("AllTurrets");
            }
                //UIManager.MenuManager.SetMenu(UIManager.MenuManager.TechTree, sceneView.Size / 2);

            /*
            if (MyController.DPadUp() && !MyController.DPadUpPrevious() && ViewTargetZoom < 1)
            {
                ViewTargetZoom *= 2;
                UIManager.AddParticle(new UIZoomParticle(1));
            }
            if (MyController.DPadDown() && !MyController.DPadDownPrevious() && ViewTargetZoom > 0.35f)
            {
                ViewTargetZoom /= 2;
                UIManager.AddParticle(new UIZoomParticle(0));
            }
            */
        }

        private void UpdateCamera(GameTime gameTime)
        {
            if (PlacedStartingTurrets && PlacedStartingMineralRock)
            {
                if (ViewFloatingTime > 0 || viewMode == ViewMode.Admiral)
                {
                    ViewFloatingTime -= gameTime.ElapsedGameTime.Milliseconds;

                    if (Vector2.Distance(FloatingViewTarget, FloatingViewPosition) < 1)
                        ViewFloatingTime = 0;
                }
                else
                    FloatingViewTarget = Position.get();
            }
            else
                foreach (UnitBasic b in FactionManager.SortedUnits[GetTeam()])
                    if (b.GetType().IsSubclassOf(typeof(MiningPlatform)))
                        FloatingViewTarget = b.Position.get();


            FloatingViewPosition += (FloatingViewTarget - FloatingViewPosition) * FloatingCameraMoveMult * gameTime.ElapsedGameTime.Milliseconds / 1000f * 60f;
            ViewZoom += (ViewTargetZoom - ViewZoom) * FloatCameraZoomMult * gameTime.ElapsedGameTime.Milliseconds / 1000f * 60f;
            ScreenShake *= 1 - gameTime.ElapsedGameTime.Milliseconds / 500f;

            if (WorldViewerReady && sceneView != null)
            {
                drawCamera.SetPosition(FloatingViewPosition);
                drawCamera.SetZoom(ViewZoom);

                ShakeOffset = Vector2.Zero;

                drawCamera3D.SetLookAt(new Vector3(FloatingViewPosition.X, Y + 1000000 / sceneView.Size.Length() / ViewZoom, FloatingViewPosition.Y),
                    new Vector3(FloatingViewPosition.X + ShakeOffset.X, Y, FloatingViewPosition.Y + ShakeOffset.Y), new Vector3(0, 0, -1));

                ShakeOffset = ScreenShake * (0.5f - Rand.F());
            }
        }

        private void UpdateRespawn(GameTime gameTime)
        {
            RespawnTime += gameTime.ElapsedGameTime.Milliseconds * 4;
            if (!OutOfBattle && RespawnTime >= Settings.PlayerRespawnTime)
                Revive();
            if (MyProfile.MyController.LeftTrigger() && OutOfBattle)
            {
                HoldRightTriggerTime += gameTime.ElapsedGameTime.Milliseconds;
                if (HoldRightTriggerTime > MaxHoldRightTriggerTime && FactionManager.CanAfford(FactionNumber,0 , GetRespawnCost()))
                {
                    FactionManager.AddEnergy(FactionNumber, -GetRespawnCost());
                    HoldRightTriggerTime = 0;
                    OutOfBattle = false;
                    Revive();
                    SetForOffense(LastOffenseCard);
                }
            }
        }

        private void UpdateController(GameTime gameTime)
        {
            BasicController MyController = MyProfile.MyController;
            if (MyController == null)
                return;

            if (UIManager != null && MyProfile.MyController.StartButton() && !MyProfile.MyController.StartButtonPrevious())
            {
                if (UIManager.MenuFrame.Active)
                    UIManager.MenuFrame.DeActivate();
                else
                {
                    UIManager.MenuFrame.Cycle("PauseMenu");
                    UIManager.TurretPickerFrame.DeActivate();
                }
            }

            if (UIManager != null && UIManager.MenuFrame.Active)
                return;

            bool PickingEnemies = false;

            if (WaveManager.GetState() == PickEnemyState.self && !OverCardPicker.TeamIsReady(GetTeam()) && WaveManager.CurrentWave > PickEnemyState.RandomRounds)
            {
                UpdatePickEnemies(gameTime, MyController);
                PickingEnemies = true;
            }

            if (PlacedStartingMineralRock && !PlacedStartingTurrets)
            {
                UpdateStartingTurrets(gameTime, MyController);
                return;
            }

            bool MenuOpen = false;

            if (!PickingEnemies)
            {
                MenuOpen = UIManager == null || UIManager.TurretPickerFrame.Active;
                if (!MenuOpen)
                {
                    if (viewMode == ViewMode.Admiral)
                        ParticleManager.CreateRing(new Vector3(FloatingViewPosition.X, Y, FloatingViewPosition.Y), 100, GetTeam());
                }
                else if (FactionManager.GetFaction(FactionNumber).PickingCards)
                    return;
            }

            UpdateAbilities(gameTime, MyController);
            /*
            if (!Dead)
            {
                //ShieldBeepCycle += gameTime.ElapsedGameTime.Milliseconds;
                if (ShieldBeepCycle > MaxShieldBeepCycle)
                {
                    ShieldBeepCycle = 0;

                    SoundManager.Play3DSound("ShieldRecharge",
                        new Vector3(Position.X(), Y, Position.Y()), 1, );
                }
            }
            */
            if (viewMode == ViewMode.Ship && !Dead)
                UpdatePlayerShip(gameTime, MyController, MenuOpen);
            else
                UpdateAdmiralMode(gameTime, MyController);

            UpdateHUD(gameTime, MyController, MenuOpen);
        }

        private void UpdateStartingTurrets(GameTime gameTime, BasicController MyController)
        {
            if (MyController.LeftStick().Length() > 0.1f)
                StartingTurretsVector = Vector2.Normalize(MyController.LeftStick()) * new Vector2(1, -1);

            if (StartingTurretsVector.Length() > 0.1f)
            {
                Vector2 PlacePos1 = StartingMineralRock.Position.get() + StartingTurretsVector * Settings.StartingTurretDistance;
                Vector2 PlacePos2 = StartingMineralRock.Position.get() - StartingTurretsVector * Settings.StartingTurretDistance;

                ParticleManager.CreateRing(new Vector3(PlacePos1.X, 0, PlacePos1.Y), 100, GetTeam());
                ParticleManager.CreateRing(new Vector3(PlacePos2.X, 0, PlacePos2.Y), 100, GetTeam());

                if (MyController.AButton() && !MyController.AButtonPrevious())
                {
                    PlasmaTurret p;
                    ParentLevel.AddObject(p = new PlasmaTurret(FactionNumber));
                    p.Position.set(PlacePos1);
                    p.SetAsStarting();

                    ParentLevel.AddObject(p = new PlasmaTurret(FactionNumber));
                    p.Position.set(PlacePos2);
                    p.SetAsStarting();

                    PlacedStartingTurrets = true;
                    if (UIManager != null)
                        UIManager.TurretPickerFrame.Cycle("AllTurrets");

                    SoundManager.Play3DSound("PlayerBuildTurret", new Vector3(Position.X(), 0, Position.Y()), 0.25f, 1000, 2);
                }
            }
        }

        private void UpdateAdmiralMode(GameTime gameTime, BasicController MyController)
        {
            Vector2 LeftStick = MyController.LeftStick();

            //Move
            if (Math.Abs(LeftStick.X) > 0.05f || Math.Abs(LeftStick.Y) > 0.05f)
            {
                LeftStick.Y = -LeftStick.Y;
                FloatingViewTarget += LeftStick * AdmiralTargetsMoveSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f * 60f / ViewZoom;
            }

            if (UIManager == null || !UIManager.TurretPickerFrame.Active)
                UpdateInteractionObject(MyController, gameTime);
            else
                InteractionObject = null;
        }

        private void AimPlayerPointer(GameTime gameTime, BasicController MyController, bool CanShoot)
        {
            if (sceneView == null)
                return;

            if ((shipAbility != null && !shipAbility.ShipCanShoot()) ||
                (CurrentSpecialWeapon != null && !CurrentSpecialWeapon.ShipCanMove()) ||
                OffenseTriggered || NoShootTime > 0 || !WaveStepState.WeaponsFree)
                CanShoot = false;

#if WINDOWS
            if (MyController.IsKeyboardController())
            {
                Basic2DScene s = (Basic2DScene)ParentScene;
                AimPointer = (MouseManager.MousePosition - sceneView.Position - sceneView.Size / 2) / drawCamera.getZoom() / 1.5f + drawCamera.getPosition();
                AdjustedAimPointer = AimHelp(AimPointer);
                AimGun(0, Logic.ToAngle(AdjustedAimPointer - Position.get()));

                if (MyController.RightTrigger() && CanShoot)
                    FireGun(gameTime, 0, 0);
            }
            else
#endif
            {
                Vector2 RightStick = MyController.RightStick();

                if (RightStick.Length() > 0.1f)
                {
                    NormalizedRStickVector = Vector2.Normalize(RightStick);
                }

                AimPointer = NormalizedRStickVector * PointerDistance * new Vector2(1, -1) + Position.get();
                AdjustedAimPointer = AimHelp(AimPointer);
                AimGun(0, Logic.ToAngle(AdjustedAimPointer - Position.get()));

                if (RightStick.Length() > 0.1f && CanShoot)
                    FireGun(gameTime, 0, 0);
            }
        }

        private Vector2 AimHelp(Vector2 AimPointer)
        {
            QuadGrid quad = Parent2DScene.quadGrids.First.Value;

            UnitBasic BestUnit = null;
            float MaxDistance = 400;
            float BestDistance = MaxDistance;

            foreach (Basic2DObject o in quad.Enumerate(AimPointer, new Vector2(MaxDistance * 2)))
                if (o.GetType().IsSubclassOf(typeof(UnitBasic)) && !o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                {
                    UnitBasic s = (UnitBasic)o;
                    if (!s.IsAlly(this) && s.CanBeTargeted())
                    {
                        float dist = Vector2.Distance(AimPointer, o.Position.get()) / 2 + Vector2.Distance(Position.get(), o.Position.get());
                        if (dist < BestDistance)
                        {
                            BestDistance = dist;
                            BestUnit = s;
                        }
                    }
                }

            if (BestUnit != null)
                return BestUnit.Position.get();
            else
                return AimPointer;
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if ((shipAbility == null || shipAbility.ShipCanTakeDamage()) && (InvTime < 0 || attackType == AttackType.Melee) && !IsTeleporting)
            {
                bool OldDead = Dead;
                float OldShields = Math.Min(ShieldDamage, ShieldToughness);
                float OldHull = Math.Min(HullDamage, HullToughness);

                ShakeScreen(Math.Min(damage * 5, 20));
                DamageAlpha = Math.Min(4, damage * 2);

                base.Damage(damage, pushTime, pushSpeed, Damager, attackType);

                if (InvTime > 0)
                {
                    ShieldDamage = OldShields;
                    HullDamage = OldHull;
                    ScreenShake = Vector2.Zero;
                    DamageAlpha = 0;
                }
                else
                {
                    float DamageDone = Math.Min(ShieldDamage, ShieldToughness) - OldShields +
                        Math.Min(HullDamage, HullToughness) - OldHull;
                    if (DamageDone > 0)
                    {
                        int DamagerFaction = Damager.FactionNumber;
                        if (DamagerFaction > -1 && DamagerFaction != NeutralManager.NeutralFaction)
                        {
                            if (DamageDictionary.ContainsKey(DamagerFaction))
                                DamageDictionary[DamagerFaction] += DamageDone;
                            else
                                DamageDictionary.Add(DamagerFaction, DamageDone);
                        }
                    }
                }

                FreezeTime = damage * 300;
                NoShootTime = damage * 500;
                if (!OldDead && Dead)
                {
                    int FirstDamagingFaction = -1;
                    int SecondDamagingFaction = -1;

                    float BestDamage = 0;
                    foreach (int i in DamageDictionary.Keys)
                    {
                        if (DamageDictionary[i] > BestDamage)
                        {
                            BestDamage = DamageDictionary[i];
                            SecondDamagingFaction = FirstDamagingFaction;
                            FirstDamagingFaction = i;
                        }
                    }

                    FactionManager.Factions[FactionNumber].roundReport.Deaths++;

                    if (FirstDamagingFaction != -1)
                    {
                        if (FactionManager.Factions[FactionNumber].Owner != null)
                            FactionManager.Factions[FactionNumber].AddEvent("Killed by " + FactionManager.Factions[FactionNumber].Owner.PlayerName, new Color(1, 0.5f, 0.5f), FactionEvent.DeathTexture);
                        else
                            FactionManager.Factions[FactionNumber].AddEvent("Killed", new Color(1, 0.5f, 0.5f), FactionEvent.DeathTexture);

                        FactionManager.Factions[FirstDamagingFaction].AddEvent("You killed " + MyProfile.PlayerName, new Color(1, 0.5f, 0.5f), FactionEvent.KillTexture);
                        FactionManager.Factions[FirstDamagingFaction].roundReport.PlayerKills++;

                        if (!Attacking)
                        {
                            TextParticleSystem.AddParticle(new Vector3(Position.X(), Y, Position.Y()), "50", (byte)LastDamager.GetTeam(), TextParticleSystemIcons.EnergyTexture);
                            FactionManager.AddEnergy(FirstDamagingFaction, 50);
                            FactionManager.AddScore(FirstDamagingFaction, 250);
                        }
                        if (SecondDamagingFaction != -1)
                        {
                            if (!Attacking)
                            {
                                FactionManager.AddEnergy(SecondDamagingFaction, 25);
                                FactionManager.AddScore(SecondDamagingFaction, 125);
                            }
                            FactionManager.Factions[SecondDamagingFaction].roundReport.PlayerAssists++;
                            FactionManager.Factions[SecondDamagingFaction].AddEvent("Assist");
                        }
                    }
                    else
                        FactionManager.Factions[FactionNumber].AddEvent("Killed", new Color(1, 0.5f, 0.5f), FactionEvent.DeathTexture);
                    
                    DamageDictionary.Clear();
                }

                if (FreezeTime > 0 && OffenseTriggered)
                {
                    ShakeScreen(50);
                    FreezeTime = 500;
                    OffenseTriggered = false;
                }
            }
        }

        public void ShakeScreen(float Value)
        {
            if (Value > ScreenShake.Length())
            {
                ScreenShake = Rand.NV2() * Value;
            }
        }

        private void UpdatePlayerShip(GameTime gameTime, BasicController MyController, bool MenuOpen)
        {
            if (VirusTime < 1 && shipAbility != null && ((MyController.LeftBumper() && !MyController.LeftBumperPrevious()) || (!MenuOpen && MyController.AButton() && !MyController.AButtonPrevious())))
                shipAbility.Trigger();

            if (VirusTime < 1 && FreezeTime < 1 && !IsTeleporting)
            {
                if ((shipAbility == null || shipAbility.ShipCanMove()) && ShutDownTime < 1)
                {
                    AimPlayerPointer(gameTime, MyController, true);

                    Vector2 LeftStick = MyController.LeftStick();

                    if ((Math.Abs(LeftStick.X) > 0.05f || Math.Abs(LeftStick.Y) > 0.05f)
                        && ((CurrentSpecialWeapon == null || CurrentSpecialWeapon.ShipCanMove()) && (!OffenseTriggered)))
                    {
                        EngineCycle += gameTime.ElapsedGameTime.Milliseconds;
                        if (EngineCycle > MaxEngineCycle)
                        {
                            EngineCycle = 0;
                            //SoundManager.Play3DSound("PlayerDashReverse",
                              //  new Vector3(Position.X(), Y, Position.Y())
                                //, new Vector3(LeftStick.X, 0, LeftStick.Y) * Acceleration, 1);
                        }

                        LeftStick.Y = -LeftStick.Y;
                        Accelerate(gameTime, LeftStick);
                    }

                    if (UIManager == null || !UIManager.TurretPickerFrame.Active)
                        UpdateInteractionObject(MyController, gameTime);
                    else
                        InteractionObject = null;
                }
            }
            else
                AimPlayerPointer(gameTime, MyController, false);
        }

        private void UpdateAbilities(GameTime gameTime, BasicController MyController)
        {
            if (CurrentSpecialWeapon != null)
                CurrentSpecialWeapon.Update(gameTime);
            if (shipAbility != null)
                shipAbility.Update(gameTime, MyController);
            if (shipAbility != null && !shipAbility.ShipCanShoot())
                return;
            if (WaveStepState.WeaponsFree && GetTeam() == WaveManager.ActiveTeam)
                bombManager.Update(gameTime, MyController);

            if (offenseAbility != null)
            {
                if (!OffenseTriggered)
                {
                    if (MyController.LeftTrigger() && !MyController.LeftTriggerPrevious() && GetOffenseProgress() == 0)
                    {
                        OffenseTimer = 0;
                        OffenseTriggered = true;
                    }
                }
                else
                {
                    OffenseTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (OffenseTimer > offenseAbility.GetTriggerTime())
                    {
                        OffenseTriggered = false;
                        if (offenseAbility.Trigger(this))
                            offenseAbility = null;
                    }
                }
            }

            if (CurrentSpecialWeapon != null && MyController.RightTrigger() && !MyController.RightTriggerPrevious() && GetOffenseProgress() == 0)
                CurrentSpecialWeapon.Trigger();
        }

        private void UpdateInteractionObject(BasicController MyController, GameTime gameTime)
        {
            Vector2 InterationPosition = viewMode == ViewMode.Ship && !Dead ? Position.get() : FloatingViewPosition;

            if (InteractionObject != null)
            {
                if (Vector2.Distance(InterationPosition, InteractionObject.Position.get()) - 
                    (Size.X() + InteractionObject.Size.X()) / 2 > InteractionDistance || 
                    (InteractionObject.GetType().IsSubclassOf(typeof(MiningPlatform)) && InteractionObject.Dead))
                    InteractionObject = null;
                else
                {
                    InteractionXAlpha -= InteractionXAlphaChange / 4;

                    if (MyController.XButton())
                    {
                        if (InteractionObject.CanInteract(this))
                        {
                            if (!MyController.XButtonPrevious())
                                SoundManager.Play3DSound("PlayerInteract",
                                    new Vector3(InterationPosition.X, Y, InterationPosition.Y), 0.2f, 300, 1);

                            InteractionTime += gameTime.ElapsedGameTime.Milliseconds;
                            if (InteractionTime > InteractionObject.getMaxInteractionTime())
                            {
                                InteractionObject.Interact(this);
                                InteractionObject = null;
                            }
                        }
                        else if (!MyController.XButtonPrevious())
                        {
                            InteractionTime = 0;
                            InteractionXAlpha = 1;
                        }
                    }
                    else
                    {
                        InteractionTime = 0;
                        InteractionAGlowTime += gameTime.ElapsedGameTime.Milliseconds;
                        if (InteractionAGlowTime > MaxInteractionAGlowTime)
                        {
                            InteractionAGlowTime = 0;
                            InteractionAGlow = !InteractionAGlow;
                        }
                    }
                }
            }
            else
            {
                InteractionTime = 0;
                InteractionXAlpha = 0;
                
                float BestDistance = InteractionDistance + Size.X() / 2;

                foreach (GameObject g in Parent2DScene.Children)
                    if (g.GetType().IsSubclassOf(typeof(BasicShipGameObject)) && g != this)
                    {
                        BasicShipGameObject s = (BasicShipGameObject)g;
                        float d = Vector2.Distance(InterationPosition, s.Position.get()) - s.Size.X() / 2;
                        if (d < BestDistance)
                        {
                            BasicShipGameObject m = (BasicShipGameObject)g;
                            if (m.AllowInteract(this))
                            {
                                BestDistance = d;
                                InteractionObject = m;
                            }
                        }
                    }
            }
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (DrawTag == GameObjectTag._2DForward)
            {
                base.Draw2D(DrawTag);
            }
            else
            {
                if (Render.CurrentWorldViewer3D != this)
                    return;

                if (InteractionObject != null)
                {
                    InteractionObject.DrawXButton(InteractionAGlow, (float)InteractionTime / InteractionObject.getMaxInteractionTime(), InteractionXAlpha);
                }

                Vector3 Position3 = Game1.graphicsDevice.Viewport.Project(
                    new Vector3(this.Position.X(), Y, this.Position.Y()), StarshipScene.CurrentCamera.ProjectionMatrix,
                    StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);
                Vector3 OutsidePosition3 = Game1.graphicsDevice.Viewport.Project(
                    new Vector3(this.Position.X() + this.Size.X() * 2, Y, this.Position.Y()), StarshipScene.CurrentCamera.ProjectionMatrix,
                    StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);

                Vector2 Position2 = new Vector2(Position3.X, Position3.Y) - Render.CurrentView.Position;
                Vector2 OutsidePosition2 = new Vector2(OutsidePosition3.X, OutsidePosition3.Y) - Render.CurrentView.Position;

                float ProjectedSize = Vector2.Distance(Position2, OutsidePosition2);

                if (!Dead && (shipAbility == null || shipAbility.ShipCanShoot()))
                {
                    Vector3 AimPos3 = Game1.graphicsDevice.Viewport.Project(new Vector3(AimPointer.X, Y, AimPointer.Y), StarshipScene.CurrentCamera.ProjectionMatrix, StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);
                    Vector2 AimPos = new Vector2(AimPos3.X, AimPos3.Y) - Render.CurrentView.Position;
                    Vector2 AimerSize = new Vector2(ProjectedSize / 2);
                    Render.DrawSprite(AimPointerTexture, AimPos, AimerSize, AimPointerRotation, TeamInfo.HudColors[GetTeam()]);
                    Render.DrawSquare(Position2, AimPos, 80, AimBarsTexture, TeamInfo.HudColors[GetTeam()]);
                }

                if (StarshipScene.DrawingShip != this)
                {
                    if (Vector2.Distance(Render.CurrentView.Size / 2, Position2) > Render.CurrentView.Size.Y * 0.4f)
                    {
                        Vector2 NewPosition = Vector2.Normalize(Position2 - Render.CurrentView.Size / 2) * (Render.CurrentView.Size.Y * 0.4f) + Render.CurrentView.Size / 2;
                        Render.DrawSprite(PlayerPointerTexture, NewPosition, new Vector2(ProjectedSize), 0, TeamInfo.GetColor(GetTeam()));
                    }
                }
                if (UIManager != null)
                    UIManager.Draw(drawCamera3D);
            }
        }

        public float GetOffenseProgress()
        {
            if (offenseAbility != null && OffenseTriggered)
                return (float)OffenseTimer / offenseAbility.GetTriggerTime();
            else if (CurrentSpecialWeapon != null)
                return CurrentSpecialWeapon.GetProgress();
            else 
                return 0;
        }

        public override void DrawFromMiniMap(Vector2 Position, float Size, Vector2 Min, Vector2 Max)
        {
            drawCamera.SetSize(drawCamera3D.Size / 2);
            Vector2 Pos1 = (drawCamera.getTopLeftCorner() - Min) /
                (Max - Min) * Size + Position;
            Vector2 Pos2 = (drawCamera.getBottomRightCorner() - Min) /
                (Max - Min) * Size + Position;
            //Pos1 = new Vector2(MathHelper.Clamp(Pos1.X, Position.X, Position.X + Size), MathHelper.Clamp(Pos1.Y, Position.Y, Position.Y + Size));
            //Pos2 = new Vector2(MathHelper.Clamp(Pos2.X, Position.X, Position.X + Size), MathHelper.Clamp(Pos2.Y, Position.Y, Position.Y + Size));


            Render.DrawOutlineRect(
                Pos1
                ,
                Pos2
                , 1.5f, TeamInfo.Colors[GetTeam()]);


            if (this.Position.X() > Max.X || this.Position.Y() > Max.Y || this.Position.X() < Min.X || this.Position.Y() < Min.Y)
                return;

            Vector2 MapPosition = (this.Position.get() - Min) /
                (Max - Min) * Size + Position;

            Render.DrawSprite(Render.BlankTexture, MapPosition - new Vector2(2), new Vector2(4), 0, TeamInfo.GetColor(GetTeam()));
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
                FactionManager.RemoveUnit(this);
                UnitCommited = false;
            }

            base.Destroy();
        }

        public override void BlowUp()
        {
            if (Dead)
                return;

            SoundManager.PlaySound("PlayerDie",
                0.5f, 0, 0);
            SoundManager.DeafTone();


            ShakeScreen(100);
            VirusTime = 0;

            if (ShieldAlpha > 0)
            {
                ShieldAlpha = 0;
                ShieldInstancer.Remove(this);
            }

            if (Attacking)
            {
                SetForOffense(null);
                OutOfBattle = true;
            }

            DeathParticles();

            RespawnTime = 0;
            Dead = true;
            FreezeTime = 0;
            RemoveTag(GameObjectTag._2DSolid);

            MiningPlatform BestPlatform = null;
            float BestDistance = 10000;
            foreach(MiningPlatform m in Parent2DScene.Enumerate(typeof(MiningPlatform)))
                if (m.GetTeam() == GetTeam() && !m.Dead)
                {
                    float d = Vector2.Distance(Position.get(), m.Position.get());
                    if (d < BestDistance)
                    {
                        BestDistance = d;
                        BestPlatform = m;
                    }
                }

            if (BestPlatform != null)
                Teleport(BestPlatform.Position.get());

            InstanceManager.RemoveChild(this);
        }

        public override void DeathParticles()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 6; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3(), new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 25, 30, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 5, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * 200, new Color(1, 0.75f, 0.5f), 20, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 2, 5);
        }

        private void Revive()
        {
            InstanceManager.AddChild(this);
            InvTime = MaxRespawnInvTime;
            Dead = false;
            AddTag(GameObjectTag._2DSolid);
            HullDamage = 0;
            ShieldDamage = 0;
            FreezeTime = 0;

            if (IsTeleporting)
            {
                Position.set(TeleportTarget);
                IsTeleporting = false;
            }
        }
    }
}
