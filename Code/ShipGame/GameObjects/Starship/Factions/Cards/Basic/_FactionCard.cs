using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public enum CardType
    {
        Rebels,
        Empire,
        Watcher,
        ExtraTurrets,
        UnitSpawners
    }

    public class FactionCard
    {
        public static bool CloakMode = false;
        public static bool SummonMode = false;
        public static bool HugeMode = false;
        public static bool SuperMode = false;
        public static int CardTypeLength = 5;

        public static Dictionary<string, Texture2D> SortedTextures = new Dictionary<string, Texture2D>();

        public static FactionCard[] FactionTurretDeck =
        {
            new PlasmaTurretCard(),
            new SnapTurretCard(),
            new RailTurretCard(),
            new StasisBombCard(),
            new MedicTurretCard(),

            new FlameTurretCard(),
            new SlowFieldCard(),
            new MineLayerCard(),
            new SpikeTurretCard(),
            new SignalTowerCard(),

            new SplinterTurretCard(),
            new ForceTurretCard(),
            new BeamTurretCard(),
            new SpearTurretCard(),
            new CrystalWallCard(),
            
            new PulseTurretCard(),
            new ProtectionCasterCard(),
            new EmpLauncherCard(),
            new RelocationLauncherCard(),
            new BankCard(),
            
            new CobraSpawnerCard(),
            new HornetSpawnerCard(),
            new VampireSpawnerCard(),
            new ScorpionSpawnerCard(),
            new StingraySpawnerCard(),
        };

        public static FactionCard[] FactionUnitDeck =
        {
            new CobraCard(),
            new HornetCard(),
            new VampireCard(),
            new ScorpionCard(),
            new StingrayCard(),
            new PitbullCard(),

            new RecluseCard(),
            new ParasiteCard(),
            new OutcastCard(),
            new MiteCard(),
            new SuperMiteCard(),
            new ScramblerCard(),
            
            new DevourerCard(),
            new ImmortalCard(),
            new CrusherCard(),
            new BabyCrusherCard(),
            
            new CrystalScoutCard(),
            new CrystalFighterCard(),
            new CrystalBattleCruiserCard(),
            new CrystalKnightCard(),
            new CrystalEnforcerCard(),
            new CrystalElementalCard(),

            new PlayerRaidCard(),
            new PlayerJuggernautCard(),
            new PlayerGrenadeCard(),
            new PlayerRocketCard(),
            new PlayerSnipeCard(),
            new PlayerRailCard(),
            new PlayerEngineerCard(),
            new PlayerKnightCard(),
            new PlayerSingularityCard(),
            new PlayerStalkerCard(),

            new PlasmaCannonStrike(),
        };

        static FactionCard()
        {
            SortedTurretDeck = new List<List<FactionCard>>();
            for (int i = 0; i < CardTypeLength; i++)
                SortedTurretDeck.Add(new List<FactionCard>());

            foreach (FactionCard card in FactionTurretDeck)
                SortedTurretDeck[(int)card.cardType].Add(card);
        }

        public static List<List<FactionCard>> SortedTurretDeck;

        public static FactionCard GetFactionUnitCard(string s)
        {
            foreach (FactionCard c in FactionUnitDeck)
            {
                HugeMode = false;
                CloakMode = false;
                SummonMode = false;
                SuperMode = false;

                if (c.Name.ToUpper().Equals(s.ToUpper()))
                {
                    return c;
                }
                if (("CLOAKED" + c.Name.ToUpper()).Equals(s.ToUpper()))
                {
                    CloakMode = true;
                    return c;
                }
                if (("SUMMONER" + c.Name.ToUpper()).Equals(s.ToUpper()))
                {
                    SummonMode = true;
                    return c;
                }
                if (("BIG" + c.Name.ToUpper()).Equals(s.ToUpper()))
                {
                    HugeMode = true;
                    return c;
                }
            }
            return null;
        }

        public CardType cardType = CardType.Rebels;

        public string Name;
        public float Alpha = 0;
        public float RedFlashAlpha = 0;

        public FactionCard()
        {

        }

        public virtual bool APress(PlayerShip Ship, bool APrevious)
        {
            return false;
        }

        public virtual void DrawTechTree(Vector2 Position, float Alpha, PlayerShip Ship)
        {
            if (!GetUnitImagePath().Equals(""))
            {
                if (!SortedTextures.ContainsKey(GetImagePath()))
                    SortedTextures.Add(GetImagePath(), AssetManager.Load<Texture2D>("Textures/ShipGame/TurretPictures/" + GetImagePath()));
            }
            else
            {
                if (!SortedTextures.ContainsKey(GetUnitImagePath()))
                    SortedTextures.Add(GetUnitImagePath(), AssetManager.Load<Texture2D>("Textures/ShipGame/UnitPictures/" + GetUnitImagePath()));
            }

            Color col = new Color(((Color.White * (1 - RedFlashAlpha)).ToVector3() + (Color.Red * RedFlashAlpha).ToVector3())) * (Alpha) * Alpha;
            Rectangle r = new Rectangle((int)Position.X, (int)Position.Y, (int)TechTreeGroup.CellSize.X, (int)TechTreeGroup.CellSize.Y);
            Render.DrawSolidRect(r, Color.Black * Alpha);
            Render.DrawSprite(SortedTextures[GetImagePath()], Position + TechTreeGroup.CellSize / 2, TechTreeGroup.CellSize, 0, col);
            Render.DrawOutlineRect(r, 3, col);
            Render.DrawShadowedText(Name, Position, col);
        }

        public virtual string GetImagePath()
        {
            return "";
        }

        public virtual string GetUnitImagePath()
        {
            return "";
        }

        public void MenuReset()
        {
            RedFlashAlpha = 0;
        }

        public virtual bool UpdateTriggered(GameTime gameTime)
        {
            return true;
        }

        public virtual UnitBasic GetUnit(int FactionNumber)
        {
            return null;
        }

        public void MenuUpdate(GameTime gameTime, bool Selected, float TargetAlpha)
        {
            RedFlashAlpha = Math.Max(0, RedFlashAlpha - PlayerMenu.CloseOpenSpeed * gameTime.ElapsedGameTime.Milliseconds * 2);
        }

        public virtual int GetUnitBuildNumber()
        {
            return 1;
        }

        public virtual void DrawFactionDisplay(Vector2 Position, float Alpha, float Size)
        {

        }

        public virtual void Update3D(PlayerShip Ship)
        {

        }

        public virtual void Draw3D(Camera3D DrawCamera ,PlayerShip Ship)
        {

        }
    }
}
