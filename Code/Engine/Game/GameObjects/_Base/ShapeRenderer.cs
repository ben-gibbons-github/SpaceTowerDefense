using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ShapeRenderer
    {
        private static int sphereResolution = 30;
        private static int sphereLineCount = (sphereResolution + 1) * 3;
        private static VertexPositionColor[] SphereVerteces;
        private static short[] SphereIndicies;

        private static VertexPositionColor[] CubeVerteces;
        private static short[] CubeIndicies;

        public static _3DEffect ColorEffectHolder;
        private static bool Loaded = false;
        private static _3DEffect CubeEffectHolder;
        private static Model CubeModel;

        public static void Load()
        {
            if (!Loaded)
            {
                CubeModel = AssetManager.Load<Model>("Models/Editor/Cube");
                CubeEffectHolder = (_3DEffect)new _3DEffect().Create("Effects/CubeEffect");

                Loaded = true;
                ColorEffectHolder = (_3DEffect)new _3DEffect().Create("Effects/ColorEffect");
                LoadSphere();
                LoadCube();
            }
        }

        private static void LoadCube()
        {
            BoundingBox box = new BoundingBox(Vector3.One * -.5f, Vector3.One * 0.5f);
            Vector3[] corners = box.GetCorners();
            Color color = Color.White;
            CubeVerteces = new VertexPositionColor[24];
            CubeIndicies = new short[24];
            CubeVerteces[0] = new VertexPositionColor(corners[0], color);
            CubeVerteces[1] = new VertexPositionColor(corners[1], color);
            CubeVerteces[2] = new VertexPositionColor(corners[1], color);
            CubeVerteces[3] = new VertexPositionColor(corners[2], color);
            CubeVerteces[4] = new VertexPositionColor(corners[2], color);
            CubeVerteces[5] = new VertexPositionColor(corners[3], color);
            CubeVerteces[6] = new VertexPositionColor(corners[3], color);
            CubeVerteces[7] = new VertexPositionColor(corners[0], color);

            // Fill in the vertices for the top of the box
            CubeVerteces[8] = new VertexPositionColor(corners[4], color);
            CubeVerteces[9] = new VertexPositionColor(corners[5], color);
            CubeVerteces[10] = new VertexPositionColor(corners[5], color);
            CubeVerteces[11] = new VertexPositionColor(corners[6], color);
            CubeVerteces[12] = new VertexPositionColor(corners[6], color);
            CubeVerteces[13] = new VertexPositionColor(corners[7], color);
            CubeVerteces[14] = new VertexPositionColor(corners[7], color);
            CubeVerteces[15] = new VertexPositionColor(corners[4], color);

            // Fill in the vertices for the vertical sides of the box
            CubeVerteces[16] = new VertexPositionColor(corners[0], color);
            CubeVerteces[17] = new VertexPositionColor(corners[4], color);
            CubeVerteces[18] = new VertexPositionColor(corners[1], color);
            CubeVerteces[19] = new VertexPositionColor(corners[5], color);
            CubeVerteces[20] = new VertexPositionColor(corners[2], color);
            CubeVerteces[21] = new VertexPositionColor(corners[6], color);
            CubeVerteces[22] = new VertexPositionColor(corners[3], color);
            CubeVerteces[23] = new VertexPositionColor(corners[7], color);
            for(int i=0;i<24;i++)
                CubeIndicies[i] = (short)(i);
        }

        private static void LoadSphere()
        {

            Vector3[] unitSphere = new Vector3[sphereLineCount * 2];

            // Compute our step around each circle
            float step = MathHelper.TwoPi / sphereResolution;

            // Used to track the index into our vertex array
            int index = 0;

            // AddCards the loop on the XY plane first
            for (float a = 0f; a < MathHelper.TwoPi; a += step)
            {
                unitSphere[index++] = new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f);
                unitSphere[index++] = new Vector3((float)Math.Cos(a + step), (float)Math.Sin(a + step), 0f);
            }

            // Next on the XZ plane
            for (float a = 0f; a < MathHelper.TwoPi; a += step)
            {
                unitSphere[index++] = new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a));
                unitSphere[index++] = new Vector3((float)Math.Cos(a + step), 0f, (float)Math.Sin(a + step));
            }

            // Finally on the YZ plane
            for (float a = 0f; a < MathHelper.TwoPi; a += step)
            {
                unitSphere[index++] = new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a));
                unitSphere[index++] = new Vector3(0f, (float)Math.Cos(a + step), (float)Math.Sin(a + step));
            }

            SphereVerteces = new VertexPositionColor[sphereLineCount * 2];
            SphereIndicies = new short[sphereLineCount * 2];

            for (int i = 0; i < unitSphere.Length; i++)
            {
                Vector3 vertPos = unitSphere[i] / 2;
                SphereVerteces[i] = new VertexPositionColor(vertPos, Color.White);
                SphereIndicies[i] = (short)(i);
            }
        }

        public static void DrawSphere(Matrix World, Camera3D camera, Vector4 Color)
        {
            ColorEffectHolder.SetFromCamera(camera);
            ColorEffectHolder.SetWorld(World);
            ColorEffectHolder.MyEffect.Parameters["ObjectColor"].SetValue(Color);
            ColorEffectHolder.Apply();
            Game1.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
            PrimitiveType.LineList, SphereVerteces, 0, sphereLineCount * 2, SphereIndicies, 0, sphereLineCount);
        }

        public static void DrawCube(Matrix World, Camera3D camera, Vector4 Color)
        {
            ColorEffectHolder.SetFromCamera(camera);
            ColorEffectHolder.SetWorld(World);
            ColorEffectHolder.MyEffect.Parameters["ObjectColor"].SetValue(Color);
            ColorEffectHolder.Apply();
            Game1.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
            PrimitiveType.LineList, CubeVerteces, 0, 24, CubeIndicies, 0, 12);
        }

        public static void DrawSphere(Matrix World, Camera3D camera, RenderTargetCube Cube)
        {
            CubeEffectHolder.SetFromCamera(camera);
            CubeEffectHolder.SetWorld(World);
            CubeEffectHolder.MyEffect.Parameters["ShadowReference"].SetValue(Cube);
            Render.DrawModel(CubeModel, CubeEffectHolder.MyEffect);
        }
    }
}
