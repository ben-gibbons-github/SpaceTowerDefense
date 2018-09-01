using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShipGame.Wave;

namespace BadRabbit.Carrot
{
    public class OverMap : SizeNodeObject
    {
        public static Texture2D HudWaveText;
        public static SpriteFont WaveFont;
        public static float SizeMult = 0;
        public static string UnitSubtitle;

        SizeNode RectL;
        SizeNode RectR;

        Vector2 InterpolatedPositionL;
        Vector2 InterpolatedPositionR;
        Vector2 InterpolatedDifference;

        public static Vector2 TargetMin;
        static Vector2 Min;

        public static Vector2 TargetMax;
        static Vector2 Max;

        LinkedList<GameObject> AllUnits;

        static Texture2D CurrentIcon;
        static string CurrentString;
        static Color CurrentColor;

        float Alpha = 0;
        static bool Draw = false;

        public OverMap()
        {
            HudWaveText = AssetManager.Load<Texture2D>("Textures/ShipGame/HUD/HudComponents/HudWaveText");
            WaveFont = AssetManager.Load<SpriteFont>("Fonts/ShipGame/WaveFont");
        }

        public override void Create()
        {
            AllUnits = GameManager.GetLevel().getCurrentScene().GetList(GameObjectTag.ShipGameUnitBasic);

            AddTag(GameObjectTag.OverDrawViews);
            AddTag(GameObjectTag.Update);

            RectL = AddSizeNode(new SizeNode(new Vector2(600, 350)));
            RectR = AddSizeNode(new SizeNode(new Vector2(900, 650)));

            base.Create();
        }

        public static void NewCard()
        {
            CurrentString = "Wave : " + WaveManager.CurrentWave.ToString() + "\n" + NeutralManager.MyPattern.CurrentCard.Type;
            CurrentIcon = NeutralManager.MyPattern.CurrentCard.MyIconTexture;
            CurrentColor = NeutralManager.MyPattern.CurrentCard.typeColor;
            Draw = true;
        }

        public static void NoCard()
        {
            Draw = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Draw)
            {
                Alpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 100000f;
                if (Alpha < 0)
                {
                    Alpha = 0;
                    CurrentString = null;
                    CurrentIcon = null;
                }
            }
            else
            {   
                Alpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 100000f;
                if (Alpha > 1)
                    Alpha = 1;
            }

            InterpolatedPositionL = RectL.Position /
                OverFormat.BaseScreenSize * MasterManager.FullScreenSize;
            InterpolatedPositionR = RectR.Position /
                OverFormat.BaseScreenSize * MasterManager.FullScreenSize;
            InterpolatedDifference = InterpolatedPositionR - InterpolatedPositionL;

            float MoveAmount = gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * 5;
            if (Vector2.Distance(Min, TargetMin) > MoveAmount)
                Min += Vector2.Normalize(TargetMin - Min) * MoveAmount;
            else
                Min = TargetMin;

            if (Vector2.Distance(Max, TargetMax) > MoveAmount)
                Max += Vector2.Normalize(TargetMax - Max) * MoveAmount;
            else
                Max = TargetMax;

            base.Update(gameTime);
        }

        public override void UpdateViewsEvent(int ViewCount)
        {
            if (ViewCount < 3)
            {
                RectL.TargetPosition = new Vector2(700, 350);
                RectR.TargetPosition = new Vector2(900, 650);
            }
            else
            {
                RectL.TargetPosition = new Vector2(350, 350);
                RectR.TargetPosition = new Vector2(650, 650);
            }

            base.UpdateViewsEvent(ViewCount);
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            if (Alpha == 0)
                return;

            float Size = InterpolatedDifference.X * SizeMult;
            Vector2 Position = (InterpolatedPositionL + InterpolatedPositionR) / 2 - new Vector2(Size / 2);

            if (CurrentIcon == null)
            {
                if (NeutralManager.MyPattern.CurrentCard.MyIconTexture == null)
                {
                    switch (NeutralManager.MyPattern.CurrentCard.Type)
                    {
                        case "Light":
                            CurrentIcon = WaveCard.LightIcon;
                            break;
                        case "Medium":
                            CurrentIcon = WaveCard.MediumIcon;
                            break;
                        case "Heavy":
                            CurrentIcon = WaveCard.HeavyIcon;
                            break;
                    }
                }
                else
                    CurrentIcon = NeutralManager.MyPattern.CurrentCard.MyIconTexture;
            }

            Render.DrawSprite(CurrentIcon, Position + new Vector2(Size) / 2,
                new Vector2(Size * Alpha), 0, Color.White * 0.5f * Alpha);
            Render.DrawShadowedText(WaveFont,
                CurrentString,
                Position + new Vector2(Size / 4, Size), Vector2.One,
                CurrentColor * SizeMult * Alpha, Color.Black * SizeMult * Alpha);

            return;

            
            Render.DrawSprite(Render.BlankTexture, Position + new Vector2(Size) / 2, new Vector2(Size), 0, Color.Black * 0.3f);

            foreach (WallChain n in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(WallChain)))
                n.DrawFromMiniMap(Position, Size, Min, Max);

            foreach (BasicShipGameObject u in AllUnits)
                u.DrawFromMiniMap(Position, Size, Min, Max);

            Render.DrawOutlineRect(Position, Position + new Vector2(Size), 1, Color.White);
            //Render.DrawSprite(HudWaveText, Position + new Vector2(Size / 3, Size * 1.1f), new Vector2(Size * 0.6f, Size * 0.2f), 0);
            //DigitRenderer.DrawDigits(WaveManager.CurrentWave, 2, Position + new Vector2(Size / 1.5f, Size * 1.1f), new Vector2(Size * 0.25f, Size * 0.15f), Color.White);
            
            base.Draw2D(DrawTag);
        }
    }
}
