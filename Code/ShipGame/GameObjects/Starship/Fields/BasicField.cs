using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace BadRabbit.Carrot
{
    public class BasicField : BasicShipGameObject
    {
        private static LinkedList<BasicField> AllFields = new LinkedList<BasicField>();

        public static bool TestFieldClear(Vector2 Position)
        {
            foreach (BasicField f in AllFields)
                if (Vector2.Distance(f.Position.get(), Position) < f.Size.X() + 300)
                    return false;

            return true;
        }

        protected int MaxSearchTime = 200;
        int SearchTime = 0;

        protected int ShipEffectTime = 1000;
        new protected FieldState fieldState = FieldState.None;

        protected int LifeTime = 10000;
        int TimeAlive = 0;

        float Alpha = 0;
        float AlphaChangeRate = 0.1f;

        protected int CircleGlows = 16;
        protected Color CircleColor = Color.White;

        public Vector2 TargetSize = Vector2.Zero;

        public SoundEffectInstance SoundInstance;

        public BasicField(int FactionNumber)
            : base(-1)
        {
            AllFields.AddLast(this);
        }

        public override void Create()
        {
            AddTag(GameObjectTag.Update);
            base.Create();
            Size.ChangeEvent = SizeChange;
        }

        private void SizeChange()
        {
            TargetSize = Size.get() * 1.4f;
            Size.setNoPerform(Vector2.Zero);
        }

        public override void Update(GameTime gameTime)
        {
            SoundInstance = SoundManager.PlayLoopingSound(SoundInstance, "FieldHum",
                new Vector3(Position.X(), Y, Position.Y()), 0.05f, 800, 2);
            /*
            if (Parent2DScene.WorldViewerChildren.Count != ActiveSounds.Count)
            {
                SoundManager.Play3DSound("FieldCast", new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, 1, false);

                foreach (SoundManager.ActiveSound a in ActiveSounds)
                    a.Instance.Stop();

                SoundManager.Play3DSound("MiningRingHum", new Vector3(Position.X(), Y, Position.Y()), Vector3.Zero, 0.25f, true);
                ActiveSounds.Clear();

                for (int i = 0; i < Parent2DScene.WorldViewerChildren.Count; i++)
                    ActiveSounds.AddLast(SoundManager.ActiveSounds[SoundManager.ActiveSoundCount - (i + 1)]);
            }
            */

            for (int i = 0; i < CircleGlows; i++)
            {
                float R = (float)(((float)i / CircleGlows * 2 * Math.PI) + (Level.Time % 2 / 2f * Math.PI) / 2);
                Vector3 P = new Vector3((float)Math.Cos(R) * Size.X() + Position.X(), 0, (float)Math.Sin(R) * Size.X() + Position.Y());
                ParticleManager.CreateParticle(P, Vector3.Zero, CircleColor * Alpha, 64, 0);
            }

            TimeAlive += gameTime.ElapsedGameTime.Milliseconds;
            if (TimeAlive > LifeTime)
            {
                Alpha -= gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChangeRate;
                if (Alpha < 0)
                    Destroy();
            }
            else
            {
                TargetSize -= Vector2.One * 0.25f * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f;
                Size.setNoPerform(Size.get() + Vector2.One * 4 * gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f);
                if (Size.X() > TargetSize.X)
                    Size.setNoPerform(TargetSize);

                if (Alpha < 1)
                {
                    Alpha += gameTime.ElapsedGameTime.Milliseconds * 60 / 1000f * AlphaChangeRate;
                    if (Alpha > 1)
                        Alpha = 1;
                }

                SearchTime += gameTime.ElapsedGameTime.Milliseconds;
                if (SearchTime > MaxSearchTime)
                {
                    SearchTime -= MaxSearchTime;
                    QuadGrid quad = Parent2DScene.quadGrids.First.Value;
                    foreach (Basic2DObject o in quad.Enumerate(Position.get(), Size.get()))
                        if ((o.GetType().IsSubclassOf(typeof(UnitShip)) || o.GetType().IsSubclassOf(typeof(PlayerShip))) && Vector2.Distance(o.Position.get(), Position.get()) < Size.X())
                        {
                            UnitBasic u = (UnitBasic)o;
                            if (u.GetTeam() != WaveManager.ActiveTeam)
                                EffectUnit((UnitBasic)o, gameTime);
                        }
                }
            }

 	        base.Update(gameTime);
        }

        public override void Destroy()
        {
            SoundManager.Play3DSound("FieldCollapse", new Vector3(Position.X(), Y, Position.Y()), 1, 800, 2);

            if (SoundInstance != null && !SoundInstance.IsDisposed)
            {
                SoundInstance.Dispose();
                SoundInstance = null;
            }

            AllFields.Remove(this);
            base.Destroy();
        }

        public virtual void EffectUnit(UnitBasic s, GameTime gameTime)
        {
            s.SetFieldState(fieldState, ShipEffectTime);
        }
    }
}
