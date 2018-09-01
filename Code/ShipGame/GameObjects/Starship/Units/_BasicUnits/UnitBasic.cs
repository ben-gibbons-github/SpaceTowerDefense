using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class UnitBasic : BasicShipGameObject
    {
        public static Color VirusColor = new Color(0.25f, 1, 0.5f);
        public static int MaxDetectorTime = 250;

        protected string CollisionSound = "";
        protected static float CloakAlphaChangeSpeed = 0.1f;

        private static Matrix NullMatrix = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        private static int CollisionIteration = 0;
        private static bool FieldSound = false;

        const int MaxBuzzTime = 500;
        int BuzzTime = 0;

        public Vector2 Speed;
        private bool UnitAdded = false;
        public List<GunBasic> Guns;
        public float Acceleration = 5f;
        public int DragTime = 0;
        public int MaxDragTime = 200;
        public float RotationSpeed = 0.1f; // Degrees
        public float FreezeTime = 0;
        public AttackType StunState = AttackType.None;
        public int Kills = 0;
        public float Mass = 1;
        public AttackType Resistence = AttackType.Red;
        public AttackType Weakness = AttackType.Red;
        protected Color ShieldColor;
        public float ShieldAlpha;
        public float UnitLevel;

        protected bool IgnoresWalls = false;
        protected float FreezeMult = 1;
        protected float SpeedMult = 1;
        protected float ShipMatrixScale = 0;
        protected int EnergyToGive = 0;
        protected int CellsToGive = 0;
        protected int StunTime = 800;
        protected float WeaknessStunTime = 4000;
        protected int TimesEMPED = 0;
        public BasicShipGameObject LastDamager;
        protected Vector3 RotationOffset;
        protected Vector3 RotationOffsetSpeed;

        protected int ScoreToGive = 10;
        public int WeaponIndexID;

        public UnitBasic(int FactionNumber)
            : base(FactionNumber)
        {
            Load();
            if (InstancerCommit())
                ShipMatrixScale = InstanceManager.AddChild(this);
        }

        protected void CommitToFaction(int FactionNumber)
        {
            this.FactionNumber = FactionNumber;
            if (!UnitAdded)
            {
                FactionManager.AddUnit(this);
                UnitAdded = true;
            }
        }

        public virtual bool InstancerCommit()
        {
            return true;
        }

        public void SetFieldState(FieldState state, int Time)
        {
            if (FieldStateTime < 201)
            {
                if ((FieldStateTime < 1 || state != fieldState) && FieldSound)
                {
                    if (state == FieldState.Cloaked)
                        SoundManager.Play3DSound("UnitCloak", new Vector3(Position.X(), Y, Position.Y()), 0.25f, 800, 2);
                    else
                        SoundManager.Play3DSound("UnitPowerUp", new Vector3(Position.X(), Y, Position.Y()), 0.25f, 800, 2);
                    FieldSound = false;
                }

                this.fieldState = state;
                this.FieldStateTime = Time;
            }
        }

        public virtual void SetLevel(float Level, float Mult)
        {
            if (Guns != null)
                foreach (GunBasic g in Guns)
                    if (g != null)
                        g.SetLevel(Level);
            UnitLevel = Level;
        }

        public virtual void AddReward()
        {
            EnergyToGive = 1;
        }

        public void ShieldFlash(float A)
        {
            if (ShieldAlpha <= 0 && InstancerCommit())
                ShieldInstancer.Add(this);

            ShieldAlpha = A;
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (damage < 0)
                return;

            if (Resistence == attackType)
                damage /= 3f;

            if (Damager != this && Damager != null)
                LastDamager = Damager;

            if (Moveable && Resistence != attackType)
            {
                if (Weakness != attackType)
                {
                    float NewFreezeTime = StunTime;
                    if (attackType != AttackType.Melee)
                        NewFreezeTime /= Mass;

                    if (FreezeTime < NewFreezeTime)
                        FreezeTime = NewFreezeTime;
                }
                else
                {
                    float NewFreezeTime = WeaknessStunTime;
                    if (attackType != AttackType.Melee)
                        NewFreezeTime /= Mass;

                    if (FreezeTime < NewFreezeTime)
                    {
                        StunState = attackType;
                        FreezeTime = NewFreezeTime;
                        damage *= 2;
                    }
                }
                float l = pushSpeed.Length();
                if (l > 0.1f)
                    SetSpeed(Vector2.Normalize(pushSpeed) * (attackType != AttackType.Melee ? 8 : 8 / Mass));
            }

            if (ShieldToughness > 0 && ShieldDamage < ShieldToughness)
            {

                if (attackType != AttackType.Blue && BuzzTime > MaxBuzzTime)
                {
                    SoundManager.Play3DSound("ShieldFlare",
                        new Vector3(Position.X(), Y, Position.Y()),
                        0.35f, 500, 1);
                    BuzzTime = 0;
                }

                ShieldFlash(1);
            }

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }

        public override void Destroy()
        {
            DeathParticles();

            if (InstancerCommit())
            {
                InstanceManager.RemoveChild(this);
                ShieldInstancer.Remove(this);
            }
            if (UnitAdded)
            {
                FactionManager.RemoveUnit(this);
                UnitAdded = false;
            }

            base.Destroy();
        }

        public void VirusParticles()
        {
            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            ParticleManager.CreateParticle(Position3, Rand.V3() * 200, VirusColor, 40, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, VirusColor, Size.X() * (1 + Rand.F()) * 6, 1);
        }

        public virtual void DeathParticles()
        {
            Vector3 Position3 = new Vector3(Position.X(), 0, Position.Y());
            for (int i = 0; i < 4; i++)
                FlamingChunkSystem.AddParticle(Position3, Rand.V3() * Size.X() / 60, new Vector3(0, -0.25f, 0),
                    Rand.V3(), Rand.V3() / 10, 0.375f * Size.X(), 30, new Vector3(1, 0.5f, 0.2f), new Vector3(1, 0.1f, 0.2f), 0, 3);

            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 5, 4);
            for (int i = 0; i < 30; i++)
                ParticleManager.CreateParticle(Position3, Rand.V3() * Size.X(), new Color(1, 0.75f, 0.5f), Size.X() / 2, 5);
            ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.75f, 0.5f), Size.X() * 2, 5);
        }

        public void Add(GunBasic gun)
        {
            if (Guns == null)
                Guns = new List<GunBasic>(1);
            Guns.Add(gun);
            gun.SetParent(this);
        }

        public void SetGun(GunBasic gun)
        {
            if (Guns == null)
            {
                Guns = new List<GunBasic>(1);
                Guns.Add(gun);
            }
            else
                Guns[0] = gun;

            if (gun != null)
                gun.SetParent(this);
        }

        public override void Create()
        {
            AddTag(GameObjectTag.Update);
            AddTag(GameObjectTag._2DForward);
            AddTag(GameObjectTag._2DSolid);
            WorldMatrix = NullMatrix;
            base.Create();
        }

        public Color GetShieldColor()
        {
            return ShieldColor * ShieldAlpha;
        }

        protected GunBasic getGun(int numb)
        {
            return Guns == null ? null : numb < Guns.Count ? Guns[numb] : null;
        }

        protected void AimGun(int numb, float Rotation)
        {
            if (Guns != null && numb < Guns.Count)
                if (Guns[numb] != null)
                    Guns[numb].SetRotation(Rotation);
        }

        protected void FireGun(GameTime gameTime, int numb, int firemode)
        {
             if (Guns != null && numb < Guns.Count && Guns[numb] != null)
                Guns[numb].Fire(gameTime, firemode);
        }

        protected virtual void AutoFire(GameTime gameTime)
        {
            if (Guns != null)
            {
                foreach (GunBasic g in Guns)
                    if (g != null)
                        g.AutoFire(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            FieldSound = true;
            BuzzTime += gameTime.ElapsedGameTime.Milliseconds;
            if (RotationOffsetSpeed != Vector3.Zero)
                RotationOffset += RotationOffsetSpeed;
            else
                RotationOffset -= RotationOffsetSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;

            if (ShieldAlpha > 0)
            {
                ShieldAlpha -= gameTime.ElapsedGameTime.Milliseconds * 0.05f * 60 / 1000f;
                if (ShieldAlpha < 0 && InstancerCommit())
                    ShieldInstancer.Remove(this);
            }

            UpdateShip(gameTime);
            base.Update(gameTime);


            SetQuadGridPosition();
        }

        public override bool CanBeTargeted()
        {
            return base.CanBeTargeted() && (fieldState != FieldState.Cloaked || FieldStateTime < 1);
        }

        public virtual void EMP(BasicShipGameObject Damager, int Level)
        {
            SoundManager.Play3DSound("SnapHit", new Vector3(Position.X(), Y, Position.X()), 0.25f, 800, 2);

            FreezeTime = 5000;
            StunState = AttackType.Blue;
            LastDamager = Damager;
        }

        public virtual void SmallBomb(BasicShipGameObject Damager)
        {
            Damage(10000, 0, Vector2.Zero, Damager, AttackType.Explosion);
            Damage(10000, 0, Vector2.Zero, Damager, AttackType.Explosion);
        }

        public override void Update2(GameTime gameTime)
        {
            if (Dead)
                return;

            if (Moveable && Solid)
                TestCollision(gameTime);

            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            if (ShutDownTime > -1)
                InstanceManager.EmitParticle(GetIntType(), Position3, ref RotationMatrix, 0, Size.X(), (Acceleration + 0.75f) / 2 * 1.5f * (1.75f - ((float)DragTime / MaxDragTime * 1.5f)));
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z) * Matrix.CreateTranslation(Position3);
            if (GetTeam() != NeutralManager.NeutralTeam)
                ParticleManager.CreateRing(Position3, Size.X() * 1.75f * (1 - CloakAlpha), GetTeam());

            if (FreezeTime > 0 && StunState != AttackType.None)
            {
                UpdateEmp();
            }

            base.Update2(gameTime);
        }

        public void TestCollision(GameTime gameTime)
        {
            foreach (Basic2DObject o in Parent2DScene.quadGrids.First.Value.Enumerate(QuadGridXMin, QuadGridYMin, QuadGridXMax, QuadGridYMax))
                if (o != this)
                {
                    if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                    {
                        if (Vector2.Distance(Position.get(), o.Position.get()) < (Size.X() + o.Size.X()) / 2)
                        {
                            BasicShipGameObject s = (BasicShipGameObject)o;
                            Collide(gameTime, s.ReturnCollision());
                            if (Dead)
                                return;
                        }
                    }
                    else
                    {
                        WallNode n = (WallNode)o;

                        if (Vector2.Distance(Position.get(), o.Position.get()) < (Size.X() + o.Size.X()) / 2)
                        {
                            float MoveAmount = (Size.X() + o.getSize().X) / 2 - Vector2.Distance(o.Position.get(), Position.get());

                            if (Vector2.Distance(o.Position.get(), Position.get()) > 0.1f)
                                Position.set(Position.get() + Vector2.Normalize(Position.get() - o.Position.get()) * MoveAmount);
                            else
                                Position.set(Position.get() + Vector2.One * MoveAmount * 2);
                        }
                        if (n.wallConnector != null)
                        {
                            float MoveAmount = (Size.X() + o.getSize().X * 0.75f) / 2 - Logic.DistanceLineSegmentToPoint(n.Position.get(), n.wallConnector.PositionNext, Position.get());

                            if (MoveAmount > 0)
                            {
                                Vector2 MoveVector;
                                if (!n.wallConnector.LineIsVertical)
                                {
                                    if (n.wallConnector.LineSlope == 0)
                                    {
                                        MoveVector = new Vector2(0, -1);
                                        if (Position.Y() > n.wallConnector.LineSlope * Position.X() + n.wallConnector.LineIntercept)
                                            MoveAmount = -MoveAmount;
                                    }
                                    else
                                    {
                                        MoveVector = new Vector2(1, -1 / n.wallConnector.LineSlope);
                                        if (!(Position.Y() > n.wallConnector.LineSlope * Position.X() + n.wallConnector.LineIntercept ^ n.wallConnector.LineSlope > 0))
                                            MoveAmount = -MoveAmount;
                                    }

                                    MoveVector.Normalize();

                                }
                                else
                                {
                                    MoveVector = new Vector2(Position.X() > n.Position.X() ? 1 : -1, 0);
                                }

                                Position.set(Position.get() + MoveVector * MoveAmount);
                            }
                        }
                    }
                }
        }

        public bool TestFree(Vector2 Position, float Angle, float Offset, float Size)
        {
            QuadGrid quad = Parent2DScene.quadGrids.First.Value;

            foreach (Basic2DObject o in quad.Enumerate(Logic.ToVector2(Angle) * Offset, new Vector2(Size  * 2)))
                if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)o;
                    if (Vector2.Distance(Logic.ToVector2(Angle) * Offset, o.Position.get()) < (Size + o.Size.X()) / 2)
                        return false;
                }

            return true;
        }

        public virtual void Collide(GameTime gameTime, BasicShipGameObject Other)
        {
            if (Other == null)
                return;

            if (!Dead && !Other.Dead && !Other.IsAlly(this) && FreezeTime < 0 && Other.GetType().IsSubclassOf(typeof(UnitBasic)))
            {
                UnitBasic u = (UnitBasic)Other;
                if (u.FreezeTime < 0)
                {
                    Vector3 Position3 = new Vector3((Position.X() + Other.Position.X()) / 2, Y, (Position.Y() + Other.Position.Y()) / 2);
                    if (CollisionSound.Equals("") && u.CollisionSound.Equals(""))
                        SoundManager.Play3DSound("CollisionImpact", Position3, 0.5f, 300, 2);
                    else
                    {
                        SoundManager.Play3DSound(CollisionSound.Equals("") ? u.CollisionSound : CollisionSound,
                            Position3, 0.5f, 300, 2);
                    }


                    Damage(Other.fieldState != FieldState.SpeedBoost || Other.FieldStateTime < 1 ? Other.CollisionDamage : Other.CollisionDamage * 1.25f
                        , 5, Position.get() - Other.Position.get(), Other, AttackType.Melee);

                    Other.Damage(fieldState != FieldState.SpeedBoost || FieldStateTime < 1 ? CollisionDamage : CollisionDamage * 1.25f
                        , 5, Other.Position.get() - Position.get(), this, AttackType.Melee);

                    if (!GetType().IsSubclassOf(typeof(UnitBuilding)))
                    {
                        FreezeTime = Math.Max(FreezeTime, Other.CollisionFreezeTime);
                        StunState = AttackType.Melee;
                    }
                    if (!u.GetType().IsSubclassOf(typeof(UnitBuilding)))
                    {
                        u.FreezeTime = Math.Max(u.FreezeTime, CollisionFreezeTime);
                        u.StunState = AttackType.Melee;
                    }
                }
            }

            float MoveAmount = (Size.X() + Other.getSize().X) / 2 - Vector2.Distance(Other.Position.get(), Position.get());
            if (Other.Moveable)
            {
                CollisionIteration++;
                if (CollisionIteration > 10000)
                    CollisionIteration = 0;

                UnitBasic u = (UnitBasic)Other;
                if (Vector2.Distance(u.Position.get(), Position.get()) > 0.1f)
                {
                    u.Push(u.Position.get() + Vector2.Normalize(Other.Position.get() - Position.get()) * MoveAmount / 2, 0);
                    Push(Position.get() + Vector2.Normalize(Position.get() - Other.Position.get()) * MoveAmount / 2, 0);
                }
                else
                {
                    u.Push(u.Position.get() + Vector2.One * MoveAmount, 0);
                    Push(Position.get() - Vector2.One * MoveAmount, 0);
                }
            }
            else
            {
                if (Vector2.Distance(Other.Position.get(), Position.get()) > 0.1f)
                    Push(Position.get() + Vector2.Normalize(Position.get() - Other.Position.get()) * MoveAmount, MoveAmount);
                else
                    Push(Position.get() + Vector2.One * MoveAmount * 2, MoveAmount * 2);
            }
        }

        public float Push(Vector2 TargetPosition, float Forced)
        {
            Position.set(TargetPosition);
            return 0;
            /*

            if (Vector2.Distance(TargetPosition, Position.get()) < 1)
                return 0;

            float ToMove = Vector2.Distance(Position.get(), TargetPosition);

            foreach (BasicShipGameObject s in Parent2DScene.quadGrids.First.Value.Enumerate(TargetPosition,Size.get()))
                if (s != this && s.Solid && Vector2.Distance(TargetPosition, s.getPosition()) < (getSize().X + s.getSize().X) / 2)
                {
                    float MoveAmount = (Size.X() + s.getSize().X) / 2 - Vector2.Distance(s.Position.get(), TargetPosition);
                    if (s.Moveable)
                    {
                        UnitBasic u = (UnitBasic)s;
                        if (u.MyCollisionIteration != CollisionIteration)
                        {
                            u.MyCollisionIteration = CollisionIteration;
                            if (Vector2.Distance(u.Position.get(), Position.get()) > 0.1f)
                                MoveAmount -= u.Push(u.Position.get() + Vector2.Normalize(u.Position.get() - Position.get()) * MoveAmount, Math.Min(Forced, MoveAmount));
                            else
                                MoveAmount -= u.Push(u.Position.get() + Vector2.One * MoveAmount, Math.Min(Forced, MoveAmount));
                        }
                    }
                    if (MoveAmount > 0)
                    {
                        if (MoveAmount > ToMove)
                        {
                            MoveAmount = ToMove;
                            ToMove = 0;
                        }
                        else
                            ToMove -= MoveAmount;
                    }
                }

            if (ToMove < Forced)
                ToMove = Forced;

            if (ToMove > 0)
                Position.add(Vector2.Normalize(TargetPosition - Position.get()) * ToMove);

            return ToMove;
            */
        }

        private void UpdateShip(GameTime gameTime)
        {
            FreezeTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (FreezeTime < 0)
                StunState = AttackType.None;

            if (DragTime < MaxDragTime)
            {
                Position.add(Speed * (MaxDragTime - DragTime) / MaxDragTime * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000);
                DragTime += gameTime.ElapsedGameTime.Milliseconds;
            }

            if (Guns != null && FreezeTime < 0)
                foreach (GunBasic g in Guns)
                    if (g != null)
                        g.Update(gameTime);

        }

        public virtual void UpdateEmp()
        {
            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());

            switch (StunState)
            {
                case AttackType.Blue:
                    //FlareSystem.AddLightning(Position3, new Color(0.25f, 0.25f, 1), 10, Size.X() / 3, 4, 10);
                    ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(0.25f, 0.25f, 1), Size.X() * 10 * Rand.F(), 1);
                    break;
                case AttackType.Red:
                    //FlareSystem.AddLightning(Position3, new Color(1, 0.25f, 0.25f), 10, Size.X() / 3, 4, 10);
                    ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(1, 0.25f, 0.25f), Size.X() * 10 * Rand.F(), 1);
                    break;
                case AttackType.Green:
                    //FlareSystem.AddLightning(Position3, new Color(0.25f, 1, 0.25f), 10, Size.X() / 3, 4, 10);
                    ParticleManager.CreateParticle(Position3, Vector3.Zero, new Color(0.25f, 1, 0.25f), Size.X() * 10 * Rand.F(), 1); 
                    break;
            }
        }

        public void Accelerate(GameTime gameTime, Vector2 Amount)
        {
            Rotation.set(MathHelper.ToDegrees(Logic.Clerp(Rotation.getAsRadians(), Logic.ToAngle(Amount), RotationSpeed * gameTime.ElapsedGameTime.Milliseconds * 60.0f / 1000.0f)));
            SetSpeed(Logic.ToVector2(Rotation.getAsRadians()) * Acceleration * 5 * (FieldStateTime > 0 ? fieldState == FieldState.SpeedBoost ? 1.25f : 0.33f : 1));
            RotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z);
        }

        public void SetSpeed(Vector2 Speed)
        {
            DragTime = 0;

            this.Speed = Speed;
        }

        public override bool CheckCollision(Basic2DObject Tester)
        {
            return Vector2.Distance(getPosition(), Tester.getPosition()) < Tester.getSize().X + getSize().X;
        }

        public override void BlowUp()
        {
            if (ShieldAlpha > 0 && InstancerCommit())
            {
                ShieldInstancer.Remove(this);
                ShieldAlpha = -1;
            }

            Destroy();
            base.BlowUp();
        }

    }
}
