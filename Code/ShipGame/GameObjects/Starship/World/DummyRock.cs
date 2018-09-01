using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BadRabbit.Carrot.WaveFSM;

namespace BadRabbit.Carrot
{
    public class DummyRock : SolidStaticWorldObject
    {
#if EDITOR && WINDOWS
        private static Texture2D PlaceholderRockTexture;
#endif

        float ShipMatrixScale = 0;

        public DummyRock() : base(-1) { }

        public override void Create()
        {
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                PlaceholderRockTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/PlaceholderRock");
                AddTag(GameObjectTag._2DForward);
            }
#endif
            AddTag(GameObjectTag._2DSolid);
            AddTag(GameObjectTag.Update);

            base.Create();

            Position.ChangeEvent = ChangePosition;
            Size.ChangeEvent = ChangePosition;
            Size.set(new Vector2(90));
        }

        public override void Update(GameTime gameTime)
        {
            SetQuadGridPosition();
            base.Update(gameTime);
        }

        private void ChangePosition()
        {
            SetQuadGridPosition();
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateRotationY(Rotation.getAsRadians()) * Matrix.CreateTranslation(new Vector3(Position.X(), Y, Position.Y()));
        }

        public override int GetIntType()
        {
            return 0;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("World/Asteroid1");
        }

        public override void CreateInGame()
        {
            base.CreateInGame();

            ShipMatrixScale = InstanceManager.AddChild(this);
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateRotationY(Rotation.getAsRadians()) * Matrix.CreateTranslation(new Vector3(Position.X(), Y, Position.Y()));

            SetQuadGridPosition();
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                if (Render.AdditiveBlending)
                    return;

                Render.DrawSprite(PlaceholderRockTexture, Position.get(), Size.get() * 1.25f, Rotation.get());
            }
#endif
            base.Draw2D(DrawTag);
        }

        public override void Destroy()
        {
#if EDITOR && WINDOWS
            if (!ParentLevel.LevelForEditing)
#endif
                InstanceManager.RemoveChild(this);
            base.Destroy();
        }
    }
}
