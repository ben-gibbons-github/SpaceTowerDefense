using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BadRabbit.Carrot.WaveFSM;

namespace BadRabbit.Carrot
{
    public class MineralRock : StaticShipGameObject
    {
#if EDITOR && WINDOWS
        private static Texture2D PlaceholderRockTexture;
#endif

        static Color GlowColor = new Color(0.1f, 0.2f, 0.5f);

        public IntValue IsStartingZone;
        public IntValue ProductionAmount;

        public MiningPlatform miningPlatform;
        float ShipMatrixScale = 0;
        float RotationOffset;

        public MineralRock() : base(-1) { }

        public override void Create()
        {
            if (MiningPlatform.MRockPointer == null)
                MiningPlatform.MRockPointer = AssetManager.Load<Texture2D>("Textures/ShipGame/MRockPointer");
            ProductionAmount = new IntValue("Production", 100);
            IsStartingZone = new IntValue("Is Starting Zone", 0); 

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                PlaceholderRockTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/PlaceholderRock");
                AddTag(GameObjectTag._2DForward);
            }
#endif
            AddTag(GameObjectTag._2DSolid);
            AddTag(GameObjectTag.Update);

            base.Create();

            Position.ChangeEvent = ChangePosition;
            Size.ChangeEvent = ChangePosition;
            Size.set(new Vector2(90));
        }

        private void ChangePosition()
        {
            SetQuadGridPosition();
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateRotationY(Rotation.getAsRadians()) * Matrix.CreateTranslation(new Vector3(Position.X(), Y, Position.Y()));
        }

        public override void Update(GameTime gameTime)
        {
            ParticleManager.CreateParticle(new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, GlowColor, Size.X() * 10, 1);

            RotationOffset += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.005f; 
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateFromYawPitchRoll(RotationOffset, Rotation.getAsRadians() + RotationOffset / 2, RotationOffset / 4) * Matrix.CreateTranslation(new Vector3(Position.X(), Y, Position.Y()));
        
            base.Update(gameTime);
        }

        public override int GetIntType()
        {
            return 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("World/Asteroid1");
        }

        public override void CreateInGame()
        {
            base.CreateInGame();

            ShipMatrixScale = InstanceManager.AddChild(this);
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateRotationY(Rotation.getAsRadians()) * Matrix.CreateTranslation(new Vector3(Position.X(), Y, Position.Y()));
       
            SetQuadGridPosition();
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                Render.DrawSprite(PlaceholderRockTexture, Position, Size, Rotation);
            }
            else
#endif

                if (miningPlatform == null)
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

                    Render.DrawSprite(MiningPlatform.MRockPointer, Position, new Vector2(Size), 0, TeamInfo.GetColor(GetTeam()));
                    base.Draw2D(DrawTag);
                }
        }

        public override void DrawFromMiniMap(Vector2 Position, float Size, Vector2 Min, Vector2 Max)
        {
            if (miningPlatform == null)
            {
                if (this.Position.X() > Max.X || this.Position.Y() > Max.Y || this.Position.X() < Min.X || this.Position.Y() > Min.X)
                    return;

                Vector2 MapPosition = (this.Position.get() + Min) /
                    (Max - Min) * Size + new Vector2(Size) + Position;

                Render.DrawSprite(Render.BlankTexture, MapPosition - Vector2.One, new Vector2(2), 0, Color.Gray);
            }
        }

        public override int getMaxInteractionTime()
        {
            return 300;
        }

        public override void Interact(PlayerShip p)
        {
            if (miningPlatform == null && FactionManager.CanBuildMiningPlatform(p.FactionNumber))
            {
                Vector3 P3 = new Vector3(Position.X(), 0, Position.Y());
                for (int i = 0; i < 40; i++)
                    LineParticleSystem.AddParticle(P3, P3 + Rand.V3() * 1000, TeamInfo.GetColor(p.GetTeam()));

                MiningPlatform m = FactionManager.GetMiningPlatform(p.FactionNumber);
                ParentLevel.AddObject(m);
                m.Position.set(Position.get());
                setPlatform(m);

                SoundManager.Play3DSound("PlayerBuildMiningRing",
                    new Vector3(m.Position.X(), Y, m.Position.Y()), 0.25f, 500, 1);

                if (p.PlacedStartingMineralRock)
                {
                    FactionManager.AddCells(p.FactionNumber, -FactionManager.GetMiningPlatformCost(p.FactionNumber));
                    FactionManager.SetBuildingPlatform(p.FactionNumber,  m);
                }
                else
                {
                    FactionManager.GetFaction(p.FactionNumber).MiningPlatformCounter = 0;
                    m.HullToughness *= 2;
                    m.SetAsStarting();
                    p.PlacedStartingMineralRock = true;
                    p.StartingMineralRock = this;
                }

                p.LastPlacedPlatform.AddFirst(m);
                PathFindingManager.AddMineralRock(m);
            }
            base.Interact(p);
        }

        public override bool AllowInteract(PlayerShip p)
        {
            return miningPlatform == null;
        }

        public override bool CanInteract(PlayerShip p)
        {
            return ((FactionManager.CanAfford(p.FactionNumber, FactionManager.GetMiningPlatformCost(p.FactionNumber)) || !p.PlacedStartingMineralRock) && miningPlatform == null && FactionManager.CanBuildMiningPlatform(p.FactionNumber));
        }

        public void setPlatform(MiningPlatform m)
        {
            miningPlatform = m;
            if (m != null)
                m.setRock(this);
        }

        public override BasicShipGameObject ReturnCollision()
        {
            if (miningPlatform == null)
                return this;
            else
                return miningPlatform;
        }

        public override void Destroy()
        {
#if EDITOR && WINDOWS
            if (!ParentLevel.LevelForEditing)
#endif
            InstanceManager.RemoveChild(this);
            base.Destroy();
        }
    }
}
