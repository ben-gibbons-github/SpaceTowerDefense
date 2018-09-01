using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class Camera3DObject : Basic3DObject, WorldViewer3D
    {
        public static bool Loaded = false;
#if EDITOR && WINDOWS
        public static _3DEffect ColorEffectHolder;
        public static Color[] PointColors = { new Color(1, 0.2f, 0.2f), new Color(1, 1, 0.2f), new Color(0.5f, 0.5f, 1) };
        public static int PointCount;
        public static VertexPositionColor[] PointList;
        public static short[] Indicies;
#endif

        public Camera3D MyCamera;
        public PlayerProfile MyPlayer;
        Vector3 cameraFront = new Vector3(0, 0, -1);

        public FloatValue ZoomDistance;
        public BoolValue IsWorldViewer;
        public BoolValue AllowTakeover;

        private SceneView sceneView;

        public Camera3D getCamera()
        {
            return MyCamera;
        }

        public SceneView getSceneView()
        {
            return sceneView;
        }

        public void setSceneView(SceneView sceneView)
        {
            this.sceneView = sceneView;
        }

        public override void Create()
        {
            AddTag(GameObjectTag.WorldViewer);

            base.Create();

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                AddTag(GameObjectTag._3DForward);
                Load();
            }
#endif

            AddTag(GameObjectTag.Update);
            MyCamera = new Camera3D(MathHelper.PiOver4, 0.1f, 1000000);

            Values.Remove(Scale);
            ZoomDistance = new FloatValue("Zoom Offset",1000,ChangeScale);
            IsWorldViewer = new BoolValue("World Viewer", true);
            IsWorldViewer.ChangeEvent = ChangeWorldViewer;
            AllowTakeover = new BoolValue("AllowTakeover");

            ChangeScale();
            Rotation.set(new Vector3(45,45,0));

            SetCollisionShape(new Point3D());
        }

        private void ChangeWorldViewer()
        {
            if (IsWorldViewer.get())
                AddTag(GameObjectTag.WorldViewer);
            else
                RemoveTag(GameObjectTag.WorldViewer);
        }

        public override void Update(GameTime gameTime)
        {
            if (MyPlayer == null)
                ApplyRotate(new Vector3(0, 0.01f * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0), Position.get(), false);
            else
                MoveCamera(gameTime);

            base.Update(gameTime);
        }

        public void MoveCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            Vector2 RStick = MyPlayer.MyController.RightStickReset();

            float pitch = RStick.Y * time * 0.002f;
            float turn = -RStick.X * time * 0.002f;

            Vector3 cameraRight = Vector3.Cross(Vector3.Up, cameraFront);
            Vector3 flatFront = Vector3.Cross(cameraRight, Vector3.Up);

            Matrix pitchMatrix = Matrix.CreateFromAxisAngle(cameraRight, pitch);
            Matrix turnMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, turn);

            Vector3 tiltedFront = Vector3.TransformNormal(cameraFront, pitchMatrix *
                                                          turnMatrix);

            // Check angle so we cant flip over
            if (Vector3.Dot(tiltedFront, flatFront) > 0.001f)
            {
                cameraFront = Vector3.Normalize(tiltedFront);
            }

            Vector2 LStick = MyPlayer.MyController.LeftStick();


            Position.add(cameraFront * LStick.Y * time * 0.1f);
            Position.add(-cameraRight * LStick.X * time * 0.1f);
        }

#if EDITOR && WINDOWS
        new public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                ColorEffectHolder = (_3DEffect)new _3DEffect().Create("Editor/ColorEffect");

                PointCount = 6;
                PointList = new VertexPositionColor[PointCount];
                Indicies = new short[PointCount];
                int i = 0;

                PointList[i++] = new VertexPositionColor(
                   new Vector3(1, 0, 0), PointColors[0]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(-1, 0, 0), PointColors[0]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, 1, 0), PointColors[1]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, -1, 0), PointColors[1]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, 0, 1), PointColors[2]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, 0, -1), PointColors[2]);

                for (i = 0; i < PointCount; i += 1)
                    Indicies[i] = (short)(i);
            }   
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            ColorEffectHolder.SetFromObject(this);
            ColorEffectHolder.SetFromCamera(camera);

            foreach (EffectPass pass in ColorEffectHolder.MyEffect.CurrentTechnique.Passes)
            {

                pass.Apply();


                Game1.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
        PrimitiveType.LineList,
        PointList,
        0,  // vertex buffer offset to add to each element of the index buffer
        PointCount,  // number of vertices in pointList
        Indicies,  // the index buffer
        0,  // first index element to read
        PointCount / 2    // number of primitives to draw
            );
            }
        }
#endif


        public override void ChangePosition()
        {
            base.ChangePosition();
            ChangeLookAt();
        }

        public override void ChangeScale()
        {
            ScaleMatrix = Matrix.CreateScale(ZoomDistance.get() / 25);
            ChangeLookAt();
            UpdateWorldMatrix();
        }

        public override void ChangeRotation()
        {
            ChangeLookAt();
        }

        public void ChangeLookAt()
        {
            if (MyPlayer == null)
            {
                Vector3 Direction = Rotation.getAsRadians();
                Vector3 LookOffset = new Vector3(ZoomDistance.get());
                float OutMult = (float)Math.Cos(Direction.X);
                Vector3 LookFrom = Position.get() + new Vector3((float)Math.Cos(Direction.Y) * OutMult * LookOffset.X, LookOffset.Y * (float)Math.Sin(Direction.X), (float)Math.Sin(Direction.Y) * OutMult * LookOffset.X);
                MyCamera.SetLookAt(LookFrom, Position.get());
            }
            else
            {
                MyCamera.SetLookAt(Position.get(), Position.get() + cameraFront);
            }
        }
    }
}
