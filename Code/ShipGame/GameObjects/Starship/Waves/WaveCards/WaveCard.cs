using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BadRabbit.Carrot;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShipGame.Wave
{
    public class WaveCard
    {
        public static Texture2D LightIcon;
        public static Texture2D MediumIcon;
        public static Texture2D HeavyIcon;

        public static Texture2D UnitBox;
        public static Texture2D UnitPicker;

        public static float LevelMult = 5;
        public Texture2D MyTexture;
        public Texture2D MyIconTexture;
        public string ImagePath;

        public string Name = "";
        public string Type = "";
        public Vector2 TypeSize;

        public string Description = "";
        public int Level = 1; // not zero indexed
        public int MinDiff;
        public int MaxDiff;
        private List<WaveUnit> Units = new List<WaveUnit>();
        private int CurrentUnit;
        int UnitTimer;
        protected int MaxUnitTimer = 125;
        bool CanSpawnUnits = false;
        public int EnergyCost;
        public bool Used;
        public Dictionary<int, int> Votes = new Dictionary<int, int>();
        public bool Super = false;
        public Color typeColor = Color.White * 0.75f;
        public LinkedList<WaveCardText> TextItems = new LinkedList<WaveCardText>();
        float SelectedAlpha = 0;

        static WaveCard()
        {
            if (UnitPicker == null)
            {
                UnitPicker = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/OverHud/HudUnitPicker");
                UnitBox = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/OverHud/HudUnitBox");

                LightIcon = AssetManager.Load<Texture2D>("Textures/ShipGame/UnitPictures/Outlines/Light");
                MediumIcon = AssetManager.Load<Texture2D>("Textures/ShipGame/UnitPictures/Outlines/Medium");
                HeavyIcon = AssetManager.Load<Texture2D>("Textures/ShipGame/UnitPictures/Outlines/Heavy");
            }
        }

        public void SetType(string Type)
        {
            if (Type == "Light")
            {
                MyIconTexture = LightIcon;
                typeColor = new Color(0.5f, 0.5f, 1);
            }
            if (Type == "Medium")
            {
                MyIconTexture = MediumIcon;
                typeColor = new Color(0.5f, 1, 0.5f);
            }
            if (Type == "Heavy")
            {
                MyIconTexture = HeavyIcon;
                typeColor = new Color(1, 0.5f, 0.5f);
            }
            this.Type = Type;
            this.TypeSize = WaveCardText.Font.MeasureString(Type);
        }

        public Texture2D GetTexture()
        {
            if (MyTexture == null)
                MyTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/UnitPictures/" + ImagePath);
            return MyTexture;
        }

        public virtual void Create()
        {

        }

        public void AddSpecial(string s)
        {

        }

        public void SetImagePath(string s)
        {
            this.ImagePath = s;
        }

        public void AddUnit(WaveUnit Unit)
        {
            Units.Add(Unit);
            Unit.Cloaked = FactionCard.CloakMode;
            Unit.Summoner = FactionCard.SummonMode;
            Unit.Huge = FactionCard.HugeMode;

            string b = Unit.Cloaked ? "Cloaked " :
                Unit.Summoner ? "Summoner " :
                Unit.Huge ? "Huge " : "";

            if (Unit.Card != null)
            {
                if (Unit.StartingUnitCount > 1)
                    TextItems.AddLast(new WaveCardText(Unit.StartingUnitCount.ToString() + " " + b +
                        Unit.Card.Name + (Unit.StartingUnitCount > 1 ? "s." : ".")));
                else
                    TextItems.AddLast(new WaveCardText(b + Unit.Card.Name + 
                        (Unit.StartingUnitCount > 1 ? "s." : ".")));
            }
        }

        public virtual void WaveStart()
        {

        }

        public virtual void Update(GameTime gameTime)
        {
            if (!CanSpawnUnits)
                return;

            UnitTimer += gameTime.ElapsedGameTime.Milliseconds;

            while (UnitTimer > MaxUnitTimer)
            {
                int reps = 100;

                UnitTimer -= MaxUnitTimer;

                WaveUnit waveUnit = null;

                do
                {
                    reps--;

                    CurrentUnit++;
                    if (CurrentUnit >= Units.Count)
                        CurrentUnit = 0;

                    waveUnit = Units[CurrentUnit];
                }
                while ((waveUnit.Card.GetType().IsSubclassOf(typeof(PlayerCard)) ||
                    waveUnit.Card.GetType().IsSubclassOf(typeof(StrikeCard)) ||
                    waveUnit.UnitCount < 1) && reps > 0);

                waveUnit.UnitCount--;

                CanSpawnUnits = false;
                foreach (WaveUnit w in Units)
                    if (w.UnitCount > 0 && !w.Card.GetType().IsSubclassOf(typeof(PlayerCard)) && !w.Card.GetType().IsSubclassOf(typeof(StrikeCard)))
                        CanSpawnUnits = true;

                LevelMult = 2;

                for (int i = 0; i < waveUnit.Card.GetUnitBuildNumber(); i++)
                {
                    UnitBasic u = waveUnit.Card.GetUnit(NeutralManager.NeutralFaction);
                    if (u == null)
                    {
                        //throw new Exception("Null unit error");
                        return;
                    }

                    WaveManager.self.ParentLevel.AddObject(u);
                    u.SetLevel(waveUnit.Level * LevelMult, 1);
                    NeutralManager.SpawnUnit(u);

                    if (u.GetType().IsSubclassOf(typeof(UnitShip)))
                    {
                        UnitShip u2 = (UnitShip)u;
                        u2.CanCloak = waveUnit.Cloaked;
                        u2.CanSummon = waveUnit.Summoner;
                        u2.IsHuge = waveUnit.Huge;
                        u2.SummonCard = (UnitCard)waveUnit.Card;
                    }
                }
            }
        }

        public virtual void WaveEvent()
        {
            float StreakMult = Math.Max(1, 1 + FactionManager.TeamStreak[WaveManager.ActiveTeam] / FactionManager.TeamStreakDivider);
            PlayerCard playerCard = null;
            StrikeCard strikeCard = null;

            CanSpawnUnits = true;
            foreach (WaveUnit waveUnit in Units)
            {
                waveUnit.UnitCount += (int)(waveUnit.StartingUnitCount * WaveManager.DifficultyMult);

                if (waveUnit.Card.GetType().IsSubclassOf(typeof(PlayerCard)))
                    playerCard = (PlayerCard)waveUnit.Card;
                if (waveUnit.Card.GetType().IsSubclassOf(typeof(StrikeCard)))
                    strikeCard = (StrikeCard)waveUnit.Card;
            }

            WaveManager.SuperWave = Super;

            foreach (PlayerShip p in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(PlayerShip)))
            {
                if (p.GetTeam() != WaveManager.ActiveTeam)
                    p.SetForOffense(playerCard);
                else
                    p.SetForDefense();
            }

            if (strikeCard != null && !strikeCard.CardPick())
            {
                WaveManager.CurrentStrike = strikeCard;
            }
        }

        public void UpdatePicker(GameTime gameTime)
        {
            if (NeutralManager.MyPattern.CurrentCard == this)
            {
                SelectedAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.01f;
                if (SelectedAlpha > 1)
                    SelectedAlpha = 1;
            }
            else
            {
                SelectedAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 0.01f;
                if (SelectedAlpha < 0)
                    SelectedAlpha = 0;
            }

            foreach (WaveCardText t in TextItems)
                if (!t.Update(gameTime))
                    return;
        }

        public void Draw(Vector2 UL, Vector2 LR, float Alpha)
        {
            if (Alpha > 0)
            {
                Render.DrawSprite(UnitBox, (UL + LR) / 2, (LR - UL) * 1.25f, 0, typeColor * Alpha * (0.9f + 0.2f * Rand.F()) * 1.5f);
                Render.DrawSprite(UnitPicker, (UL + LR) / 2, (LR - UL), 0, typeColor * Alpha * (0.9f + 0.2f * Rand.F()) * 1.5f);
                Render.DrawSprite(HudItem.GlowTexture, (UL + LR) / 2, (LR - UL) * 1.25f, 0, typeColor * Alpha * (0.1f + Rand.F() * 0.1f) * 2);
                Render.DrawSprite(GetTexture(), (UL + LR) / 2, LR - UL, 0, Color.White * Alpha * (0.9f + 0.2f * Rand.F()) * 1.25f);
                Render.DrawOutlineRect(UL, LR, 2, Color.White * Alpha * (0.5f + Rand.F()));
                Render.DrawShadowedText(WaveCardText.Font, Name, UL, Vector2.One, Color.White * Alpha * (0.5f + Rand.F()),
                    Color.Black * Alpha * (0.5f + Rand.F()));
                if (MyIconTexture != null)
                    Render.DrawSprite(MyIconTexture, (UL + LR) / 2 + new Vector2(0, UL.Y - LR.Y), (LR - UL) * 0.75f, 0);
                Render.DrawShadowedText(WaveCardText.Font, Type, (UL + LR) / 2 + new Vector2(-TypeSize.X / 2, (UL.Y - LR.Y) * 0.75f), 
                    Vector2.One, typeColor * Alpha * OverCardPicker.SizeBonus, 
                    Color.Black * Alpha * OverCardPicker.SizeBonus);
                
                if (TextItems.Count > 0 && SelectedAlpha > 0)
                {
                    Vector2 DrawPosition = new Vector2(LR.X, (UL.Y + LR.Y) / 2 - TextItems.Count * WaveCardText.Font.MeasureString(TextItems.First.Value.Text).Y / 2);

                    foreach (WaveCardText t in TextItems)
                    {
                        t.Draw(DrawPosition, Alpha * SelectedAlpha * OverCardPicker.SizeBonus);
                        DrawPosition.Y += WaveCardText.Font.MeasureString(TextItems.First.Value.Text).Y;
                    }
                }
            }
        }

        public void EndWave()
        {
            foreach (WaveUnit u in Units)
                u.UnitCount = 0;
        }
    }
}
