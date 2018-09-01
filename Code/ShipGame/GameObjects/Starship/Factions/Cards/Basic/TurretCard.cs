using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShipGame.Wave;

namespace BadRabbit.Carrot
{
    public class TurretCard : FactionCard
    {
        public static Color RedColor = new Color(1, 0.5f, 0.5f);
        public static Color GreenColor = new Color(0.5f, 1, 0.5f);
        public static Color BlueColor = new Color(0.5f, 0.5f, 1);
        public static Color GreenBlueColor = new Color(0.5f, 0.9f, 0.9f);
        public static Color PurpleColor = new Color(0.8f, 0.5f, 0.9f);
        public static Color WhiteColor = new Color(0.75f, 0.75f, 0.75f);

        static Vector4 TurretBlue4 = new Vector4(0.5f, 0.5f, 1f, 1);
        static Color TurretBlue = new Color(TurretBlue4 / 3);

        public int CircleGlows = 32;
        public float Radius = 200;
        public float TurretSize = 100;

        public int CardCellsCost = 0;
        public int CardCellsCostIncrease = 75;
        public Dictionary<int, int> FactionCostIncreases = new Dictionary<int, int>();

        public string Caption = "";
        public string StrongVs = "";
        Texture2D StrongTexture;

        public Texture2D GetStrongTexture()
        {
            if (StrongTexture == null)
            {
                switch (StrongVs.ToUpper())
                {
                    case "LIGHT":
                        StrongTexture = WaveCard.LightIcon;
                        break;
                    case "MEDIUM":
                        StrongTexture = WaveCard.MediumIcon;
                        break;
                    case "HEAVY":
                        StrongTexture = WaveCard.HeavyIcon;
                        break;
                }
            }
            return StrongTexture;
        }

        public TurretCard()
        {
            CardCellsCost = (int)(Settings.CellsCostMult * CardCellsCost);
        }

        public void Reset()
        {
            FactionCostIncreases.Clear();
        }

        public virtual Color GetColor()
        {
            return Color.White;
        }

        public virtual float GetPlaceStrength(int FactionNumber)
        {
            if (NeutralManager.MyPattern.CurrentCard != null)
            {
                if (!FactionCostIncreases.ContainsKey(FactionNumber))
                    return 10 - GetTurretWeight() * (NeutralManager.MyPattern.CurrentCard.Type.Equals(StrongVs) ? 1f : 2f);
                else// if (FactionCostIncreases[FactionNumber] < WaveManager.CurrentWave * GetTurretWeight())
                    return 10 - ((GetTurretWeight() * (NeutralManager.MyPattern.CurrentCard.Type.Equals(StrongVs) ? 1f : 2f)) * 
                        (1 + FactionCostIncreases[FactionNumber] / 2f));
            }
            else
                return -1000;
        }

        protected virtual float GetTurretWeight()
        {
            return 0.5f; //higher for more expensive
        }

        public virtual float GetTurretFragility()
        {
            return 1.5f; //higher more fragile, negative avoids blocks
        }

        public virtual float GetBuildingAvoidence()
        {
            return 1; //higher avoids allies, negative atracts towards them
        }

        public virtual float GetTurretAvoidence()
        {
            return 0; //higher avoids allies, negative atracts towards them
        }

        public virtual float GetBaseAvoidence()
        {
            return 0; //higher avoids allies, negative atracts towards them
        }

        public virtual float GetTurretAgression()
        {
            return 1; //higher puts it in line with spawns
        }

        public virtual Vector2 GetPlacePosition(int FactionNumber)
        {
            float BestScore = -500000;
            Vector2 BestPosition = Vector2.Zero;

            float Distance = 1000;
            float JumpSize = 100;

            Vector2 MinPos = new Vector2(10000);
            Vector2 MaxPos = new Vector2(-10000);

            foreach (MiningPlatform m in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(MiningPlatform)))
                if (m.GetTeam() == FactionManager.GetTeam(FactionNumber) && !m.Dead)
                {
                    MinPos = Vector2.Min(MinPos, m.Position.get() - new Vector2(Distance));
                    MaxPos = Vector2.Max(MaxPos, m.Position.get() + new Vector2(Distance));
                }

            Basic2DScene b = (Basic2DScene)GameManager.GetLevel().getCurrentScene();
            MinPos = Vector2.Max(MinPos, b.MinBoundary.get());
            MaxPos = Vector2.Min(MaxPos, b.MaxBoundary.get());

            for (float x = MinPos.X; x < MaxPos.X; x += JumpSize)
                for (float y = MinPos.Y; y < MaxPos.Y; y += JumpSize)
                    if (TestFree(new Vector2(x, y), TurretSize, FactionNumber) && 
                        PathFindingManager.GetCellValue(new Vector2(x, y)) != PathFindingManager.DeadCell)
                    {
                        float score = 5000;

                        if (GetTurretFragility() != 0)
                        {
                            if (PathFindingManager.CollisionLine(new Vector2(x, y), 1, 0, 3))
                                score += 100 * GetTurretFragility();
                            if (PathFindingManager.CollisionLine(new Vector2(x, y), -1, 0, 3))
                                score += 100 * GetTurretFragility();
                            if (PathFindingManager.CollisionLine(new Vector2(x, y), 0, 1, 3))
                                score += 100 * GetTurretFragility();
                            if (PathFindingManager.CollisionLine(new Vector2(x, y), 0, -1, 3))
                                score += 100 * GetTurretFragility();

                            if (PathFindingManager.CollisionLine(new Vector2(x, y), -1, -1, 2))
                                score += 100 * GetTurretFragility();
                            if (PathFindingManager.CollisionLine(new Vector2(x, y), 1, -1, 2))
                                score += 100 * GetTurretFragility();
                            if (PathFindingManager.CollisionLine(new Vector2(x, y), -1, 1, 2))
                                score += 100 * GetTurretFragility();
                            if (PathFindingManager.CollisionLine(new Vector2(x, y), 1, 1, 2))
                                score += 100 * GetTurretFragility();
                        }

                        foreach (NeutralSpawn spawn in NeutralManager.SpawnList)
                            if (PathFindingManager.GetCellValue(spawn.Position.get()) > PathFindingManager.StartingCell - 50 &&
                                PathFindingManager.GetCellValue(spawn.Position.get()) < PathFindingManager.StartingCell - 20 &&
                                PathFindingManager.GetAreaClear(spawn.Position.get()))
                            {
                                if (GetTurretFragility() != 0 && PathFindingManager.CollisionLine(new Vector2(x, y), spawn.Position.get()))
                                    score += 500 * GetTurretFragility();

                                if (score > BestScore)
                                {
                                    MiningPlatform NearestMiningPlatform = null;
                                    float NearestDistance = 10000;

                                    foreach (MiningPlatform m2 in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(MiningPlatform)))
                                        if (m2.GetTeam() == FactionManager.GetTeam(FactionNumber) && !m2.Dead)
                                        {
                                            float d = Vector2.Distance(m2.Position.get(), spawn.Position.get());
                                            if (d < NearestDistance)
                                            {
                                                NearestDistance = d;
                                                NearestMiningPlatform = m2;
                                            }
                                        }

                                    score -= Logic.DistanceLineSegmentToPoint(spawn.Position.get(),
                                        NearestMiningPlatform.Position.get(), new Vector2(x, y)) * GetTurretAgression();
                                }
                            }

                        if (score > BestScore)
                        {
                            Basic2DScene Parent2DScene = (Basic2DScene)GameManager.GetLevel().getCurrentScene();
                            QuadGrid quad = Parent2DScene.quadGrids.First.Value;

                            foreach (Basic2DObject o in quad.Enumerate(new Vector2(x, y), new Vector2(200)))
                                if (o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                                {
                                    UnitBuilding s = (UnitBuilding)o;
                                    if (s.GetTeam() == FactionManager.GetTeam(FactionNumber))
                                    {
                                        float d = Vector2.Distance(o.Position.get(), new Vector2(x, y));
                                        if (d < 2000)
                                        {
                                            score -= (2000 - d) * GetBuildingAvoidence();
                                            if (s.GetType().IsSubclassOf(typeof(MiningPlatform)))
                                                score -= (2000 - d) * GetBaseAvoidence();
                                            else if (s.GetType().IsSubclassOf(typeof(UnitTurret)))
                                            {
                                                UnitTurret t = (UnitTurret)s;
                                                if (t.MyCard != null)
                                                {
                                                    if (t.MyCard.StrongVs.Equals(StrongVs))
                                                        score -= (2000 - d) * GetTurretAvoidence();
                                                }
                                                else
                                                {
                                                    if (StrongVs.Equals("Heavy"))
                                                        score -= (2000 - d) * GetTurretAvoidence();
                                                }
                                            }
                                        }
                                    }
                                }

                            if (score > BestScore)
                            {
                                BestScore = score;
                                BestPosition = new Vector2(x, y);
                            }
                        }
                    }

            return BestPosition;
        }

        public bool TestFree(Vector2 Position, float Size, int FactionNumber)
        {
            Basic2DScene Parent2DScene = (Basic2DScene)GameManager.GetLevel().getCurrentScene();
            QuadGrid quad = Parent2DScene.quadGrids.First.Value;

            foreach (UnitBasic o in FactionManager.SortedUnits[FactionManager.GetTeam(FactionNumber)])
                if (o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                {
                    UnitBuilding s = (UnitBuilding)o;
                    if (Vector2.Distance(Position, o.Position.get()) < (Size + o.Size.X()))
                        return false;
                }

            return true;
        }

        public override bool APress(PlayerShip Ship, bool APrevious)
        {
            if (!APrevious)
            {
                {
                    int CardCost = CardCellsCost;
                    if (FactionCostIncreases.ContainsKey(Ship.FactionNumber))
                        CardCost += CardCellsCostIncrease * FactionCostIncreases[Ship.FactionNumber];

                    if (FactionManager.CanAfford(Ship.FactionNumber, CardCost) && Ship.CanPlaceTurret(TurretSize))
                    {
                        if (FactionCostIncreases.ContainsKey(Ship.FactionNumber))
                            FactionCostIncreases[Ship.FactionNumber]++;
                        else
                            FactionCostIncreases.Add(Ship.FactionNumber, 1);

                        FactionManager.AddCells(Ship.FactionNumber, -CardCost);
                        UnitTurret u = (UnitTurret)GetUnit(Ship.FactionNumber);
                        u.MyCard = this;
                        Ship.PlaceTurret(u);
                        return true;
                    }
                    else
                        RedFlashAlpha = 1;
                }
            }
            return false;
        }

        public override void DrawTechTree(Vector2 Position, float Alpha, PlayerShip Ship)
        {
            if (!SortedTextures.ContainsKey(GetImagePath()))
                SortedTextures.Add(GetImagePath(), AssetManager.Load<Texture2D>("Textures/ShipGame/TurretPictures/" + GetImagePath()));

            int CardCost = CardCellsCost;
            if (FactionCostIncreases.ContainsKey(Ship.FactionNumber))
                CardCost += CardCellsCostIncrease * FactionCostIncreases[Ship.FactionNumber];

            Color col = new Color(((Color.White * (1 - RedFlashAlpha)).ToVector3() + (Color.Red * RedFlashAlpha).ToVector3())) * (Alpha) * Alpha;
            Rectangle r = new Rectangle((int)Position.X, (int)Position.Y, (int)TechTreeGroup.CellSize.X, (int)TechTreeGroup.CellSize.Y);
            Render.DrawSolidRect(r, Color.Black * Alpha * 0.4f);
            Render.DrawSprite(SortedTextures[GetImagePath()], Position + TechTreeGroup.CellSize / 2, TechTreeGroup.CellSize, 0, col);
            Render.DrawOutlineRect(r, 3, col);
            Render.DrawShadowedText(Name + "\n$ " + CardCost.ToString(), Position, col);
        }

        public override void Update3D(PlayerShip Ship)
        {
            if (Ship.CanPlaceTurret(TurretSize))
            {
                Vector2 PlacePosition = Ship.GetPlacePosition(TurretSize / 2);
                ParticleManager.CreateParticle(new Vector3(PlacePosition.X, 0, PlacePosition.Y), Vector3.Zero, TurretBlue, TurretSize * (10 + Rand.r.Next(5)), 1);
                ParticleManager.CreateRing(new Vector3(PlacePosition.X, 0, PlacePosition.Y), TurretSize * UnitBuilding.BuildingRingSizeMult, Ship.GetTeam());

                for (int i = 0; i < CircleGlows; i++)
                {
                    float R = (float)(((float)i / CircleGlows * 2 * Math.PI) + (Level.Time % 2 / 2f * Math.PI));
                    Vector3 P = new Vector3((float)Math.Cos(R) * Radius + PlacePosition.X, 0, (float)Math.Sin(R) * Radius + PlacePosition.Y);
                    ParticleManager.CreateParticle(P, Vector3.Zero, Color.White, 64, 1);
                }
            }
        }
    }
}
