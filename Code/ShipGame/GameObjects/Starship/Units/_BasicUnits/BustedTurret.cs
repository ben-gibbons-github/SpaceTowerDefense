using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class BustedTurret : BasicShipGameObject
    {
        static float AlphaChange = 0.0f;

        UnitTurret ParentTurret;
        private float particleAlpha = 1;


        public BustedTurret(UnitTurret ParentTurret) :
            base (-1)
        {
            this.ParentTurret = ParentTurret;
        }

        public override void Update(GameTime gameTime)
        {
            if (particleAlpha > 0)
            {
                particleAlpha -= gameTime.ElapsedGameTime.Milliseconds * 60f / 1000f * AlphaChange / 20;
                InstanceManager.EmitParticle(GetIntType(), new Vector3(Position.X(), Y, Position.Y()), ref ParentTurret.RotationMatrix, 0, ParentTurret.Size.X(), particleAlpha);
            }
            base.Update(gameTime);
        }

        public void Activate()
        {
            particleAlpha = 1;
            Size.set(ParentTurret.Size.get());
            Position.set(ParentTurret.Position.get());
            Rotation.set(ParentTurret.Rotation.get());

            SetQuadGridPosition();

            InstanceManager.AddChild(this);
            AddTag(GameObjectTag.Update);
            //AddTag(GameObjectTag._2DSolid);
        }

        public void Deactivate()
        {
            particleAlpha = 0;
            InstanceManager.RemoveChild(this);
            RemoveTag(GameObjectTag.Update);
        }

        public override void Destroy()
        {
            InstanceManager.RemoveChild(this);
            base.Destroy();
        }

        public override int GetIntType()
        {
            return 1;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Extra/BustedTurret");
        }
    }
}
