using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class DeferredLightEffect : Deferred3DEffect
    {
        private EffectParameter InverseView;
        private EffectParameter InverseViewProjection;


        public override void SetUp()
        {
            InverseView = Collection["InverseView"];
            InverseViewProjection = Collection["InverseViewProjection"];

            base.SetUp();
        }

        public void SetInverseCamera(Camera3D camera)
        {
            if (InverseView != null)
                InverseView.SetValue(camera.InverseView);
            if (InverseViewProjection != null)
                InverseViewProjection.SetValue(camera.InverseViewProjection);
            if (CameraPosition != null)
                CameraPosition.SetValue(camera.Position);
            if (ViewProjection != null)
                ViewProjection.SetValue(camera.ViewMatrix * camera.ProjectionMatrix);
        }
    }
}
