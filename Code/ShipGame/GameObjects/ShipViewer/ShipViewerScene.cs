using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BadRabbit.Carrot.EffectParameters;
using Microsoft.Xna.Framework;

#if WINDOWS && EDITOR
using System.Windows.Forms;
using System.IO;
#endif

namespace BadRabbit.Carrot
{
    public class ShipViewerScene : Forward3DScene
    {
        public override void Create()
        {
            ParticleManager.Load();
#if WINDOWS && EDITOR
            if (ParentLevel.LevelForEditing)
                AddRightClickEvent("Save Ship", SaveShip);
#endif
            base.Create();
        }

#if WINDOWS && EDITOR
        private void SaveShip(Button b)
        {
            SaveFileDialog openFileDialog1;

            openFileDialog1 = new SaveFileDialog();

            openFileDialog1.InitialDirectory = DialogManager.LastFileLocation;
            openFileDialog1.Filter = "Level files (*.shp)|*.shp";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.OverwritePrompt = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DialogManager.LastFileLocation = openFileDialog1.FileName;

                Save(openFileDialog1.FileName);
            }
        }

        private void Save(string Location)
        {
            Stream MyStream;

            try
            {
                if (File.Exists(Location))
                    File.Delete(Location);
                if ((MyStream = File.Create(Location)) != null)
                {
                    using (MyStream)
                    {
                        WriteShipFile(new BinaryWriter(MyStream));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not write file to disk. \nOriginal error: " + ex.Message);
            }
        }

        private void WriteShipFile(BinaryWriter Writer)
        {
            SaveHelper.MyWriter = Writer;

            Writer.Write((byte)1);

            Writer.Write("Ship");

            WriteScale(Writer);

            WriteShipViewer(Writer);
            WriteParticlePoints(Writer);
            WriteWeaponPoints(Writer);
        }

        private void WriteShipViewer(BinaryWriter Writer)
        {
            foreach (GameObject g in Children)
                if (g.GetType().Equals(typeof(ShipViewer)))
                {
                    ShipViewer s = (ShipViewer)g;

                    Writer.Write(s.model.getFullPath());
                    Writer.Write(s.effect.getFullPath());

                    ColorParameter DrawColor = (ColorParameter)s.effect.findValueParameter("DrawColor");
                    ColorParameter SpecularColor = (ColorParameter)s.effect.findValueParameter("SpecularColor");
                    FloatParameter SpecularExponent = (FloatParameter)s.effect.findValueParameter("SpecularExponent");
                    Texture2DParameter Texture = (Texture2DParameter)s.effect.findValueParameter("Texture");

                    Writer.Write(Texture.getFullPath());

                    SaveHelper.Write(DrawColor.get());
                    SaveHelper.Write(SpecularColor.get());
                    Writer.Write((Single)SpecularExponent.get());

                    return;
                }
            throw new Exception("No ShipViewer to save");
        }

        private void WriteWeaponPoints(BinaryWriter Writer)
        {
            int WeaponCount = 0;

            foreach (GameObject g in Children)
                if (g.GetType().Equals(typeof(ShipWeaponPoint)))
                    WeaponCount++;

            Writer.Write((Int32)WeaponCount);

            foreach (GameObject g in Children)
                if (g.GetType().Equals(typeof(ShipWeaponPoint)))
                {
                    ShipWeaponPoint s = (ShipWeaponPoint)g;
                    SaveHelper.Write(s.Position.get());
                    Writer.Write((Int32)s.Layer.get());
                }
        }

        private void WriteParticlePoints(BinaryWriter Writer)
        {
            int ParticleCount = 0;

            foreach (GameObject g in Children)
                if (g.GetType().Equals(typeof(ShipParticlePoint)))
                    ParticleCount++;

            Writer.Write((Int32)ParticleCount);

            foreach (GameObject g in Children)
                if (g.GetType().Equals(typeof(ShipParticlePoint)))
                {
                    ShipParticlePoint s = (ShipParticlePoint)g;
                    SaveHelper.Write(s.Position.get());
                    Writer.Write((Int32)s.Layer.get());
                    Writer.Write((Int32)s.CinematicDelay.get());
                    Writer.Write((Int32)s.GameDelay.get());
                    Writer.Write((Int32)s.ParticleType.get());
                    SaveHelper.Write(s.MaxVelocity.get());
                    SaveHelper.Write(s.MinVelocity.get());
                    SaveHelper.Write(s.MinColor.get());
                    SaveHelper.Write(s.MaxColor.get());
                    Writer.Write((Single)s.MinSize.get());
                    Writer.Write((Single)s.MaxSize.get());
                    Writer.Write(s.CinematicOnly.get());
                }
        }

        private void WriteScale(BinaryWriter Writer)
        {
            foreach(GameObject o in Children)
                if (o.GetType().Equals(typeof(ShipScaleRing)))
                {
                    ShipScaleRing s = (ShipScaleRing)o;
                    Writer.Write((Single)s.Scale.X());
                    Writer.Write((Single)s.Rotation.Y());
                    return;
                }
            Writer.Write((Single)0);
            Writer.Write((Single)0);
            throw new Exception("You must place a Scale Ring");
        }
#endif

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ParticleManager.Update(gameTime);
        }

        public override void UpdateEditor(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ParticleManager.Update(gameTime);
            base.UpdateEditor(gameTime);
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            base.Draw3D(camera, DrawTag);
            ParticleManager.ParticleDepthStencil = Microsoft.Xna.Framework.Graphics.DepthStencilState.DepthRead;
            ParticleManager.PreDraw(camera);
            ParticleManager.Draw(camera);
        }
    }
}
