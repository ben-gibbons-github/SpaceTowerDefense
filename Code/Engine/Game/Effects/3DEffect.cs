using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class _3DEffect : EffectHolder
    {
        public EffectParameter View;
        public EffectParameter Projection;
        public EffectParameter ViewProjection;
        public EffectParameter World;
        public EffectParameter Rotation;
        public EffectParameter CameraPosition;
        public EffectParameter Time;

        private EffectTechnique ShadowTechnique;
        private EffectTechnique ForwardTechnique;

        public override void SetUp()
        {
            View = Collection["View"];
            Projection = Collection["Projection"];
            ViewProjection = Collection["ViewProjection"];
            World = Collection["World"];
            Rotation = Collection["Rotation"];
            CameraPosition = Collection["CameraPosition"];
            Time = Collection["Time"];

            ShadowTechnique = FindTechnique("Shadow");
            ForwardTechnique = FindTechnique("Forward");

            base.SetUp();
        }

        public void SetTime(float Time)
        {
            if (this.Time != null)
                this.Time.SetValue(Time);
        }

        public void SetFromObject(Basic3DObject g)
        {
            if (World != null)
                World.SetValue(g.WorldMatrix);
            if (Rotation != null)
                Rotation.SetValue(g.RotationMatrix);
            if (Time != null)
                Time.SetValue(Level.Time);
        }

        public void SetWorld(Matrix world)
        {
            this.World.SetValue(world);
        }

        public void SetRotation(Matrix rotation)
        {
            if (this.Rotation != null)
                this.Rotation.SetValue(rotation);
        }

        public void SetFromCamera(Camera3D camera)
        {
            if (View != null)
                View.SetValue(camera.ViewMatrix);
            if (Projection != null)
                Projection.SetValue(camera.ProjectionMatrix);
            if (CameraPosition != null)
                CameraPosition.SetValue(camera.Position);
        }

        public void SetForwardTechnique()
        {
            if (ForwardTechnique != null)
                MyEffect.CurrentTechnique = ForwardTechnique;
        }

        public void SetShadowTechnique()
        {
            if (ShadowTechnique != null)
                MyEffect.CurrentTechnique = ShadowTechnique;
        }
    }
}
