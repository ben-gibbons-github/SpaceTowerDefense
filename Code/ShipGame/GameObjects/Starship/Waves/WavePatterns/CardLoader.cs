using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShipGame.Wave;
using System.IO;
using Microsoft.Xna.Framework;

#if WINDOWS && EDITOR
using System.Windows.Forms;
#endif

namespace BadRabbit.Carrot
{
    public class CardLoader
    {
        private enum ProcessStep
        {
            Level,
            MinDiff,
            MaxDiff,
            EnergyCost,
            Name,
            Type,
            Description,
            Special,
            PathName,
            UName, UCount, ULevel
        }

        public static Dictionary<int, List<WaveCard>> SortedCardLists = new Dictionary<int, List<WaveCard>>();

        public static WaveCard GetRandomCard(int Wave)
        {
            return SortedCardLists[Wave - 1][Rand.r.Next(SortedCardLists[Wave - 1].Count)];
        }

        public static void FillCards(string[] Sources)
        {
            SortedCardLists.Clear();
            LinkedList<string> ReadFiles = new LinkedList<string>();

            foreach (string Fname in Sources)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(TitleContainer.OpenStream("Content/Extras/ShipGame/WavePatterns/" + Fname + ".txt")))
                    {
                        String line = sr.ReadToEnd();
                        ReadFiles.AddLast(line);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            AddCards(ReadFiles);
        }

#if WINDOWS && EDITOR
        public static void FillCardsFromLoader()
        {
            SortedCardLists.Clear();
            LinkedList<string> ReadFiles = new LinkedList<string>();
            OpenFileDialog openFileDialog1;

            openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "cardLevel files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(File.OpenRead(openFileDialog1.FileName)))
                    {
                        String line = sr.ReadToEnd();
                        ReadFiles.AddLast(line);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            AddCards(ReadFiles);
        }
#endif

        private static void AddCards(LinkedList<string> SourcesFiles)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ProcessStep processStep = ProcessStep.Level;
            WaveCard newCard = new WaveCard();
            string UnitName = "";
            int UnitCount = 0;
            float UnitLevel = 0;

            foreach (string SourceFile in SourcesFiles)
            {
                foreach (char c in SourceFile)
                {
                    if (c == ';' || c == ',' || c == '!')
                    {
                        switch (processStep)
                        {
                            case ProcessStep.Level:
                                string s = stringBuilder.ToString();
                                newCard.Level = int.Parse(s);
                                processStep = ProcessStep.MinDiff;
                                break;
                            case ProcessStep.MinDiff:
                                newCard.MinDiff = int.Parse(stringBuilder.ToString());
                                processStep = ProcessStep.MaxDiff;
                                break;
                            case ProcessStep.MaxDiff:
                                newCard.MaxDiff = int.Parse(stringBuilder.ToString());
                                processStep = ProcessStep.EnergyCost;
                                break;
                            case ProcessStep.EnergyCost:
                                newCard.EnergyCost = int.Parse(stringBuilder.ToString());
                                processStep = ProcessStep.Name;
                                break;
                            case ProcessStep.Name:
                                newCard.Name = stringBuilder.ToString();
                                processStep = ProcessStep.Type;
                                break;
                            case ProcessStep.Type:
                                newCard.SetType(stringBuilder.ToString());
                                processStep = ProcessStep.Description;
                                break;
                            case ProcessStep.Description:
                                newCard.Description = stringBuilder.ToString();
                                processStep = ProcessStep.Special;
                                break;
                            case ProcessStep.Special:
                                newCard.AddSpecial(stringBuilder.ToString());
                                processStep = c == ',' ? ProcessStep.Special : ProcessStep.PathName;
                                break;
                            case ProcessStep.PathName:
                                newCard.SetImagePath(stringBuilder.ToString());
                                processStep = ProcessStep.UName;
                                break;
                            case ProcessStep.UName:
                                UnitName = stringBuilder.ToString();
                                processStep = ProcessStep.UCount;
                                break;
                            case ProcessStep.UCount:
                                UnitCount = int.Parse(stringBuilder.ToString());
                                processStep = ProcessStep.ULevel;
                                break;
                            case ProcessStep.ULevel:
                                UnitLevel = int.Parse(stringBuilder.ToString());
                                newCard.AddUnit(new WaveUnit(FactionCard.GetFactionUnitCard(UnitName), UnitCount, UnitLevel));
                                if (c == ';')
                                    processStep = ProcessStep.UName;
                                else
                                {
                                    processStep = ProcessStep.Level;
                                    if (!SortedCardLists.ContainsKey(newCard.Level - 1))
                                        SortedCardLists.Add(newCard.Level - 1, new List<WaveCard>());
                                    SortedCardLists[newCard.Level - 1].Add(newCard);
                                    newCard = new WaveCard();
                                    newCard.Super = FactionCard.SuperMode;
                                }
                                break;
                        }
                        stringBuilder = new StringBuilder();
                    }
                    else if (c != ' ' && c != '\n' && c != '\r')
                        stringBuilder.Append(c);
                }
            }
        }

    }
}
