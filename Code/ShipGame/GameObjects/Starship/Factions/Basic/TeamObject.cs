using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class TeamInfo
    {
        public const int MaxTeams = 8;

        public  static Vector3[] Colors3 = { new Vector3(0.4f, 0.4f, 1), new Vector3(1, 0.4f, 0.4f), 
                                                new Vector3(1, 1, 0.1f), new Vector3(0.1f, 1, 0.1f), 
                                                new Vector3(1, 0.1f, 1), new Vector3(1f, 0.5f, 0.1f), 
                                                new Vector3(0.1f, 1, 1), new Vector3(0.5f, 0.1f, 1),
                                                new Vector3(0.5f, 0.5f, 0.5f) };
        public static Color[] Colors = new Color[9];
        public static Color[] HudColors = new Color[9];

        static TeamInfo()
        {
            for (int i = 0; i < Colors3.Length; i++)
            {
                Colors[i] = new Color(Colors3[i]);
                HudColors[i] = new Color(Vector3.One - (Vector3.One - Colors3[i]) / 1.5f);
            }
        }

        public static Color GetColor(int Team)
        {
            return Team != -1 ? Colors[Team] : Color.White;
        }
    }
}
