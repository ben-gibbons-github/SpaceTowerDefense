using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class PlayerUIManager
    {
        private LinkedList<UIParticleBasic> Particles = new LinkedList<UIParticleBasic>();
        private UIParticleBasic ParticleToRemove;

        private PlayerShip ParentShip;

        //public PlayerMenuManager MenuManager;

        public LinkedList<HudBox> HudBoxes = new LinkedList<HudBox>();

        public FormFrame TurretPickerFrame;
        public FormFrame MenuFrame;

        public bool Active()
        {
            return TurretPickerFrame.Active || MenuFrame.Active;
        }

        public PlayerUIManager(PlayerShip ParentShip)
        {
            this.ParentShip = ParentShip;

            //MenuManager = new PlayerMenuManager(ParentShip);

            TurretPickerFrame = new FormFrame(ParentShip);
            TurretPickerFrame.ScreenOffset = new Vector2(0, 200);
            TurretForm.BuildAllTurrets(TurretPickerFrame);

            MenuFrame = new FormFrame(ParentShip);
            PlayerMenuBuilder.BuildPauseMenu(MenuFrame);
            PlayerMenuBuilder.BuildControllerOptions(MenuFrame);
            PlayerMenuBuilder.BuildScreenOptions(MenuFrame);

            PlayerMarker m = new PlayerMarker(ParentShip.MyProfile);
            ParentShip.ParentLevel.AddObject(m);
            m.Visible = false;
            m.MoveSpeed.set(10);
            m.ResizeSpeed.set(10); 
            TurretPickerFrame.AddTarget(m);

            m = new PlayerMarker(ParentShip.MyProfile);
            ParentShip.ParentLevel.AddObject(m);
            m.MoveSpeed.set(5);
            m.ResizeSpeed.set(10);
            MenuFrame.AddTarget(m);

            TurretPickerFrame.DeActivate();
            MenuFrame.DeActivate();

            TurretPickerFrame.SetRestrictedView(ParentShip.MyProfile.PlayerNumber);
            MenuFrame.SetRestrictedView(ParentShip.MyProfile.PlayerNumber);

            AddHudBox(new HudHealthBox());
            AddHudBox(new HudScoreBox());
            AddHudBox(new HudWeaponsBox());
            AddHudBox(new HudOutline());
            AddHudBox(new HudProgressCircle());
            AddHudBox(new HudFactionFeed());
            AddHudBox(new HudTimerBox());
        }

        void AddHudBox(HudBox b)
        {
            HudBoxes.AddLast(b);
            b.Create(ParentShip);
        }

        public void Update(GameTime gameTime)
        {
            TurretPickerFrame.FrameSize = new Vector2(1280, 720) / ParentShip.sceneView.Size;
            TurretPickerFrame.Update(gameTime);
            TurretPickerFrame.SetRestrictedView(ParentShip.sceneView.Index);

            MenuFrame.FrameSize = new Vector2(1280, 720) / ParentShip.sceneView.Size;
            MenuFrame.Update(gameTime);
            MenuFrame.SetRestrictedView(ParentShip.sceneView.Index);

            foreach (HudBox b in HudBoxes)
                b.Update(gameTime);

            foreach (UIParticleBasic part in Particles)
                part.Update(gameTime);

            if (ParticleToRemove != null)
            {
                Particles.Remove(ParticleToRemove);
                ParticleToRemove = null;
            }
        }

        public void AddParticle(UIParticleBasic part)
        {
            Particles.AddLast(part);
            part.Create(this, ParentShip);
        }

        public void RemoveParticle(UIParticleBasic part)
        {
            ParticleToRemove = part;
        }

        public void Draw(Camera3D DrawCamera)
        {
            foreach (UIParticleBasic part in Particles)
                part.Draw();

            WaveManager.Draw(ParentShip.getSceneView());

            foreach (HudBox b in HudBoxes)
                b.PreDraw();
        }
    }
}
