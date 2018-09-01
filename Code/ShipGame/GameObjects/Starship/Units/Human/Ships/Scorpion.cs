using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Scorpion : UnitShip
    {
        private static Color LaserColor = new Color(1, 0.4f, 0.4f);
        private const int MaxShots = 2;
        private const float LaserStartSize = 50;
        private const float LaserEndSize = 100;
        private const int ShotTime = 300;
        private const int AttackFreezeTime = 500;
        private const float AttackLineWidth = 32;
        private const float LaserDamage = 6;

        float RevolutionSpeed;

        int MaxChargeTime = 1000;
        int CommitTime = 500;

        private int ChargeTime = 0;
        bool Commited = false;
        Vector2 AttackPosition;
        int Shots = 2;

        public Scorpion(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "HeavyHumanExplode";
            DeathVolume = 1;
            DeathDistance = 1200;
            DeathExponenent = 1.5f;

            CollisionSound = "HeavyImpact";
            HullToughness = 10;
            ShieldToughness = 10;
            Acceleration = 0.125f;
            Add(UnitTag.Heavy);
            Add(UnitTag.Human);
            MaxEngagementDistance = 400;
            MinEngagementDistance = 300;
            Shots = MaxShots;
            Resistence = AttackType.Red;
            Weakness = AttackType.Green;
            Mass = 10;
            ScoreToGive = 25;
        }

        public override int GetUnitWeight()
        {
            return 2;
        }

        public override void SetLevel(float Level, float Mult)
        {
            base.SetLevel(Level, Mult);
            CollisionDamage = 5 * Level;
            HullToughness = 3 + Level;
            ShieldToughness = 3 + Level;
            Acceleration = 0.2f;
            Add(UnitTag.Heavy);
            MaxChargeTime = (int)(1200 - 200 * Level);
        }

        public override void Create()
        {
            base.Create();
            ShieldColor = ShieldInstancer.RedShield;
            Size.set(new Vector2(90));
        }

        public override void Update2(GameTime gameTime)
        {
            TestCollision(gameTime);

            Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
            InstanceManager.EmitParticle(GetIntType(), Position3, ref RotationMatrix, 0, Size.X(), 1);
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z) * Matrix.CreateTranslation(Position3);
            if (GetTeam() != NeutralManager.NeutralTeam)
                ParticleManager.CreateRing(Position3, Size.X() * 1.75f * (1 - CloakAlpha), GetTeam());

            UpdateEmp();
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            if (TimesEMPED == 0)
            {
                FreezeTime = 1000 - 250 * UnitLevel + 1000 * Level;
                ChargeTime = 0;
                StunState = AttackType.Blue;
                LastDamager = Damager;
                TimesEMPED++;
            }
        }

        protected override void AI(GameTime gameTime)
        {
            float TargetRevolutionSpeed = 0.01f;

            if (CurrentAttackTarget != null && CurrentAttackTarget.CanBeTargeted() && 
                (Vector2.Distance(Position.get(), CurrentAttackTarget.Position.get()) < GetEngagementDistance() || Commited))
            {
                if (ChargeTime > MaxChargeTime / 2)
                    TargetRevolutionSpeed = 0.1f;

                ChargeTime += gameTime.ElapsedGameTime.Milliseconds;
                float Alpha = ChargeTime / (float)MaxChargeTime;

                Vector3 Position3 = new Vector3(Position.X(), Y, Position.Y());
                ParticleManager.CreateParticle(Position3, Vector3.Zero, LaserColor * Alpha, LaserStartSize + LaserEndSize * Alpha * 8, 1);
                FlareSystem.AddLightning(Position3, LaserColor * Alpha, 40, LaserStartSize + LaserEndSize * Alpha / 4, 3, 15);

                if (!Commited && ChargeTime > CommitTime)
                {
                    Commited = true;
                    AttackPosition = CurrentAttackTarget.Position.get();
                    AttackPosition = Position.get() + Vector2.Normalize(AttackPosition - Position.get()) * GetEngagementDistance();
                }

                if (CurrentAttackTarget != null)
                {
                    Rotation.set(MathHelper.ToDegrees(Logic.Clerp(Rotation.getAsRadians(), Logic.ToAngle(CurrentAttackTarget.Position.get() - Position.get()), RotationSpeed * gameTime.ElapsedGameTime.Milliseconds * 60.0f / 1000.0f)));
                    RotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.getAsRadians() + RotationOffset.X, RotationOffset.Y, RotationOffset.Z);
                }

                if (ChargeTime > MaxChargeTime)
                {
                    if (Vector2.Distance(AttackPosition, Position.get()) > 0)
                    {
                        for (int i = 0; i < 5; i++)
                            ParticleManager.CreateParticle(new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, LaserColor, LaserStartSize + LaserEndSize, 0);

                        if (Shots > 1)
                        {
                            FireShot(Position.get(), AttackPosition, AttackLineWidth);
                            ChargeTime = MaxChargeTime - ShotTime;
                            Shots--;
                        }
                        else
                        {
                            FireShot(Position.get(), AttackPosition, AttackLineWidth);
                            ChargeTime = 0;
                            Shots = MaxShots;
                            FreezeTime = AttackFreezeTime;
                            Commited = false;
                        }
                    }
                    else
                    {
                        ChargeTime = 0;
                        Shots = MaxShots;
                    }
                }

                SearchTime += gameTime.ElapsedGameTime.Milliseconds;
                if (SearchTime > MaxSearchTime)
                {
                    SearchTime -= MaxSearchTime;
                    AISearch(gameTime);
                }
            }
            else
            {
                if (ChargeTime > 0)
                {
                    ChargeTime -= gameTime.ElapsedGameTime.Milliseconds;
                    float Alpha = ChargeTime / MaxChargeTime;
                    ParticleManager.CreateParticle(new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, LaserColor * Alpha, LaserStartSize + LaserEndSize * Alpha, 1);
                    if (ChargeTime < 0)
                        ChargeTime = 0;
                }
                base.AI(gameTime);
            }

            RevolutionSpeed += (TargetRevolutionSpeed - RevolutionSpeed) * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.1f;
            RotationOffsetSpeed = new Vector3(0, 0, RevolutionSpeed);
        }

        public void FireShot(Vector2 StartPosition, Vector2 EndPosition, float LineWidth)
        {
            SoundManager.Play3DSound("SiegeLaserFire", new Vector3(Position.X(), Y, Position.Y()), 0.75f, 1000, 1.5f);

            ParticleManager.CreateParticle(new Vector3(StartPosition.X, 0, StartPosition.Y), Vector3.Zero, LaserColor, 300, 5);
            ParticleManager.CreateParticle(new Vector3(StartPosition.X, 0, StartPosition.Y), Vector3.Zero, LaserColor, 300, 7);
            ParticleManager.CreateParticle(new Vector3(StartPosition.X, 0, StartPosition.Y), Vector3.Zero, LaserColor, 300, 4);

            int c = (int)Vector2.Distance(StartPosition, EndPosition) / 10;
            for (int i = 0; i < c; i++)
                ParticleManager.CreateParticle(
                    new Vector3(StartPosition.X, 0, StartPosition.Y) + (new Vector3(EndPosition.X - StartPosition.X, 0, EndPosition.Y - StartPosition.Y) * i / c),
                    Vector3.Zero, LaserColor, 100, 5);

            ParticleManager.CreateParticle(new Vector3(EndPosition.X, 0, EndPosition.Y), Vector3.Zero, LaserColor, 300, 5);
            ParticleManager.CreateParticle(new Vector3(EndPosition.X, 0, EndPosition.Y), Vector3.Zero, LaserColor, 300, 7);
            ParticleManager.CreateParticle(new Vector3(EndPosition.X, 0, EndPosition.Y), Vector3.Zero, LaserColor, 300, 4);

            QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

            Vector2 UpperLeftCorner = Logic.Min(StartPosition, EndPosition) - new Vector2(200);
            Vector2 LowerRightCorner = Logic.Max(StartPosition, EndPosition) + new Vector2(200);

            QuadGridXMin = (int)((UpperLeftCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridXMax = (int)((LowerRightCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridYMin = (int)((UpperLeftCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);
            QuadGridYMax = (int)((LowerRightCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);

            if (QuadGridXMax > quadGrid.CellsX - 1)
                QuadGridXMax = quadGrid.CellsX - 1;
            if (QuadGridXMin > quadGrid.CellsX - 1)
                QuadGridXMin = quadGrid.CellsX - 1;
            if (QuadGridYMax > quadGrid.CellsY - 1)
                QuadGridYMax = quadGrid.CellsY - 1;
            if (QuadGridYMin > quadGrid.CellsY - 1)
                QuadGridYMin = quadGrid.CellsY - 1;
            if (QuadGridXMax < 0)
                QuadGridXMax = 0;
            if (QuadGridXMin < 0)
                QuadGridXMin = 0;
            if (QuadGridYMax < 0)
                QuadGridYMax = 0;
            if (QuadGridYMin < 0)
                QuadGridYMin = 0;

            foreach (Basic2DObject g in quadGrid.Enumerate(QuadGridXMin, QuadGridYMin, QuadGridXMax, QuadGridYMax))
                if (g.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)g;
                    if (!s.IsAlly(this) && CheckCircle(s ,StartPosition, EndPosition, LineWidth))
                    {
                        s = s.ReturnCollision();
                        s.Damage(LaserDamage * GetDamageMult(), 1, EndPosition - StartPosition, this, AttackType.Red);
                    }
                }
        }

        public bool CheckCircle(BasicShipGameObject g ,Vector2 StartPosition, Vector2 EndPosition, float LineWidth)
        {
            return DistanceLineSegmentToPoint(StartPosition, EndPosition, g.getPosition()) < (g.getSize().X + AttackLineWidth) / 2;
        }

        public float DistanceLineSegmentToPoint(Vector2 A, Vector2 B, Vector2 p)
        {
            Vector2 v = B - A;
            v.Normalize();

            float distanceAlongLine = Vector2.Dot(p, v) - Vector2.Dot(A, v);
            Vector2 nearestPoint;

            if (distanceAlongLine < 0)
                nearestPoint = A;
            else if (distanceAlongLine > Vector2.Distance(A, B))
                nearestPoint = B;
            else
                nearestPoint = A + distanceAlongLine * v;

            float actualDistance = Vector2.Distance(nearestPoint, p);
            return actualDistance;
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanBasicIndex + 3;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Ship4");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType != AttackType.Explosion && attackType != AttackType.Melee)
            {
                if (FreezeTime > 0)
                {
                    ChargeTime = 0;
                    damage -= 0.3f;
                    base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
                }
                else
                {
                    damage -= 0.3f;
                    damage /= 0.25f;
                    base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
                    FreezeTime = 0;
                }
            }
            else
                base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
