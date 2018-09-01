using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class HUDFormat
    {
        //scales are based on a 1000x1000 screen absolute size
        public static Vector2 BaseScreenSize = new Vector2(1280, 720);

        public static Vector2 MiniMapPosition = new Vector2(800, 150);
        public static float MiniMapSize = 350;

        public static Vector2 EventDisplayPosition = new Vector2(920, 375);
        public static Vector2 EventDisplaySize = new Vector2(250, 250);

        public static Vector2 HealthDisplayPosition = new Vector2(100, 560);
        public static Vector2 HealthDisplaySize = new Vector2(350, 100);

        public static Vector2 WeaponDisplayPosition = new Vector2(100, 460);
        public static Vector2 WeaponDisplaySize = new Vector2(300, 100);

        public static Vector2 FactionDisplayPosition = new Vector2(175, 125);
        public static float FactionIconSize = 80;
        public static float FactionMinIconSize = 40;

        public static Vector2 WavePosition = new Vector2(500, 200);
    }
}
