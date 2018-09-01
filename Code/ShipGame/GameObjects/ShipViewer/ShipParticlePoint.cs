using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class ShipParticlePoint : Basic3DObject
    {
#if EDITOR && WINDOWS
        private static Model PointModel;
        private static _3DEffect PointEffect;
        private static bool Loaded = false;
#endif

        int Timer = 0;
        bool CanProduceParticles = false;

        public IntValue Layer;
        public IntValue CinematicDelay;
        public IntValue GameDelay;
        public IntValue ParticleType;
        public Vector3Value MaxVelocity;
        public Vector3Value MinVelocity;
        public ColorValue MinColor;
        public ColorValue MaxColor;
        public FloatValue MinSize;
        public FloatValue MaxSize;
        public BoolValue CinematicOnly;
        public BoolValue NoInterpolate;

        public override void Create()
        {
            Layer = new IntValue("Layer");
            CinematicDelay = new IntValue("Cinematic Delay");
            GameDelay = new IntValue("Game Delay");
            ParticleType = new IntValue("Particle Type");
            MinVelocity = new Vector3Value("Min Velocity");
            MaxVelocity = new Vector3Value("Max Velocity");
            MinColor = new ColorValue("Min Color");
            MaxColor = new ColorValue("Max Color");
            MinSize = new FloatValue("Min Size", 80);
            MaxSize = new FloatValue("Max Size", 120);
            CinematicOnly = new BoolValue("Cinematic Only");
            NoInterpolate = new BoolValue("No Interpolate", false);

            base.Create();
            Load();
            RemoveValue(Scale);
            RemoveValue(Rotation);
            AddTag(GameObjectTag._3DForward);
            AddTag(GameObjectTag.Update);
        }

        public override void UpdateEditor(GameTime gameTime)
        {
            Update(gameTime);
            base.UpdateEditor(gameTime);
        }

        public void AddTime(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;

            if (Timer > GameDelay.get())
            {
                Timer = 0;
                CanProduceParticles = true;
            }
            else
                CanProduceParticles = false;
        }

        public void ProduceParticle(Vector3 Position)
        {
            if (CanProduceParticles)
                ParticleManager.CreateParticle(Position, Logic.RLerp(MinVelocity.get(), MaxVelocity.get()), new Color(Logic.RLerp(MinColor.get(), MaxColor.get())), MathHelper.Lerp(MinSize.get(), MaxSize.get(), Rand.F()), ParticleType.get());
        }

        public override void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            if (Timer > CinematicDelay.get())
            {
                Timer -= CinematicDelay.get();
                if (!NoInterpolate.get())
                    ParticleManager.CreateParticle(Position.get(), Logic.RLerp(MinVelocity.get(), MaxVelocity.get()), new Color(Logic.RLerp(MinColor.get(), MaxColor.get()) / 3), MathHelper.Lerp(MinSize.get(), MaxSize.get(), Rand.F()), ParticleType.get());
                else
                    ParticleManager.CreateParticle(Position.get(), Logic.RLerp(MinVelocity.get(), MaxVelocity.get()), new Color(MinColor.get() / 3), MathHelper.Lerp(MinSize.get(), MaxSize.get(), Rand.F()), ParticleType.get());
            }
            base.Update(gameTime);
        }

#if EDITOR && WINDOWS
        new private static void Load()
        {
            if (!Loaded)
            {
                PointModel = AssetManager.Load<Model>("Models/ShipGame/Point");
                PointEffect = (_3DEffect)new _3DEffect().Create("Effects/WhiteEffect");
                Loaded = true;
            }
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (ParentLevel.LevelForEditing)
            {
                PointEffect.SetFromCamera(camera);
                PointEffect.SetFromObject(this);
                Render.DrawModel(PointModel, PointEffect.MyEffect);
                base.Draw3D(camera, DrawTag);
            }
        }
#endif
    }
}
