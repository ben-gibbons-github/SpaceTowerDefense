using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PolorGrapher : Basic3DObject, WorldViewer3D
    {
        float Time;
        FloatValue TimeSpeed;
        FloatValue CameraDistance;

        FloatValue CameraSpeed;
        Camera3D MyCamera;
        float WorldCameraInterpolation;
        Vector3 WorldCameraFrom;
        Vector3 WorldCameraTo;
        Vector3 WorldCameraLookAt;
        SceneView sceneView;

        PolorPoint MyPoint;

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
            CameraSpeed = new FloatValue("Camera Speed", 0.1f);
            CameraDistance = new FloatValue("Camera Distance", 100);

            AddTag(GameObjectTag.Update);
            AddTag(GameObjectTag.WorldViewer);
            MyCamera = new Camera3D(MathHelper.PiOver4, 0.1f, 1000000);

            TimeSpeed = new FloatValue("TimeSpeed", 1);

            WorldCameraInterpolation = 1;
            MyPoint = new PolorPoint();

            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            WorldCameraInterpolation += gameTime.ElapsedGameTime.Milliseconds * 60 / 100000f * CameraSpeed.get();

            if (WorldCameraInterpolation > 1)
            {
                WorldCameraInterpolation = 0;
                WorldCameraFrom = Rand.V3() * CameraDistance.get();
                WorldCameraTo = Rand.V3() * CameraDistance.get();
            }

            //WorldCameraLookAt = MyPoint.DrawPosition;
            MyCamera.SetLookAt(Vector3.Lerp(WorldCameraFrom, WorldCameraTo, WorldCameraInterpolation), WorldCameraLookAt);
            //MyCamera.SetLookAt(new Vector3(10,100,10), new Vector3(0,0,0));
            MyPoint.Update(gameTime);

            base.Update(gameTime);
        }
    }
}
