using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class LineModel : Basic3DObject, WorldViewer3D
    {
        IntValue ModelCount;
        private int ModelCountPrevious;

        ModelValue[] Models;
        LineModelItem[] Items;
        int CurrentItem;

        IntValue LineFlares;
        IntValue RandomFlares;
        IntValue MaxItemTime;
        int ItemTime;

        FloatValue CameraSpeed;
        Camera3D MyCamera;
        float WorldCameraInterpolation;
        Vector3 WorldCameraFrom;
        Vector3 WorldCameraTo;
        Vector3 WorldCameraLookAt;
        SceneView sceneView;

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
            ModelCount = new IntValue("Model Count");
            ModelCount.ChangeEvent = ModelCountChange;

            LineFlares = new IntValue("Line Flares", 100);
            RandomFlares = new IntValue("Random Flare", 100);
            MaxItemTime = new IntValue("Item Time", 10000);

            CameraSpeed = new FloatValue("Camera Speed", 0.1f);
            AddTag(GameObjectTag.Update);
            AddTag(GameObjectTag.WorldViewer);
            MyCamera = new Camera3D(MathHelper.PiOver4, 0.1f, 1000000);

            WorldCameraInterpolation = 1;

            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            ItemTime += gameTime.ElapsedGameTime.Milliseconds;
            if (ItemTime > MaxItemTime.get())
            {
                ItemTime -= MaxItemTime.get();

                CurrentItem++;
                if (CurrentItem > ModelCount.get() - 1)
                    CurrentItem = 0;
            }
            if (Items[CurrentItem] == null)
                Items[CurrentItem] = new LineModelItem(Models[CurrentItem].get(), LineFlares.get(), RandomFlares.get());
            Items[CurrentItem].Update(gameTime);

            WorldCameraInterpolation += gameTime.ElapsedGameTime.Milliseconds * 60 / 100000f * CameraSpeed.get();

            if (WorldCameraInterpolation > 1)
            {
                WorldCameraInterpolation = 0;
                WorldCameraFrom = Rand.V3() * Items[CurrentItem].Distance * 3;
                WorldCameraTo = Rand.V3() * Items[CurrentItem].Distance * 3;
                WorldCameraLookAt = Items[CurrentItem].Points[Rand.r.Next(Items[CurrentItem].Points.Count)] / 2;
            }

            MyCamera.SetLookAt(Vector3.Lerp(WorldCameraFrom, WorldCameraTo, WorldCameraInterpolation), WorldCameraLookAt);

            base.Update(gameTime);
        }

        void ModelCountChange()
        {
            ModelValue[] NewModels = new ModelValue[ModelCount.get()];

            for (int i = 0; i < Math.Max(ModelCount.get(), ModelCountPrevious); i++)
            {
                if (i < ModelCount.get())
                {
                    if (i < ModelCountPrevious)
                    {
                        NewModels[i] = Models[i];
                    }
                    else
                    {
                        Level.ReferenceObject = this;
                        NewModels[i] = new ModelValue("Model " + i.ToString());
                    }
                }
                else
                {
                    RemoveValue(Models[i]);
                }
            }

            Models = NewModels;
            Items = new LineModelItem[ModelCount.get()];
            ModelCountPrevious = ModelCount.get();

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                ParentScene.UpdateSelected();
            }
#endif
        }
    }
}
