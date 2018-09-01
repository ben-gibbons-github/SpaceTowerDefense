using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot.SpyGame
{
    public class SpyPlayer : Basic3DObject, WorldViewer3D
    {
        private static Model PlayerModel;
        private static bool Loaded = false;
        private static Deferred3DEffect effect3D;

        PlayerProfile MyProfile;
        Camera3D MyCamera = new Camera3D();
        SceneView sceneView;

        float MoveSpeed = 1;
        float LookSpeed = 0.1f;
        
        float ZoomAmount = 200;
        float ZoomAngle = 0.4f;

        public SpyPlayer(PlayerProfile MyProfile)
        {
            this.MyProfile = MyProfile;
        }

        public override void Create()
        {
            AddTag(GameObjectTag.Update);
            AddTag(GameObjectTag._3DSolid);
            AddTag(GameObjectTag._3DForward);
            AddTag(GameObjectTag._3DDeferredGBuffer);
            AddTag(GameObjectTag.WorldViewer);

            base.Create();

            Load();
        }

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

        new private static void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                PlayerModel = AssetManager.Load<Model>("Models/SpyGame/PlayerTempFigure");
                effect3D = (Deferred3DEffect)new Deferred3DEffect().Create("Effects/dtex");
            }
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 Lstick = -MyProfile.MyController.LeftStick();
            float LLength = Lstick.Length();
            if (LLength > 0)
            {
                if (LLength > 0.5f)
                    LLength = 0.5f;
                Lstick = Vector2.Normalize(Lstick) * MoveSpeed / 1000f * 60f * gameTime.ElapsedGameTime.Milliseconds * LLength * 2;
            }

            Vector2 Rstick = -MyProfile.MyController.RightStick() * LookSpeed / 1000f * 60f * gameTime.ElapsedGameTime.Milliseconds * MyProfile.MyController.Sensitivity;

            Position.add(new Vector3(
                (float)Math.Cos(Rotation.Y()) * Lstick.Y + (float)Math.Cos(Rotation.Y() + Math.PI / 2) * Lstick.X,
                0,                
                (float)Math.Sin(Rotation.Y() + Math.PI / 2) * Lstick.X + (float)Math.Sin(Rotation.Y()) * Lstick.Y));

            Rotation.add(-new Vector3(Rstick.Y, Rstick.X, 0));
            
            MyCamera.SetTopDownView(Position.get(),
                                    Vector3.Zero,
                                    new Vector2((float)Math.Cos(ZoomAngle), (float)Math.Sin(ZoomAngle)) * ZoomAmount,
                                    Vector2.Zero,
                                    Rotation.Y());
            
            base.Update(gameTime);
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            switch (DrawTag)
            {
                case GameObjectTag._3DDeferredGBuffer:
                    {
                        effect3D.SetWorldViewIT(camera, this);
                        effect3D.SetFromObject(this);
                        effect3D.SetFromCamera(camera);
                        effect3D.SetDeferredTechnique();
                        break;
                    }
                default:
                    {
                        effect3D.SetFromCamera(camera);
                        effect3D.SetUV(camera);
                        effect3D.SetForwardTechnique();
                        break;
                    }
            }
            Render.DrawModel(PlayerModel, effect3D.MyEffect);
            base.Draw3D(camera, DrawTag);
        }
    }
}
