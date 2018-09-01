using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BadRabbit.Carrot
{
    public enum ControlMode
    {
        Move,
        Rotate,
        Scale
    }

    public class ObjectControls3D : Form
    {
#if !EDITOR && WINDOWS
        public void Draw3D(Camera3D camera) { }
        public bool RayCast(GameTime gameTime) { return false; } 
#endif
#if EDITOR && WINDOWS
        private static bool Loaded = false;
        private static bool LoadedIcons = false;
        private static Model ArrowModel;
        private static Model CubeModel;
        private static Model BallModel;
        private static _3DEffect ColorEffectHolder;
        private static _3DEffect ColorSetEffectHolder;
        private static Texture2D GlowCircle;
        private static Color[] PointColors = { new Color(1, 0.2f, 0.2f), new Color(1, 1, 0.2f), new Color(0.5f, 0.5f, 1) ,new Color(0.5f,0.5f,0.5f)};
        private static Matrix[] ModelMatricies = {   Matrix.CreateTranslation(new Vector3(5, 0, 0)),  
                                                    Matrix.CreateFromYawPitchRoll(0,0,MathHelper.ToRadians(90)) * Matrix.CreateTranslation(new Vector3(0, 5, 0)), 
                                                    Matrix.CreateFromYawPitchRoll(-MathHelper.ToRadians(90),0,0) * Matrix.CreateTranslation(new Vector3(0, 0, 5))  };
        public static Texture2D RotateIcon;
        public static Texture2D ScaleIcon;
        public static Texture2D MoveIcon;
        private static int PointCount;
        private static VertexPositionColor[] PointList;
        private static short[] Indicies;


        public Vector2[] ScreenPositions = new Vector2[4];
        public LockMode CurrentMode = LockMode.None;
        public Vector2 MouseLockPosition;
        public ControlMode controlMode = ControlMode.Move;
        Matrix WorldMatrix = Matrix.Identity;
        public List<Form> MyForms = new List<Form>();
#endif
        public Basic3DScene ParentScene;

        public ObjectControls3D(Basic3DScene ParentScene)
        {
            this.ParentScene = ParentScene;
        }
#if EDITOR && WINDOWS
        public override void Create(FormHolder Parent)
        {
            base.Create(Parent);
            Load();
            CreateForms();
        }

        public override void Update(GameTime gameTime, Window Updater)
        {
            if (CurrentMode != LockMode.None)
            {
                if (MouseManager.mouseState.LeftButton == ButtonState.Pressed)
                {
                    bool Success = false;
                    Vector3 Result = Vector3.Zero;
                    Basic3DObject.GetAveragePosition(ParentScene.SelectedGameObjects, ref Result, ref Success);

                    Vector3 Direction = -Vector3.One;
                    if (CurrentMode == LockMode.MoveX)
                        Direction = new Vector3(1, 0, 0);
                    if (CurrentMode == LockMode.MoveY)
                        Direction = new Vector3(0, 1, 0);
                    if (CurrentMode == LockMode.MoveZ)
                        Direction = new Vector3(0, 0, 1);

                    Vector2 MouseMult = Vector2.One;

                    if (CurrentMode != LockMode.MoveAll)
                    {
                        Vector2 CenterScreenPos = ScreenPositions[3];
                        Vector2 ScreenPos2D = ScreenPositions[CurrentMode == LockMode.MoveX ? 0 : CurrentMode == LockMode.MoveY ? 1 : 2];
                        MouseMult = Vector2.Normalize(ScreenPos2D - CenterScreenPos);
                    }

                    Vector2 MouseForce = new Vector2(MouseManager.MousePosition.X - MouseLockPosition.X, MouseManager.MousePosition.Y - MouseLockPosition.Y) * MouseMult / 1000;
                    Camera3DObject c = (Camera3DObject)ParentScene.MyCamera.get();
                    float Value = (MouseForce.X + MouseForce.Y) * c.ZoomDistance.get();

                    switch (controlMode)
                    {
                        case ControlMode.Move:
                            ParentScene.MoveSelected(Value * Direction);
                            break;
                        case ControlMode.Rotate:
                            ParentScene.RotateSelected(Value * Direction, Result);
                            break;
                        case ControlMode.Scale:
                            ParentScene.ScaleSelected(Vector3.One + Value / 300 * Direction, Result);
                            break;
                    }
                }
                else
                {
                    Game1.self.IsMouseVisible = true;
                    CurrentMode = LockMode.None;
                }
                Mouse.SetPosition((int)MouseLockPosition.X, (int)MouseLockPosition.Y);
            }

            base.Update(gameTime, Updater);
        }

        public bool RayCast(GameTime gameTime)
        {
            if (MouseManager.MouseClicked && ParentScene.SelectedGameObjects.Count > 0 && ParentScene.MyCamera.get() != null)
            {
                float BestDist = 64;
                float d;
                for (int i = 0; i < 4; i++)
                {
                    d = Vector2.Distance(WorldViewer.self.RelativeMousePosition, ScreenPositions[i]);
                    if (d < BestDist)
                    {
                        BestDist = d;
                        CurrentMode = i < 2 ? i == 0 ? LockMode.MoveX : LockMode.MoveY :
                            i == 2 ? LockMode.MoveZ : LockMode.MoveAll;

                        MouseLockPosition = MouseManager.MousePosition;
                        Game1.self.IsMouseVisible = false;

                    }
                }
            }
            return CurrentMode != LockMode.None ? true : false;
        }

        public void CreateForms()
        {
            foreach (Form form in MyForms)
                Parent.RemoveForm(form);
            MyForms.Clear();

            int ButtonX = 0;
            int ButtonY = 0;
            int Margin = -23;
            int ButtonSize = 16;

            AddForm(new Button(SetRotate, RotateIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetScale, ScaleIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));

            ButtonX -= Margin;
            AddForm(new Button(SetMove, MoveIcon, new Vector2(ButtonSize), new Vector2(ButtonX, ButtonY)));
            SetMove((Button)MyForms[2]);
        }

        public void SetRotate(Button button)
        {
            controlMode = ControlMode.Rotate;
            Switchselect(button);
        }

        public void SetScale(Button button)
        {
            controlMode = ControlMode.Scale;
            Switchselect(button);
        }

        public void SetMove(Button button)
        {
            controlMode = ControlMode.Move;
            Switchselect(button);
        }

        private void Switchselect(Button b)
        {
            foreach (Button but in MyForms)
                but.Selected = false;
            b.Selected = true;
        }

        new public void AddForm(Form NewForm)
        {
            MyForms.Add(NewForm);
            Parent.AddForm(NewForm);
        }

        public static void Load()
        {
            if (!Loaded)
            {
                Loaded = true; 
                LoadIcons();

                CubeModel = AssetManager.Load<Model>("Editor/CubeModel");
                ArrowModel = AssetManager.Load<Model>("Editor/ArrowModel");
                BallModel = AssetManager.Load<Model>("Editor/BallModel");

                ColorEffectHolder = (_3DEffect)new _3DEffect().Create("Editor/ColorEffect");
                ColorSetEffectHolder = (_3DEffect)new _3DEffect().Create("Editor/ColorSetEffect");

                GlowCircle = AssetManager.Load<Texture2D>("Editor/GlowCircle");

                PointCount = 6;
                PointList = new VertexPositionColor[PointCount];
                Indicies = new short[PointCount];
                int i = 0;

                PointList[i++] = new VertexPositionColor(
                   new Vector3(5, 0, 0), PointColors[0]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, 0, 0), PointColors[0]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, 5, 0), PointColors[1]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, 0, 0), PointColors[1]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, 0, 5), PointColors[2]);

                PointList[i++] = new VertexPositionColor(
                   new Vector3(0, 0, 0), PointColors[2]);

                for (i = 0; i < PointCount; i += 1)
                    Indicies[i] = (short)(i);
            }
        }

        public static void LoadIcons()
        {
            if (!LoadedIcons)
            {
                LoadedIcons = true;
                ScaleIcon = Game1.content.Load<Texture2D>("Editor/ScaleIcon");
                RotateIcon = Game1.content.Load<Texture2D>("Editor/RotateIcon");
                MoveIcon = Game1.content.Load<Texture2D>("Editor/MoveIcon");
                GridIcon = Game1.content.Load<Texture2D>("Editor/GridIcon");
            }
        }


        public override void DrawAdditive()
        {
            if (CurrentMode != LockMode.None)
            {
                int i = CurrentMode == LockMode.MoveX ? 0 :
                    CurrentMode == LockMode.MoveY ? 1 : CurrentMode == LockMode.MoveZ ? 2 : 3;
                Rectangle rect = new Rectangle((int)ScreenPositions[i].X-16,(int)ScreenPositions[i].Y-16,32,32);
                Game1.spriteBatch.Draw(GlowCircle,rect , PointColors[i]); ;
            }

            base.DrawAdditive();
        }

        public void Draw3D(Camera3D camera)
        {
            Camera3DObject c = (Camera3DObject)ParentScene.MyCamera.get();
            if (c != null)
            {
                Vector3 Result = Vector3.Zero;
                bool Success = false;
                Basic3DObject.GetAveragePosition(ParentScene.SelectedGameObjects, ref Result, ref Success);

                WorldMatrix = c.ScaleMatrix * Matrix.CreateTranslation(Result);
                ColorEffectHolder.SetWorld(WorldMatrix);
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


                ColorSetEffectHolder.Collection["Color"].SetValue(Color.LightGray.ToVector4());
                ColorSetEffectHolder.SetFromCamera(camera);
                ColorSetEffectHolder.SetWorld(WorldMatrix);
                Render.DrawModel(CubeModel, ColorSetEffectHolder.MyEffect);
                Vector3 ScreenPos = WorldViewer.self.MyViewport.Project(Vector3.Zero, camera.ProjectionMatrix, camera.ViewMatrix, WorldMatrix);
                ScreenPositions[3] = new Vector2(ScreenPos.X, ScreenPos.Y);


                Model EndModel = controlMode == ControlMode.Move ? ArrowModel :
                    controlMode == ControlMode.Rotate ? BallModel : CubeModel;
                for (int i = 0; i < 3; i++)
                {
                    ColorSetEffectHolder.Collection["Color"].SetValue(PointColors[i].ToVector4());
                    ColorSetEffectHolder.SetWorld(ModelMatricies[i] * WorldMatrix);
                    Render.DrawModel(EndModel, ColorSetEffectHolder.MyEffect);
                    ScreenPos = WorldViewer.self.MyViewport.Project(Vector3.Zero, camera.ProjectionMatrix, camera.ViewMatrix, ModelMatricies[i] * WorldMatrix);
                    ScreenPositions[i] = new Vector2(ScreenPos.X, ScreenPos.Y);
                }
            }
        }
#endif

        public static Texture2D GridIcon;
    }
}

