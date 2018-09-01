using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public enum AttackType
    {
        Red,
        Green,
        Blue,
        Melee,
        White,
        Explosion,
        None,
    }

    public enum FieldState
    {
        Cloaked,
        DamageBoost,
        SpeedBoost,
        None,
    }

    public class BasicShipGameObject : Basic2DObject
    {
        protected static Color HealthBackgroundColor = Color.Black;
        protected static Color HealthTopColor = Color.Red;
        protected static Color HealthBottomColor = Color.Blue;

        public int CollisionFreezeTime = 400;
        public int FactionNumber = -1;
        public float ThreatLevel = 1; //
        public float CollisionDamage = 0;
        public float CollisionSpeedMult = 0.5f;
        public float CollisionSpeedMass = 1;
        public float HullDamage = 0;
        public float ShieldDamage = 0;
        public Matrix RotationMatrix = Matrix.Identity;
        public bool Dead = false;
        public int ShutDownTime = 0;
        protected int VirusTime = 0;
        public int FieldStateTime = 0;
        public FieldState fieldState = FieldState.None;
        public bool Moveable = false;
        public Matrix WorldMatrix = Matrix.Identity;
        public Color MyColor = new Color(0.25f, 0.25f, 0.25f, 1);
        public float Y = 0;
        public bool Solid = true;
        public float CloakAlpha = 1;

        public float HullToughness = 10;
        public float ShieldToughness = 0;
        protected int MaxShieldRechargeTime = 5000;
        protected float ShieldRechargeRate = 0.1f;
        protected List<UnitTag> ShipTags = new List<UnitTag>();

        private int ShieldRechargeTime;


        public BasicShipGameObject(int FactionNumber)
        {
            this.FactionNumber = FactionNumber;
        }

        public override void Create()
        {
            base.Create();
            AddTag(GameObjectTag.ShipGameUnitBasic);
        }

        public void Add(UnitTag shipTag)
        {
            ShipTags.Add(shipTag);
        }

        public virtual void EmpStrike()
        {

        }

        public virtual void PowerUp()
        {
            HullDamage = 0;
            ShieldDamage = 0;
        }

        public virtual void PlasmaCannonStrike()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (ShieldDamage != 0)
            {
                if (ShieldRechargeTime < MaxShieldRechargeTime)
                {
                    ShieldRechargeTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (ShieldRechargeTime > MaxShieldRechargeTime)
                    {
                        AddShields(ShieldRechargeRate * (ShieldRechargeTime - MaxShieldRechargeTime));
                        ShieldRechargeTime = MaxShieldRechargeTime;
                    }
                }
                else
                {
                    AddShields(ShieldRechargeRate * gameTime.ElapsedGameTime.Milliseconds);
                }
            }

        }

        public virtual void Virus(int VirusTime)
        {
            if (this.VirusTime < VirusTime)
            {
                this.VirusTime = VirusTime;

                QuadGrid quad = Parent2DScene.quadGrids.First.Value;

                foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(VirusTime / 50f * 2)))
                    if (o.GetType().IsSubclassOf(typeof(UnitTurret)) && Vector2.Distance(o.Position.get(), Position.get()) < VirusTime / 50f)
                    {
                        UnitTurret u = (UnitTurret)o;
                        if (u.CanBeTargeted() && u.IsAlly(this))
                        {
                            u.Virus(VirusTime);
                        }
                    }
            }
        }

        protected void AddShields(float Amount)
        {
            ShieldDamage = Math.Max(0, ShieldDamage - Amount);
        }

        public bool TestTag(UnitTag Tag)
        {
            return ShipTags.Contains(Tag);
        }

        public virtual bool StopsBullet(BasicShipGameObject Other)
        {
            return !IsAlly(Other) && !Dead && (fieldState != FieldState.Cloaked || FieldStateTime < 1);
        }

        public bool IsAlly(BasicShipGameObject Other)
        {
            int Team = GetTeam();
            int OTeam = Other.GetTeam();

            if (Team != OTeam)
            {
                if (OTeam == WaveManager.ActiveTeam || Team == WaveManager.ActiveTeam)
                    return false;
                else
                    return true;
            }
            else
                return true;

            if (OTeam == WaveManager.ActiveTeam || Team == WaveManager.ActiveTeam || (Team == NeutralManager.NeutralTeam && OTeam == NeutralManager.NeutralTeam))
            {
                if (OTeam != NeutralManager.NeutralTeam)
                {
                    if (Team != NeutralManager.NeutralTeam)
                        return Team == OTeam || Team == -1 || OTeam == -1;
                    else
                        return OTeam != WaveManager.ActiveTeam;
                }
                else
                    return Team == NeutralManager.NeutralTeam;
            }
            else
                return false;
        }

        public virtual bool CanBeTargeted()
        {
            return !Dead;
        }

        public int GetTeam()
        {
            return FactionNumber == -1 ? -1 :
                FactionNumber < FactionManager.Factions.Count ? 
                FactionManager.Factions[FactionNumber].Team : 
                NeutralManager.NeutralTeam;
        }

        public bool getIsShieldDamage()
        {
            return ShieldDamage < ShieldToughness;
        }

        public float getShieldValue()
        {
            return 1 - ShieldDamage / ShieldToughness;
        }

        public float getHullValue()
        {
            return 1 - HullDamage / HullToughness;
        }

        public virtual void NewWave()
        {

        }

        public virtual void NewWaveEvent()
        {

        }

        public virtual DrawItem getDrawItem()
        {
            return null;
        }

        public virtual int getMaxInteractionTime()
        {
            return 3000;
        }

        public virtual void Interact(PlayerShip p)
        {

        }

        public virtual bool CanInteract(PlayerShip p)
        {
            return true;
        }

        public virtual bool AllowInteract(PlayerShip p)
        {
            return false;
        }

        public virtual int GetUnitWeight()
        {
            return 1;
        }

        public float GetDamageMult()
        {
            return fieldState == FieldState.DamageBoost && FieldStateTime > 0 ? 2 : 1;
        }

        public float Heal(float Amount)
        {
            if (HullDamage >= Amount)
            {
                HullDamage -= Amount;
                return Amount;
            }
            else
            {
                float OldAmount = Amount;

                Amount -= HullDamage;
                HullDamage = 0;
                if (Amount <= 0)
                    return OldAmount;

                if (ShieldDamage >= Amount)
                {
                    ShieldDamage -= Amount;
                    return OldAmount;
                }
                else
                {
                    Amount -= ShieldDamage;
                    ShieldDamage = 0;
                    return OldAmount - Amount;
                }
            }
        }

        public virtual void Damage(float damage, float pushTime, Vector2 pushVector, BasicShipGameObject Damager, AttackType attackType)
        {
            if (ShieldToughness <= ShieldDamage)
            {
                HullDamage += damage;
                if (HullDamage >= HullToughness)
                    BlowUp();
            }
            else
            {
                ShieldDamage += damage;
            }

            ShieldRechargeTime = 0;
        }

        public virtual bool BulletBounces(Bullet b)
        {
            return false;
        }

        public virtual void BlowUp()
        {
            Dead = true;
        }

        public void DrawXButton(bool DrawGlow, float BarMult, float XMult)
        {
            Vector3 Position3 = Game1.graphicsDevice.Viewport.Project(
               new Vector3(this.Position.X(), Y, this.Position.Y()), StarshipScene.CurrentCamera.ProjectionMatrix,
               StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);

            Vector3 Size3 = Game1.graphicsDevice.Viewport.Project(
                new Vector3(this.Position.X() + this.Size.X(), Y, this.Position.Y()), StarshipScene.CurrentCamera.ProjectionMatrix,
                StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);

            Vector2 Position = new Vector2(Position3.X, Position3.Y) - Render.CurrentView.Position;
            float Size = Vector2.Distance(Position, new Vector2(Size3.X, Size3.Y) - Render.CurrentView.Position) / 1.6f;
            Position.Y -= Size;

            Render.DrawSprite(DrawGlow ? ControllerLoader.XButtonGlow : ControllerLoader.XButton, Position, new Vector2(Size), 0);
            if (BarMult > 0)
                Render.DrawBar(Position - new Vector2(Size / 2, 10), Position + new Vector2(Size / 2, 0), BarMult, Color.Black, Color.White);
            if (XMult > 0)
                Render.DrawSprite(PlayerProfile.XTexture, Position, new Vector2(Size) * 2, 0, Color.White * XMult);
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            bool IsUpgraded = false;
            if (GetType().IsSubclassOf(typeof(UnitTurret)))
            {
                UnitTurret u = (UnitTurret)this;
                IsUpgraded = u.IsUpdgraded;
            }

            if (ShieldDamage > 0 || HullDamage > 0 || Dead || IsUpgraded || GetType().IsSubclassOf(typeof(MiningPlatform)))
            {
                float HealthMult = 1 - (((HullDamage > HullToughness ? HullToughness : HullDamage) +
                    (ShieldDamage > ShieldToughness ? ShieldToughness : ShieldDamage)) /
                    (ShieldToughness + HullToughness));

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
            
            base.Draw2D(DrawTag);
        }

        public float HealthMult()
        {
            return 1 - (((HullDamage > HullToughness ? HullToughness : HullDamage) +
                       (ShieldDamage > ShieldToughness ? ShieldToughness : ShieldDamage)) /
                       (ShieldToughness + HullToughness));
        }

        protected virtual void DrawHealthBar(float HealthMult, Vector2 Position, float Size)
        {
            Vector2 Offset = new Vector2(Size, 2.5f);

            Render.DrawSolidRect(Position - Offset, Position + Offset, HealthBackgroundColor);
            Render.DrawSolidRect(Position - Offset + Vector2.One, Position - Offset + new Vector2((Size * 2 - 2) * HealthMult, 4), TeamInfo.GetColor(GetTeam()));
        }

        public virtual void DrawFromMiniMap(Vector2 Position, float Size, Vector2 Min, Vector2 Max)
        {
            if (this.Position.X() > Max.X || this.Position.Y() > Max.Y || this.Position.X() < Min.X || this.Position.Y() < Min.Y)
                return;

            Vector2 MapPosition = (this.Position.get() - Min) /
                (Max - Min) * Size + Position;

            Render.DrawSprite(Render.BlankTexture, MapPosition - new Vector2(1), new Vector2(2), 0, TeamInfo.GetColor(GetTeam()));
        }

        public virtual BasicShipGameObject ReturnCollision()
        {
            return this;
        }

    }
}
