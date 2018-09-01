using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShipGame.Wave;

namespace BadRabbit.Carrot
{
    public class TurretForm : BasicGameForm
    {
        public Texture2D MyTexture;
        public SpriteFontValue Font;
        public Color MyColor;
        public StringValue Text;

        public TurretCard MyCard;
        public PlayerShip ParentShip;

        bool Selected = false;

        float SelectedAlpha = 0;
        float FlashAlpha = 0;
        float ErrorAlpha = 0;
        float DistanceAlpha = 0;

        float RingAlpha = 0;

        public void SetValues(Texture2D MyTexture, SpriteFont Font, string Text, Color MyColor,
            Vector2 Position, Vector2 Size, FactionCard MyCard)
        {
            ParentShip = (PlayerShip)ParentFrame.Parent;
            this.MyColor = MyColor;
            this.MyTexture = MyTexture;
            this.Font.set(Font);
            this.Text.set(Text);
            this.Size.set(Size);
            this.Position.set(Position);
            this.MyCard = (TurretCard)MyCard;
        }

        public override void Update(GameTime gameTime)
        {
            if (ParentFrame != null)
                DistanceAlpha = (300 - Math.Abs(this.Position.Y() - ParentFrame.CameraPosition.Y)) / 300f *
                    (600 - Math.Abs(this.Position.X() - ParentFrame.CameraPosition.X)) / 600f;

            if (Selected && FactionManager.GetFaction(ParentShip.FactionNumber).PickingCards)
            {
                SelectedAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (SelectedAlpha > 1)
                    SelectedAlpha = 1;
            }
            else
            {
                SelectedAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                if (SelectedAlpha < 0)
                    SelectedAlpha = 0;
            }
            FlashAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
            ErrorAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;

            if (Alpha > 0 && !FactionManager.GetFaction(ParentShip.FactionNumber).PickingCards && MarkerCount > 0)
            {
                if (!ParentShip.CanPlaceTurret(MyCard.TurretSize))
                {
                    RingAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                    if (RingAlpha < 0)
                        RingAlpha = 0;
                }
                else
                {
                    RingAlpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChange;
                    if (RingAlpha > 1)
                        RingAlpha = 1;
                }
            }

            base.Update(gameTime);
        }

        public override void Update2(GameTime gameTime)
        {
            if (Alpha > 0 && !FactionManager.GetFaction(ParentShip.FactionNumber).PickingCards && MarkerCount > 0)
            {
                Vector2 PlacePosition = ParentShip.GetPlacePosition(MyCard.TurretSize / 2);
                ParticleManager.CreateParticle(new Vector3(PlacePosition.X, 0, PlacePosition.Y), Vector3.Zero, MyColor * Alpha * RingAlpha * 0.25f, MyCard.TurretSize * (10 + Rand.r.Next(5)), 1);
                ParticleManager.CreateRing(new Vector3(PlacePosition.X, 0, PlacePosition.Y), MyCard.TurretSize * UnitBuilding.BuildingRingSizeMult * Alpha * RingAlpha, ParentShip.GetTeam());

                for (int i = 0; i < MyCard.CircleGlows; i++)
                {
                    float R = (float)(((float)i / MyCard.CircleGlows * 2 * Math.PI) + (Level.Time % 2 / 2f * Math.PI));
                    Vector3 P = new Vector3((float)Math.Cos(R) * MyCard.Radius * Alpha * RingAlpha + PlacePosition.X, 0, (float)Math.Sin(R) * MyCard.Radius * Alpha * RingAlpha + PlacePosition.Y);
                    ParticleManager.CreateParticle(P, Vector3.Zero, MyColor, 64 * Alpha * RingAlpha, 1);
                }
            }

            base.Update2(gameTime);
        }

        public override bool TriggerAsCurrent(BasicMarker m)
        {
            if (BasicMarker.SelectSound != null)
                BasicMarker.SelectSound.Play(BasicMarker.SelectVolume, 0, 0);

            Faction f = FactionManager.Factions[ParentShip.FactionNumber];
            FlashAlpha = 1;

            if (FactionManager.GetFaction(ParentShip.FactionNumber).PickingCards)
            {
                if (Selected)
                {
                    for (int i = 0; i < f.Cards.Count; i++)
                    {
                        if (f.Cards[i] == MyCard)
                        {
                            f.Cards[i] = null;
                            Selected = false;
                            return true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < f.Cards.Count; i++)
                    {
                        if (f.Cards[i] == null)
                        {
                            f.Cards[i] = MyCard;
                            Selected = true;
                            break;
                        }
                    }

                    if (!Selected)
                    {
                        f.Cards.Add(MyCard);
                        Selected = true;
                    }

                    if (Faction.MaxCards == f.Cards.Count)
                    {
                        for (int i = 0; i < f.Cards.Count; i++)
                        {
                            if (f.Cards[i] == null)
                            {
                                return true;
                            }
                        }

                        ParentShip.ReadyMenu();
                    }

                }
            }
            else
            {
                int CardCost = MyCard.CardCellsCost;
                if (MyCard.FactionCostIncreases.ContainsKey(ParentShip.FactionNumber))
                    CardCost += MyCard.CardCellsCostIncrease * MyCard.FactionCostIncreases[ParentShip.FactionNumber];

                if (FactionManager.CanAfford(ParentShip.FactionNumber, CardCost) && ParentShip.CanPlaceTurret(MyCard.TurretSize))
                {
                    if (MyCard.FactionCostIncreases.ContainsKey(ParentShip.FactionNumber))
                        MyCard.FactionCostIncreases[ParentShip.FactionNumber]++;
                    else
                        MyCard.FactionCostIncreases.Add(ParentShip.FactionNumber, 1);

                    FactionManager.AddCells(ParentShip.FactionNumber, -CardCost);
                    UnitTurret u = (UnitTurret)MyCard.GetUnit(ParentShip.FactionNumber);
                    u.MyCard = MyCard;
                    ParentShip.PlaceTurret(u);
                    //ParentFrame.DeActivate();
                }
                else
                    ErrorAlpha = 1;
            }

            return base.TriggerAsCurrent(m);
        }

        public override void Create()
        {
            MinAlpha = 0.6f;
            Font = new SpriteFontValue("Font");
            Text = new StringValue("Text");
            base.Create();
        }

        public override void DrawAsForm(Vector2 Position, Vector2 Size)
        {
            Vector2 FormSize = Size * Alpha * 1.5f;
            float FormAlpha = Alpha * DistanceAlpha;

            Render.DrawSprite(WaveCard.UnitPicker, Position, Size * new Vector2((Rand.F() + 6) / 7 * 4, 2.5f), Rotation.get(), MyColor * FormAlpha * 2);
            
            Render.DrawSprite(WaveCard.UnitBox, Position, FormSize * 1.25f, Rotation.get(), MyColor * (FormAlpha * FormAlpha * 0.3f * (4 + Rand.F())));
            Render.DrawSprite(WaveCard.UnitPicker, Position, FormSize * new Vector2((Rand.F() + 6) / 7, 1), Rotation.get(), MyColor * (FormAlpha * FormAlpha * 0.3f * (4 + Rand.F())));
            Render.DrawSprite(MyTexture, Position, FormSize, Rotation.get(), Color.White * FormAlpha);


            if (FlashAlpha > 0)
                Render.DrawSprite(HudItem.GlowTexture, Position, FormSize * 2, Rotation.get(), MyColor * 1.5f * FlashAlpha * FormAlpha);
            if (ErrorAlpha > 0)
            {
                Render.DrawSprite(HudItem.GlowTexture, Position, FormSize * 2, Rotation.get(), Color.Red * 1.5f * ErrorAlpha * FormAlpha);
                Render.DrawSprite(FactionEvent.LossTexture, Position, FormSize, Rotation.get(), Color.Red * 1.5f * ErrorAlpha * FormAlpha);
            }
            if (SelectedAlpha > 0)
            {
                Render.DrawSprite(FactionEvent.ReadyTexture, Position, FormSize * SelectedAlpha, Rotation.get(), MyColor * SelectedAlpha * 2 * FormAlpha);
                Render.DrawSprite(HudItem.GlowTexture, Position, FormSize, Rotation.get(), MyColor * SelectedAlpha * FormAlpha * 1.5f);
            }

            if (FormAlpha > 0.5 && Font.get() != null)
            {
                float DrawHeight = Position.Y - Size.Y / 2;
                float FontHeight = Font.get().MeasureString(Name.get()).Y;
                DrawHeight -= FontHeight * 2;

                string CostString = "";
                if (MyCard.FactionCostIncreases.ContainsKey(ParentShip.FactionNumber))
                    CostString = "$" + (MyCard.CardCellsCost + MyCard.CardCellsCostIncrease * MyCard.FactionCostIncreases[ParentShip.FactionNumber]).ToString();
                else
                    CostString = "$" + (MyCard.CardCellsCost).ToString();

                Render.DrawShadowedText(Font.get(), Text.get(), new Vector2(Position.X - Font.get().MeasureString(Text.get()).X / 2, DrawHeight),
                    Vector2.One * 2, Color.White * (FormAlpha - 0.5f) * 2, Color.Black * (FormAlpha - 0.5f) * 2);
                DrawHeight -= FontHeight;
                Render.DrawShadowedText(Font.get(), CostString, new Vector2(Position.X - Font.get().MeasureString(CostString).X / 2, DrawHeight),
                    Vector2.One * 2, Color.White * (FormAlpha - 0.5f) * 2, Color.Black * (FormAlpha - 0.5f) * 2);
                if (FactionManager.GetFaction(ParentShip.FactionNumber).PickingCards)
                {
                    Vector2 s = Font.get().MeasureString(MyCard.Caption);
                    DrawHeight -= s.Y + 20;
                    //DrawHeight += 2 * FontHeight;
                    Render.DrawSprite(HudItem.GlowTexture, new Vector2(Position.X, DrawHeight + s.Y / 2), s * 6,
                        0, Color.Black * (FormAlpha - 0.5f) * 2);
                    Render.DrawShadowedText(Font.get(), MyCard.Caption, new Vector2(Position.X - Font.get().MeasureString(MyCard.Caption).X / 2, DrawHeight),
                        Vector2.One * 2, Color.White * (FormAlpha - 0.5f) * 2, Color.Black * (FormAlpha - 0.5f) * 2);
                }

                if (!MyCard.StrongVs.Equals(""))
                {
                    Render.DrawSprite(MyCard.GetStrongTexture(), Position - Size * 3 / 8 * new Vector2(1, -1), Size / 2, 0, Color.White * (FormAlpha * FormAlpha));
                    Render.DrawShadowedText(Font.get(), MyCard.StrongVs,
                        new Vector2(Position.X - Size.X * 1 / 8, Position.Y - Font.get().MeasureString(MyCard.StrongVs).Y * 3 / 4 + Size.Y / 2),
                        Vector2.One * 2, Color.White * (FormAlpha - 0.5f) * 2, Color.Black * (FormAlpha - 0.5f) * 2);
                }
            }

            Render.DrawOutlineRect(Position - FormSize * (FormAlpha - 0.5f), Position + FormSize * (FormAlpha - 0.5f), 2 * FormAlpha, Color.White * (FormAlpha * FormAlpha * FormAlpha));

            base.DrawAsForm(Position, FormSize);
        }

        public static void BuildAllTurrets(FormFrame frame)
        {
            int x;
            for (int y = 0; y < 5; y++)
            {
                x = 0;
                foreach (TurretCard c in FactionCard.SortedTurretDeck[y])
                {
                    TurretForm t = new TurretForm();
                    GameManager.GetLevel().AddObject(t);
                    frame.Add(t);

                    string s = c.GetUnitImagePath().Equals("") ?
                        "Textures/ShipGame/TurretPictures/" + c.GetImagePath() :
                        "Textures/ShipGame/UnitPictures/" + c.GetUnitImagePath();

                    t.SetValues(AssetManager.Load<Texture2D>(s), AssetManager.Load<SpriteFont>("Fonts/ShipGame/EventFont"), c.Name,
                        c.GetColor(), new Vector2(150 * x, 150 * y), new Vector2(125), c);
                    x++;
                }
            }
            frame.Commit("AllTurrets", true);
        }

        public static void BuildSelectedTurrets(FormFrame frame, int FactionNumber)
        {
            frame.ClearForms();
            frame.Commit("AllTurrets", true);
            for (int x = 0; x < Faction.MaxCards; x++)
            {
                TurretCard c = (TurretCard)FactionManager.Factions[FactionNumber].Cards[x];
                TurretForm t = new TurretForm();
                GameManager.GetLevel().AddObject(t);
                frame.Add(t);

                string s = c.GetUnitImagePath().Equals("") ?
                    "Textures/ShipGame/TurretPictures/" + c.GetImagePath() :
                    "Textures/ShipGame/UnitPictures/" + c.GetUnitImagePath();

                t.SetValues(AssetManager.Load<Texture2D>(s), AssetManager.Load<SpriteFont>("Fonts/ShipGame/EventFont"), c.Name,
                    c.GetColor(), new Vector2(150 * x, 0), new Vector2(125), c);
            }
            
            frame.Commit("AllTurrets", false);
            frame.DeActivate();
        }
    }
}
