using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShipGame.Wave;

namespace BadRabbit.Carrot.WaveFSM
{
    public class PickEnemyState : WaveState
    {
        public static PickEnemyState self;
        public static int RandomRounds = 4;

        static PickEnemyState()
        {
            self = new PickEnemyState();
        }

        int Timer = 0;
        int MaxTimer = 30000;

        float SizeBonusChange = 0.05f;
        int CardCount = 3;

        bool SinglePlayer = false;
        int SinglePlayerCounter = 0;

        int SingleTimer = 0;
        int MaxSingleTimer = 500;

        int AfterTimer = 0;
        int MaxAfterTimer = 1000;
        bool MoneyMade = false;

        public override void Enter()
        {
            NeutralManager.MyPattern.CurrentCard = null;
            MoneyMade = false;
            MaxAfterTimer = Math.Min(7500, (int)(2000 * WaveManager.CurrentWave / WaveCard.LevelMult * (FactionManager.TeamCount > 1 ? 3 : 1)));
            OverCardPicker.Ready = false;
            CardCount = 3;
            SinglePlayer = FactionManager.TeamCount == 1;
            OverCardPicker.SinglePlayer = SinglePlayer;

            SinglePlayerCounter = Rand.r.Next(CardCount * 10) + CardCount * 10;

            Timer = 0;
            AfterTimer = 0;

            OverCardPicker.CanPick = false;
            OverCardPicker.Reset(CardCount);

            base.Enter();
        }

        public override void Update(GameTime gameTime)
        {
            if (!OverCardPicker.Ready)
            {
                Timer += gameTime.ElapsedGameTime.Milliseconds;

                if (Timer < MaxTimer)
                {
                    if (OverCardPicker.SizeBonus < 1)
                        OverCardPicker.SizeBonus += SizeBonusChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                    else
                    {
                        OverCardPicker.CanPick = true;
                        OverCardPicker.SizeBonus = 1;

                        if (SinglePlayer || WaveManager.CurrentWave < RandomRounds + 1)
                        {
                            SingleTimer += gameTime.ElapsedGameTime.Milliseconds;
                            if (SingleTimer > (MaxSingleTimer - SinglePlayerCounter * 25) / 16)
                            {
                                SingleTimer = 0;
                                if (SinglePlayerCounter > 0)
                                {
                                    SinglePlayerCounter--;
                                    OverCardPicker.SingleMove();
                                    SoundManager.PlaySound("Ready", 1, 0, 0);
                                }
                                else
                                {
                                    OverCardPicker.ReadySingle();
                                    SoundManager.PlaySound("MenuOpen", 1, 0, 0);
                                }
                            }
                        }
                    }
                }
                else
                    OverCardPicker.ReadyTeamNow();
            }
            else
            {
                if (!MoneyMade)
                {
                    MoneyMade = true;
                    FactionManager.MarkMoney();
                    OverMap.TargetMax = new Vector2(-100000);
                    OverMap.TargetMin = new Vector2(100000);
                    foreach (MiningPlatform m in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(MiningPlatform)))
                        m.MakeMoney();
                    foreach (Bank m in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(Bank)))
                        m.MakeMoney();
                    FactionManager.EventMoney();
                    OverMap.NewCard();
                }

                AfterTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (AfterTimer > MaxAfterTimer)
                {
                    if (OverCardPicker.SizeBonus > 0)
                        OverCardPicker.SizeBonus -= SizeBonusChange * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                    else
                    {
                        OverCardPicker.SizeBonus = 0;
                        WaveManager.CurrentStrike = null;
                        WaveManager.NewEvent(GameManager.GetLevel().getCurrentScene());

                        if (WaveManager.CurrentStrike == null)
                            WaveManager.SetState(WaveStepState.self);
                        else
                            WaveManager.SetState(PickStrikeState.self);
                    }
                }
            }

            base.Update(gameTime);
        }
    }
}
