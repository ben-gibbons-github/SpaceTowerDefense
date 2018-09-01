using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class FireMode
    {
        protected GunBasic Parent;
        protected UnitBasic ParentUnit;

        //Counters
        private int ROF = 0;
        private int BurstTime = 0;
        private int ReloadTime = 0;
        private int ClipSize = 0;
        private int BurstSize = 0;
        private int ChargeTime = 0;

        //Ammo
        protected int MaxROF;
        protected int MaxBurstTime;
        protected int MaxReloadTime;
        protected int MaxClipSize = 1;
        protected int MaxBurstSize = 1;
        public int Ammo = -100;

        //Creation
        public float BulletSpeed = 1;
        protected float Accuracy;
        protected int BulletCount = 1;
        protected int LifeTime = 900;
        protected int MaxHits = 1;
        protected int MaxChargeTime = 0;

        //Sound
        protected float FireVolume = 0.5f;
        protected string FireSound = "VampireFire";
        protected float FireDistance = 800;
        protected float FireExponent = 2;

        //Damage
        protected float Damage;
        protected float ModifierFactor = 1;
        protected float PushTime;
        protected float PushVelocityMult = 0.1f;
        protected float BulletExplosionDistance = 0;
        protected float BulletExplosionDamage = 0;
        protected AttackType attackType = AttackType.White;

        public void ReadyROF()
        {
            ROF = MaxROF + 1;
        }

        public virtual void SetLevel(float Level)
        {

        }

        public void SetParent(GunBasic Parent)
        {
            this.Parent = Parent;
        }

        public void SetParent(UnitBasic ShipPartent)
        {
            this.ParentUnit = ShipPartent;
        }

        public virtual void Fire(GameTime gameTime)
        {
            if (ReloadTime <= 0)
            {
                bool CreatedMatrix = false;
                Matrix RotationMatrix = Matrix.Identity;
                ChargeTime += gameTime.ElapsedGameTime.Milliseconds;
                if (ChargeTime > MaxChargeTime)
                {
                    while (ROF >= MaxROF && ClipSize > 0 && BurstSize > 0 && (Ammo > 0 || Ammo == -100))
                    {
                        Vector3 P = Parent != null ?
                   new Vector3(Parent.getParent().Position.X(), 0, Parent.getParent().Position.Y()) :
                   new Vector3(ParentUnit.Position.X(), 0, ParentUnit.Position.Y());

                        SoundManager.Play3DSound(FireSound,
                            P,
                            FireVolume, FireDistance, FireExponent);


                        ChargeTime = 0;
                        if (!CreatedMatrix)
                        {
                            CreatedMatrix = true;
                            if (Parent != null)
                                RotationMatrix = Matrix.CreateRotationZ(-Parent.getRotation());
                            else if (ParentUnit.Guns != null)
                                RotationMatrix = Matrix.CreateRotationZ(-ParentUnit.Guns[0].getRotation());
                        }

                        ROF -= MaxROF;
                        BurstTime = 0;
                        ClipSize--;
                        BurstSize--;
                        if (Ammo > 0)
                            Ammo--;

                        for (int i = 0; i < BulletCount; i++)
                        {
                            Bullet b = getBullet();
                            if (Parent != null)
                            {
                                if (Parent.getParent().GetType().IsSubclassOf(typeof(UnitShip)))
                                {
                                    UnitShip s = (UnitShip)Parent.getParent();
                                    if (s.IsHuge)
                                        b.Big = true;
                                }
                                Parent.getParent().ParentLevel.AddObject(b);
                                b.SetShipParent(Parent.getParent());

                                ParentUnit = Parent.getParent();
                                Vector3 P3 = InstanceManager.GetWeaponPosition(ParentUnit.GetIntType(), new Vector3(ParentUnit.Position.X(), ParentUnit.Y, ParentUnit.Position.Y()), ref ParentUnit.RotationMatrix, ParentUnit.WeaponIndexID++, ParentUnit.Size.X());

                                Vector2 P2 = getPositionPattern(i);
                                if (P2 != Vector2.Zero)
                                    P2 = Vector2.Transform(P2, RotationMatrix);

                                b.SetPosition(new Vector2(P3.X, P3.Z) + P2, false);
                                b.Y = P3.Y;
                                Vector2 v = BulletSpeed * Logic.ToVector2(Parent.getRotation() + Accuracy - Rand.F() * Accuracy * 2 + getDirectionPattern(i));
                                b.SetSpeed(v);
                            }
                            else
                            {
                                ParentUnit.ParentLevel.AddObject(b);
                                if (ParentUnit.GetType().IsSubclassOf(typeof(UnitShip)))
                                {
                                    UnitShip s = (UnitShip)ParentUnit;
                                    if (s.IsHuge)
                                        b.Big = true;
                                }
                                b.SetShipParent(ParentUnit);
                                b.SetPosition(ParentUnit.Position.get() + Vector2.Transform(getPositionPattern(i), RotationMatrix), false);
                                b.SetSpeed(BulletSpeed * Logic.ToVector2(ParentUnit.Guns[0].getRotation() + Accuracy - Rand.F() * Accuracy * 2 + getDirectionPattern(i)));
                            }

                            b.SetAttackType(attackType);
                            b.SetStartingPosition(b.getPosition());
                            b.SetDamage(Damage, PushTime, PushVelocityMult);
                            b.SetModifierFactor(ModifierFactor);
                            b.SetLifeTime(LifeTime);
                            b.SetExplosive(BulletExplosionDistance, BulletExplosionDamage);
                            b.AddTime(ROF);
                        }
                    }
                }
                else
                    CreateChargeParticles(ChargeTime / (float)(MaxChargeTime));
                if (ClipSize <= 0)
                    ReloadTime += gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        public virtual void CreateChargeParticles(float A)
        {

        }

        public virtual void Fire(float DirectionR)
        {
            if (ParentUnit == null)
                return;

            if (ReloadTime <= 0)
            {
                bool CreatedMatrix = false;
                Matrix RotationMatrix = Matrix.Identity;

                SoundManager.Play3DSound(FireSound,
                    new Vector3(ParentUnit.Position.X(), ParentUnit.Y, ParentUnit.Position.Y()),
                    FireVolume, FireDistance, FireExponent);

                if (!CreatedMatrix)
                {
                    CreatedMatrix = true;
                    RotationMatrix = Matrix.CreateRotationZ(DirectionR);
                }

                ROF -= MaxROF;
                BurstTime = 0;
                ClipSize--;
                BurstSize--;
                if (Ammo > 0)
                    Ammo--;

                for (int i = 0; i < BulletCount; i++)
                {
                    Bullet b = getBullet();

                    ParentUnit.ParentLevel.AddObject(b);
                    b.SetShipParent(ParentUnit);
                    b.SetPosition(ParentUnit.Position.get() + Vector2.Transform(getPositionPattern(i), RotationMatrix), false);
                    b.SetSpeed(BulletSpeed * Logic.ToVector2(DirectionR + Accuracy - Rand.F() * Accuracy * 2 + getDirectionPattern(i)));
                    b.SetAttackType(attackType);
                    b.SetStartingPosition(b.getPosition());
                    b.SetDamage(Damage, PushTime, PushVelocityMult);
                    b.SetModifierFactor(ModifierFactor);
                    b.SetLifeTime(LifeTime);
                    b.SetExplosive(BulletExplosionDistance, BulletExplosionDamage);
                    //b.AddTime(ROF);
                }
            }
        }


        public virtual float getDirectionPattern(int BulletNumb)
        {
            return 0;
        }

        public virtual Vector2 getPositionPattern(int BulletNumb)
        {
            return Vector2.Zero;
        }

        public virtual Bullet getBullet()
        {
            return null;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (ReloadTime > 0)
            {
                ReloadTime += gameTime.ElapsedGameTime.Milliseconds;
                if (ReloadTime > MaxReloadTime)
                {
                    ReloadTime = 0;
                    ClipSize = MaxClipSize;
                }
            }

            BurstTime += gameTime.ElapsedGameTime.Milliseconds;
            if (BurstTime > MaxBurstTime)
            {
                BurstTime = 0;
                BurstSize = MaxBurstSize;
            }

            ROF = Math.Min(MaxROF, ROF + gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}
