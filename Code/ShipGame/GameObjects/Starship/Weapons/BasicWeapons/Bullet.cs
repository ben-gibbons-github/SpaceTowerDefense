using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Bullet : Basic2DObject
    {
        public static LinkedList<Bullet> DodgeBullets = new LinkedList<Bullet>();
        public bool ShouldDodge = false;

        public float ImpactVolume = 0.5f;
        public float ImpactDistance = 500;
        public float ImpactExponent = 2;
        public string ImpactString = "VampireImpact";
        bool BounceSound = false;

        bool Commited = false;
        bool Destroyed = false;
        private static int MaxSearchTime = 500;
        protected bool Armed = true;
        private int SearchTime = 0;
        public Vector2 Speed = Vector2.One;
        public UnitBasic ParentUnit;
        public bool Big;

        protected float Damage = 0;
        protected float PushTime = 0;
        protected float ModifierFactor;
        protected float PushVelocityMult = 0.1f;
        public int LifeTime;
        public int TimeAlive;
        protected Vector2 BulletDodgeDistance = new Vector2(300);
        protected float BulletExplosionDistance = 0;
        protected float BulletExplosionDamage = 0;
        public AttackType attackType = AttackType.White;

        protected BasicShipGameObject[] TargetsHit;
        protected int TargetHitCount = 0;
        protected int MaxHits;
        protected bool BulletCanBounce = true;
        protected bool BulletHasBounced = false;

        public Vector2 PreviousPosition;
        public Matrix WorldMatrix;
        protected bool NoInstanceCommit = false;
        public float Y;


        public override void Create()
        {
            if (ShouldDodge)
                DodgeBullets.AddLast(this);

            AddTag(GameObjectTag.Update);
            if (!Commited && !NoInstanceCommit)
            {
                Commited = true;
                BulletInstancer.AddChild(this);
            }
            base.Create();
        }

        public void SetStartingPosition(Vector2 Position)
        {
            PreviousPosition = Position;
        }

        public void SetAttackType(AttackType attackType)
        {
            this.attackType = attackType;
        }

        public void SetShipParent(UnitBasic ShipParent)
        {
            this.ParentUnit = ShipParent;
        }

        public void SetModifierFactor(float ModifierFactor)
        {
            this.ModifierFactor = ModifierFactor;
        }

        public void SetDamage(float Damage, float PushTime, float PushVelocityMult)
        {
            this.PushTime = PushTime;
            this.PushVelocityMult = PushVelocityMult;
            this.Damage = Damage;
        }

        public void SetLifeTime(int LifeTime)
        {
            this.LifeTime = LifeTime;
        }

        public void SetMaxHits(int MaxHits)
        {
            this.MaxHits = MaxHits;
        }

        public virtual void SetSpeed(Vector2 Speed)
        {
            this.Speed = Speed * ParentUnit.GetDamageMult();
        }

        public void AddTime(int Milliseconds)
        {
            SearchTime += Milliseconds;

            Milliseconds = Math.Min(LifeTime - TimeAlive, Milliseconds);
            TimeAlive += Milliseconds;
            Position.addNoPerform(Speed * Milliseconds / 1000f * 60f);
            if (TimeAlive >= LifeTime)
            {
                Armed = false;
                Destroy();
            }
        }

        private void TestCollisions()
        {
            QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

            Vector2 UpperLeftCorner = Vector2.Zero;
            Vector2 LowerRightCorner = Vector2.Zero;

            if (SearchTime < MaxSearchTime)
            {
                UpperLeftCorner = Logic.Min(PreviousPosition, getPosition());
                LowerRightCorner = Logic.Max(PreviousPosition, getPosition());
            }
            else
            {
                SearchTime = 0;
                UpperLeftCorner = Logic.Min(PreviousPosition, getPosition() - BulletDodgeDistance);
                LowerRightCorner = Logic.Max(PreviousPosition, getPosition() + BulletDodgeDistance);
            }

            if (UpperLeftCorner.X < quadGrid.Min.X || UpperLeftCorner.Y < quadGrid.Min.Y ||
                LowerRightCorner.X > quadGrid.Max.X || LowerRightCorner.Y > quadGrid.Max.Y)
                return;

            QuadGridXMin = (int)((UpperLeftCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridXMax = (int)((LowerRightCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridYMin = (int)((UpperLeftCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);
            QuadGridYMax = (int)((LowerRightCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);

            foreach (Basic2DObject g in quadGrid.Enumerate(QuadGridXMin, QuadGridYMin, QuadGridXMax, QuadGridYMax))
                if (g.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                {
                    if (CheckCircle(g))
                    {
                        BasicShipGameObject s = (BasicShipGameObject)g;
                        s = s.ReturnCollision();
                        if (s != null && s.StopsBullet(ParentUnit))
                        {
                            Collide(s);
                            if (Destroyed)
                            {
                                return;
                            }
                        }
                    }
                    else if (g.GetType().IsSubclassOf(typeof(UnitShip)))
                    {
                        float d = Vector2.Distance(getPosition(), g.getPosition());
                        if (d < BulletDodgeDistance.X && d - Speed.Length() / 4 > Vector2.Distance(Position.get() + Speed, g.getPosition()))
                        {
                            UnitShip u = (UnitShip)g;
                            if (u.BulletDodgeDistance > 0 && !u.IsAlly(ParentUnit))
                            {
                                u.SetBulletToDodge(this);
                            }
                        }
                    }
                }
        }

        public bool CheckCircle(Basic2DObject g)
        {
            return Logic.DistanceLineSegmentToPoint(getPosition(), PreviousPosition, g.getPosition()) < (g.Size.get().X + Size.get().X) / 2;
        }

        public override void Update(GameTime gameTime)
        {
            if (Math.Abs(Y) > 10)
            {
                float YMoveAmount = gameTime.ElapsedGameTime.Milliseconds * 60 / 100000f;

                if (Y > 0)
                    Y -= YMoveAmount;
                else
                    Y += YMoveAmount;
            }

            WorldMatrix = Matrix.CreateScale(Size.X() / BulletInstancer.ModelScale) * Matrix.CreateRotationY(Logic.ToAngle(Speed) + (float)Math.PI) * Matrix.CreateTranslation(new Vector3(Position.X(), Y, Position.Y()));
            PreviousPosition = Position.get();
            AddTime(gameTime.ElapsedGameTime.Milliseconds);
        }

        public override void Update2(GameTime gameTime)
        {
            TestCollisions();
        }

        public virtual void Collide(BasicShipGameObject s)
        {
            if (!s.IsAlly(ParentUnit))
            {
                if (!BulletCanBounce || !s.BulletBounces(this))
                {
                    float Damage = getDamage(s, 1);
                    Damage *= ParentUnit.GetDamageMult();
                    s.Damage(Damage, PushTime * Damage, Speed * PushVelocityMult, ParentUnit, attackType);
                }
                else
                {
                    BulletHasBounced = true;

                    if (!BounceSound)
                    {
                        SoundManager.Play3DSound("ShieldBounce", new Vector3(Position.X(), Y, Position.Y()),
                            0.35f, 500, 1);
                        BounceSound = true;
                    }

                    if (s.GetType().IsSubclassOf(typeof(UnitBasic)))
                    {
                        UnitBasic b = (UnitBasic)s;
                        b.ShieldFlash(1);
                    }
                    Speed = Vector2.Reflect(Speed, Vector2.Normalize(s.Position.get() - PreviousPosition));
                    Position.set(PreviousPosition);
                    return;
                }
            }

            if (TargetHitCount < MaxHits)
            {
                if (TargetsHit == null)
                {
                    TargetsHit = new BasicShipGameObject[MaxHits];
                    TargetsHit[TargetHitCount++] = s;
                }
                else if (!TargetsHit.Contains(s))
                    TargetsHit[TargetHitCount++] = s;
                else
                    return;
            }
            else
                Destroy();
        }

        public override void Destroy()
        {
            if (ShouldDodge && DodgeBullets.Contains(this))
                DodgeBullets.Remove(this);

            if (Commited && !NoInstanceCommit)
            {
                Commited = false;
                BulletInstancer.RemoveChild(this);
            }

            Destroyed = true;
            if (Armed && BulletExplosionDamage > 0 && BulletExplosionDistance > 0)
            {
                QuadGrid grid = Parent2DScene.quadGrids.First.Value;

                foreach(Basic2DObject o in grid.Enumerate(Position.get(), new Vector2(BulletExplosionDistance)))
                if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)o;
                    float dist = Vector2.Distance(s.Position.get(),  Position.get()) - o.Size.X() / 2;
                   
                    if (dist < BulletExplosionDistance && !ParentUnit.IsAlly(s))
                    {
                        float DistMult = 1;
                        if (dist > 0)
                            DistMult = (BulletExplosionDistance - dist) / BulletExplosionDistance;
                        s.Damage(DistMult * BulletExplosionDamage, DistMult * PushTime, Vector2.Normalize(s.Position.get() - Position.get()) * PushVelocityMult * DistMult, ParentUnit, attackType);
                    }
                }
            }

            SoundManager.Play3DSound(ImpactString, new Vector3(Position.X(), Y, Position.Y()),
                ImpactVolume, ImpactDistance, ImpactExponent);

            base.Destroy();
        }

        public virtual float getDamage(BasicShipGameObject s, float Mult)
        {
            return Damage * Mult;
        }

        public void SetExplosive(float BulletExplosionDistance, float BulletExplosionDamage)
        {
            this.BulletExplosionDistance = BulletExplosionDistance;
            this.BulletExplosionDamage = BulletExplosionDamage;
        }
    }
}
