using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BadRabbit.Carrot
{
    public class FormFrame
    {
        public static SoundEffect OpenSound;
        public static float OpenVolume = 1;

        public static SoundEffect CloseSound;
        public static float CloseVolume = 1;

        public Dictionary<string, LinkedList<GameObject>> SortedChildren;
        public LinkedList<GameObject> FormChildren = new LinkedList<GameObject>();
        public LinkedList<BasicMarker> TargetMarkers = new LinkedList<BasicMarker>();
        public GameObject Parent;

        public Vector2 CameraPosition;
        public Vector2 ScreenOffset;
        public float CameraZoom = 1;

        public int RestrictedView = -1;

        public Vector2 FrameSize = Vector2.One;


        public bool Active = true;

        public FormFrame(GameObject Parent)
        {
            this.Parent = Parent;
        }

        public void Add(BasicGameForm Form)
        {
            FormChildren.AddLast(Form);
            Form.AddToFrame(this);
        }

        public void Add(BasicMarker M)
        {
            M.AddToFrame(this);
        }

        public void SetRestrictedView(int RestrictedView)
        {
            this.RestrictedView = RestrictedView;
            foreach (BasicGameForm g in FormChildren)
                g.RestrictedView = RestrictedView;
        }

        public void Update(GameTime gameTime)
        {
            if (TargetMarkers.Count > 0)
            {
                Vector2 PositionSum = Vector2.Zero;
                foreach (BasicMarker m in TargetMarkers)
                    PositionSum += m.Position.get();

                PositionSum /= TargetMarkers.Count;
                CameraPosition = PositionSum;
            }
        }

        public void Cycle(string s)
        {
            if (OpenSound != null)
                OpenSound.Play(OpenVolume, 0, 0);

            if (SortedChildren == null || !SortedChildren.ContainsKey(s))
                return;

            foreach (BasicGameForm f in FormChildren)
                f.RemoveFromFrame(this);

            FormChildren = SortedChildren[s];

            foreach (BasicGameForm f in FormChildren)
                f.AddToFrame(this);

            foreach (BasicMarker m in TargetMarkers)
                m.AddToFrame(this);

            Active = true;
        }

        public void Commit(string s, bool BeginNew)
        {
            if (SortedChildren == null)
                SortedChildren = new Dictionary<string, LinkedList<GameObject>>();

            if (!SortedChildren.ContainsKey(s))
                SortedChildren.Add(s, FormChildren);
            else
                SortedChildren[s] = FormChildren;

            if (BeginNew)
            {
                foreach (BasicGameForm f in FormChildren)
                    f.RemoveFromFrame(this);

                FormChildren = new LinkedList<GameObject>();

                foreach (BasicMarker m in TargetMarkers)
                    m.AddToFrame(this);
            }
        }

        public void Activate()
        {
            if (OpenSound != null)
                OpenSound.Play(OpenVolume, 0, 0);

            Active = true;
            foreach (BasicGameForm f in FormChildren)
                f.AddToFrame(this);
            foreach (BasicMarker m in TargetMarkers)
                m.AddToFrame(this);
        }

        public void DeActivate()
        {
            if (CloseSound != null)
                CloseSound.Play(CloseVolume, 0, 0);

            Active = false;
            foreach (BasicGameForm f in FormChildren)
                f.RemoveFromFrame(this);
            foreach (BasicMarker m in TargetMarkers)
                m.RemoveFromFrame(this);
        }

        public void ClearForms()
        {
            foreach (BasicMarker m in TargetMarkers)
                m.RemoveFromFrame(this);

            foreach (GameObject g in FormChildren)
                g.Destroy();
            FormChildren.Clear();
        }

        public void AddTarget(BasicMarker M)
        {
            M.AddToFrame(this);
            TargetMarkers.AddLast(M);
        }
    }
}
