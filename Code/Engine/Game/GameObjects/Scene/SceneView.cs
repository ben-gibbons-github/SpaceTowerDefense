using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class SceneView
    {
        private Viewport MyView;
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 UVMult = Vector2.One;
        public Vector2 UVOffset = Vector2.Zero;
        public int Index;

        public SceneView(int Width, int Height, int X, int Y, Vector2 UVMult, Vector2 UVOffset, int Index)
        {
            MyView = new Viewport(X, Y, Width, Height);
            Position = new Vector2(X, Y);
            Size = new Vector2(Width, Height);
            this.UVMult = UVMult;
            this.UVOffset = UVOffset;
            this.Index = Index;
        }

        public bool GetLeft()
        {
            return (int)Position.X == 0;
        }

        public bool GetTop()
        {
            return (int)Position.Y == 0;
        }

        public void Set()
        {
            try
            {
                Game1.graphicsDevice.Viewport = MyView;
            }
            catch (Exception e)
            {
                MasterManager.e = e;
                Console.WriteLine(e.Message);
            }
        }

        public void SetCamera(Camera2D camera)
        {
            Render.ViewWidth = MyView.Width;
            Render.ViewHeight = MyView.Height;
            camera.SetSize(new Vector2(MyView.Width, MyView.Height));
        }

        public void SetCamera(Camera3D camera, Vector2 WindowSize)
        {
            Render.ViewWidth = MyView.Width;
            Render.ViewHeight = MyView.Height;
            if (camera != null)
            {
                camera.SetSize(new Vector2(MyView.Width, MyView.Height));
                camera.SetMult(UVMult);
                //camera.SetOffset((WindowSize / 2 - (new Vector2(MyView.X, MyView.Y) + new Vector2(MyView.Width, MyView.Height) / 2)) * UVOffsetMult / WindowSize);
                camera.SetOffset(UVOffset);
            }
        }

        public static SceneView[] GetViews(int Count, Vector2 WindowSize)
        {
            SceneView[] views = new SceneView[Count];
            for (int i = 0; i < Count; i++)
            {
                views[i] = GetView(i, Count, WindowSize);
            }
            return views;
        }

        private static SceneView GetView(int Index, int Count, Vector2 WindowSize)
        {
            int SceneWidth = (int)WindowSize.X;
            int SceneHeight = (int)WindowSize.Y;

            switch (Count)
            {
                case 1:
                    return new SceneView(SceneWidth, SceneHeight, 0, 0, Vector2.One, Vector2.Zero, 0);
                case 2:
                    if (Index == 0)
                        return new SceneView(SceneWidth, SceneHeight / 2, 0, 0, new Vector2(1, 0.5f), new Vector2(0, 0), 0);
                    else
                        return new SceneView(SceneWidth, SceneHeight / 2, 0, SceneHeight / 2, new Vector2(1, 0.5f), new Vector2(0, 0.5f), 1);
                case 3:
                    if (Index == 0)
                        return new SceneView(SceneWidth, SceneHeight / 2, 0, 0, new Vector2(1, 0.5f), new Vector2(0, 0), 0);
                    else if (Index == 1)
                        return new SceneView(SceneWidth / 2, SceneHeight / 2, 0, SceneHeight / 2, new Vector2(0.5f, 0.5f), new Vector2(0, 0.5f), 1);
                    else
                        return new SceneView(SceneWidth / 2, SceneHeight / 2, SceneWidth / 2, SceneHeight / 2, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 2);
                case 4:
                    if (Index == 0)
                        return new SceneView(SceneWidth / 2, SceneHeight / 2, 0, 0, new Vector2(0.5f, 0.5f), new Vector2(0, 0), 0);
                    else if (Index == 1)
                        return new SceneView(SceneWidth / 2, SceneHeight / 2, SceneWidth / 2, 0, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0), 1);
                    else if (Index == 2)
                        return new SceneView(SceneWidth / 2, SceneHeight / 2, 0, SceneHeight / 2, new Vector2(0.5f, 0.5f), new Vector2(0, 0.5f), 2);
                    else
                        return new SceneView(SceneWidth / 2, SceneHeight / 2, SceneWidth / 2, SceneHeight / 2, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), 3);
            }
            return null;
        }
    }
}
