using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class GameFormsManager
    {
        public static float PlayerMarkMoveMult = 0.05f;
        public static Color[] PlayerColors = { new Color(1, 0.1f, 0.1f), new Color(0.1f, 0.1f, 1), new Color(0.1f, 1, 0.1f), new Color(1, 1, 0.1f) };
        public static Vector2[] PlayerOffsets = { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(1, 1) };
    }
}
