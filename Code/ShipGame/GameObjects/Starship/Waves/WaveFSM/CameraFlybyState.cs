using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.WaveFSM
{
    public class CameraFlybyState : WaveState
    {
        public static CameraFlybyState self;
        public static bool BlockPlayerWorldViewer = true;

        static CameraFlybyState()
        {
            self = new CameraFlybyState();
        }

        float ProgressSpeed = 0.005f;

        Dictionary<int, CameraFlybyCenter> SortedCenters = new Dictionary<int,CameraFlybyCenter>();

        Dictionary<int, List<CameraFlybyNode>> SortedNodes = 
            new Dictionary<int,List<CameraFlybyNode>>();
        Camera3DObject WorldCamera;

        CameraFlybyNode NextNode;
        CameraFlybyNode LastNode;

        float NodesProgress;
        int NodeCount;
        int CurrentCenter;
        int CenterCount = -1;

        public override void Enter()
        {
            OverCardPicker.SizeBonus = 0;
            OverMap.SizeMult = 0;
            CenterCount = -1;
            CurrentCenter = 0;
            BlockPlayerWorldViewer = true;
            ProgressSpeed = 0.005f;
            LastNode = null;
            NextNode = null;

            NodeCount = 0;
            NodesProgress = 1;
            SortedNodes.Clear();

            SceneObject s = GameManager.GetLevel().getCurrentScene();

            WorldCamera = (Camera3DObject)s.FindObject(typeof(Camera3DObject));

            if (WorldCamera == null)
                s.ParentLevel.AddObject(WorldCamera = new Camera3DObject());

            WorldCamera.RemoveTag(GameObjectTag.Update);

            foreach (CameraFlybyCenter center in s.Enumerate(typeof(CameraFlybyCenter)))
            {
                if (!SortedCenters.ContainsKey(center.CenterOrder.get()))
                {
                    SortedCenters.Add(center.CenterOrder.get(), center);
                    CenterCount = CenterCount > center.CenterOrder.get() ? CenterCount : center.CenterOrder.get();
                }
            }

            foreach (CameraFlybyNode node in s.Enumerate(typeof(CameraFlybyNode)))
            {
                if (!SortedNodes.ContainsKey(node.NodeOrder.get()))
                    SortedNodes[node.NodeOrder.get()] = new List<CameraFlybyNode>();
                SortedNodes[node.NodeOrder.get()].Add(node);
                NodeCount = NodeCount > node.NodeOrder.get() ? NodeCount : node.NodeOrder.get();
            }

            int i = 0;
            while (LastNode == null)
            {
                LastNode = GetNode(i);
                i++;

                if (i > NodeCount)
                    return;
            }

            while (NextNode == null)
            {
                NextNode = GetNode(i);
                i++;

                if (i > NodeCount)
                    return;
            }
            
            base.Enter();
        }

        CameraFlybyNode GetNode(int Index)
        {
            return SortedNodes.ContainsKey(Index) ? SortedNodes[Index][Rand.r.Next(SortedNodes[Index].Count)] : null;
        }

        public override void Update(GameTime gameTime)
        {
            FadeManager.SetFadeColor(Vector4.Zero);
            int PreviousBase = (int)NodesProgress;
            NodesProgress += gameTime.ElapsedGameTime.Milliseconds * ProgressSpeed * 60 / 1000;

            if ((int)NodesProgress > PreviousBase)
            {
                CurrentCenter++;
                while (!SortedCenters.ContainsKey(CurrentCenter))
                {
                    CurrentCenter++;
                    if (CurrentCenter > CenterCount)
                    {
                        CurrentCenter = 0;
                        if (CenterCount == -1)
                            return;
                    }
                }
                
                LastNode = GetNode((int)NodesProgress);
                NodesProgress += 1;

                while (LastNode == null && NodesProgress <= NodeCount)
                {
                    LastNode = GetNode((int)NodesProgress);
                    NodesProgress += 1;
                }

                NextNode = GetNode((int)NodesProgress);

                while (NextNode == null && NodesProgress <= NodeCount)
                {
                    NextNode = GetNode((int)NodesProgress);
                    NodesProgress += 1;
                }
            }

            if (NextNode != null && LastNode != null)
            {
                Vector3 LastPosition = new Vector3(LastNode.Position.X(), LastNode.Z.get(), LastNode.Position.Y());
                Vector3 NextPosition = new Vector3(NextNode.Position.X(), NextNode.Z.get(), NextNode.Position.Y());
                Vector3 CenterPosition = new Vector3(SortedCenters[CurrentCenter].Position.X(), SortedCenters[CurrentCenter].Z.get(), SortedCenters[CurrentCenter].Position.Y());
                Vector3 PlacePosition = LastPosition + (NextPosition - LastPosition) * (NodesProgress - (int)NodesProgress);
                WorldCamera.MyCamera.SetLookAt(PlacePosition, CenterPosition);

                float FadeA = (Math.Abs((NodesProgress - (int)NodesProgress) - 0.5f) - 0.25f) * 4;
                FadeManager.SetFadeColor(new Vector4(0, 0, 0, FadeA));
            }
            else
            {
                FadeManager.SetFadeColor(new Vector4(0, 0, 0, 1));
                WaveManager.SetState(FadeInState.self);
                FadeInState.SetTargetState(ChooseStartState.self);
            }

            base.Update(gameTime);
        }

        public override void Exit()
        {
            BlockPlayerWorldViewer = false;
            WorldCamera.RemoveTag(GameObjectTag.WorldViewer);
            GameManager.GetLevel().getCurrentScene().WorldViewerChildren.Remove(WorldCamera);
            foreach (PlayerShip p in GameManager.GetLevel().getCurrentScene().Enumerate(typeof(PlayerShip)))
                p.ReadyWorldViewer();

            base.Exit();
        }
    }
}
