using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ShipLighting : GameObject
    {
        public static Vector4 WorldAmbientLightColor = Vector4.One;
        public static Vector3 WorldLightOneDirection;
        public static Vector4 WorldLightOneColor;
        public static Vector3 WorldLightTwoDirection;
        public static Vector4 WorldLightTwoColor;

        public ColorValue AmbientLightColor;

        public Vector3Value LightOneDirection;
        public ColorValue LightOneColor;

        public Vector3Value LightTwoDirection;
        public ColorValue LightTwoColor;

        public override void Create()
        {
            AmbientLightColor = new ColorValue("Ambient Light Color", new Vector4(0.5f, 0.5f, 0.5f, 1), 0.2f);
            LightOneDirection = new Vector3Value("Light One Direction", Vector3.One);
            LightOneColor = new ColorValue("Light One Color", new Vector4(1, 0.9f, 0.85f, 1));
            LightTwoDirection = new Vector3Value("Light Two Direction", new Vector3(-1, 1, -1));
            LightTwoColor = new ColorValue("Light Two Color", new Vector4(0.1f, 0.3f, 0.65f, 1));

            base.Create();
        }

        public override void CreateInGame()
        {
            WorldAmbientLightColor = AmbientLightColor.get();
            WorldLightOneDirection = Vector3.Normalize(LightOneDirection.get());
            WorldLightOneColor = LightOneColor.get();
            WorldLightTwoDirection = Vector3.Normalize(LightTwoDirection.get());
            WorldLightTwoColor = LightTwoColor.get();

            base.CreateInGame();
        }
    }
}
