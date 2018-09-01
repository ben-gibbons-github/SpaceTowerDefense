using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BadRabbit.Carrot;
using System.IO;

namespace ShipGame.Wave
{
    public class WavePattern
    {
        public static string[] WaveMultiplayerSources = { "MPlay" };

        public WaveCard CurrentCard;

        public WavePattern(int CardCount)
        {
            FillCards(WaveMultiplayerSources);
        }

        public WavePattern(int CardCount, params string[] Sources)
        {
            FillCards(Sources);
        }

        public void Update(GameTime gameTime)
        {
            if (WaveManager.CurrentWave > 0 && CurrentCard != null)
                CurrentCard.Update(gameTime);
        }

        private void FillCards(string[] Sources)
        {
#if WINDOWS && EDITOR
            CardLoader.FillCardsFromLoader();
#endif
#if !WINDOWS || !EDITOR
            CardLoader.FillCards(Sources);
#endif
            PickRandom(CardLoader.SortedCardLists);
        }

        private void PickRandom(Dictionary<int, List<WaveCard>> SortedCardLists)
        {
            CurrentCard = CardLoader.GetRandomCard(1);
        }

        public void WaveStart()
        {
            CurrentCard.WaveStart();
        }

        public void WaveEvent()
        {
            CurrentCard.WaveEvent();
        }

        internal void EndWave()
        {
            CurrentCard.EndWave();
        }
    }
}
