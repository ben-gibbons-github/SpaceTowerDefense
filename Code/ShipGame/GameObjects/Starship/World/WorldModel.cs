using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class WorldModel : BasicModel
    {
        bool Applied = false;
        BoolValue Additive;
        BoolValue DepthRead;

        Vector3Value RotationAmount;

        public override void Create()
        {
            AddTag(GameObjectTag.Update);

            Additive = new BoolValue("Additive", false);
            DepthRead = new BoolValue("DepthRead", false);
            RotationAmount = new Vector3Value("Rotation Amount");

            AddTag(GameObjectTag._2DForward);
            AddTag(GameObjectTag._3DForward);
            base.Create();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Rotation.add(RotationAmount.get());
            base.Update(gameTime);
        }

        private void ApplyEffects()
        {
            EffectParameterCollection Parameters = MyEffect.get().Parameters;

            if (Parameters["AmbientLightColor"] != null)
                Parameters["AmbientLightColor"].SetValue(ShipLighting.WorldAmbientLightColor);
            if (Parameters["LightOneDirection"] != null)
                Parameters["LightOneDirection"].SetValue(ShipLighting.WorldLightOneDirection);
            if (Parameters["LightOneColor"] != null)
                Parameters["LightOneColor"].SetValue(ShipLighting.WorldLightOneColor);
            if (Parameters["LightTwoDirection"] != null)
                Parameters["LightTwoDirection"].SetValue(ShipLighting.WorldLightTwoDirection);
            if (Parameters["LightTwoColor"] != null)
                Parameters["LightTwoColor"].SetValue(ShipLighting.WorldLightTwoColor);

            base.CreateInGame();
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            Game1.graphicsDevice.BlendState = Additive.get() ? BlendState.Additive : BlendState.Opaque;
            Game1.graphicsDevice.DepthStencilState = DepthRead.get() ? DepthStencilState.DepthRead : DepthStencilState.Default;
            if (!Applied)
            {
                Applied = true;
                ApplyEffects();
            }
            base.Draw3D(camera, DrawTag);
        }
    }
}
