using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class Path3D : Basic3DObject
    {
#if EDITOR && WINDOWS
        private static Vector4 drawColor = new Vector4(0.25f, 1, 0.25f, 1);
        int LineCount = 0;
        VertexPositionColor[] LineVerteces;
        short[] LineIndicies;
        Vector3 PreviousPosition;
#endif

        public ObjectListValue Nodes;
        public FloatValue TravelSpeed;

        private GameObject[] PathObjects;
        private float[] PathPosition;
        private float[] PathSpeed;
        private int ObjectCount = 0;
        private int arySize = 0;


        public override void Create()
        {
            Nodes = new ObjectListValue("Nodes", typeof(Path3DNode));
            TravelSpeed = new FloatValue("Travel Speed", 1);

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                this.AddRightClickEvent("Add Node", AddNode);
                AddTag(GameObjectTag._3DForward);
                Nodes.ChangeEvent = NodeChange;
            }
#endif
            base.Create();
        }

        public override bool TriggerEvent(EventType Event, string[] args)
        {
            switch (Event)
            {
                case EventType.AddObject:
                    GameObject o = ParentLevel.FindObject(args[0]);
                    if (o != null && o.GetType().IsSubclassOf(typeof(Basic3DObject)))
                    {
                        Basic3DObject b = (Basic3DObject)o;

                        if (ObjectCount == 0)
                            AddTag(GameObjectTag.Update);

                        if (arySize == ObjectCount)
                        {
                            if (arySize == 0)
                            {
                                PathObjects = new GameObject[4];
                                PathSpeed = new float[4];
                                PathPosition = new float[4];
                            }
                            else
                            {
                                arySize *= 2;
                                GameObject[] NewPathObjects = new GameObject[arySize];
                                float[] NewPathSpeed = new float[arySize];
                                float[] NewPathPosition = new float[arySize];

                                for (int i = 0; i < ObjectCount; i++)
                                {
                                    NewPathObjects[i] = PathObjects[i];
                                    NewPathSpeed[i] = PathSpeed[i];
                                    NewPathPosition[i] = PathPosition[i];
                                }

                                PathObjects = NewPathObjects;
                                PathPosition = NewPathPosition;
                                NewPathSpeed = PathSpeed;
                            }
                        }

                        PathObjects[ObjectCount] = b;
                        PathPosition[ObjectCount] = Logic.ParseF(args[1]);
                        float f = Logic.ParseF(args[2]);
                        PathSpeed[ObjectCount] = f != 0 ? f : TravelSpeed.get();
                        ObjectCount++;
                    }
                    return true;
            }

            return base.TriggerEvent(Event, args);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Basic3DObject o in PathObjects)
            {

            }
            base.Update(gameTime);
        }

        private void AddNode(Button b)
        {
            Vector3 p = Position.get();
            if (Nodes.Value.Count > 0)
            {
                Path3DNode n = (Path3DNode)Nodes.Value.Last.Value;
                p = n.Position.get();
            }

            Path3DNode a;
            Nodes.add(Add(a = new Path3DNode()));
            a.Position.set(p);
        }

#if EDITOR && WINDOWS
        public override void ChangePosition()
        {
            base.ChangePosition();
            foreach (Path3DNode n in Nodes.Value)
                n.ApplyMove(Position.get() - PreviousPosition, false);
            PreviousPosition = Position.get();
        }

        private void NodeChange()
        {
            LineCount = Nodes.Value.Count - 1;
            if (LineCount < 0)
                LineCount = 0;

            LineVerteces = new VertexPositionColor[LineCount * 2];
            LineIndicies = new short[LineCount * 2];

            int i= 0;
            LinkedListNode<GameObject> n = Nodes.Value.First;
            if(n != null)
                while (n.Next != null)
                {
                    Path3DNode p = (Path3DNode)n.Value;
                    LineVerteces[i++] = new VertexPositionColor(p.GetPosition(), Color.White);
                    p = (Path3DNode)n.Next.Value;
                    LineVerteces[i++] = new VertexPositionColor(p.GetPosition(), Color.White);
                    n = n.Next;
                }

            for (i = 0; i < LineCount * 2; i++)
                LineIndicies[i] = (short)(i);
        }

        public override void Draw3D(Camera3D camera, GameObjectTag DrawTag)
        {
            if (LineCount < 1)
                return;


            int i = 0;
            LinkedListNode<GameObject> n = Nodes.Value.First;
            if (n != null)
                while (n.Next != null)
                {
                    Path3DNode p = (Path3DNode)n.Value;
                    LineVerteces[i++].Position = p.GetPosition();
                    p = (Path3DNode)n.Next.Value;
                    LineVerteces[i++].Position = p.GetPosition();
                    n = n.Next;
                }
            ShapeRenderer.Load();
            ShapeRenderer.ColorEffectHolder.SetFromCamera(camera);
            ShapeRenderer.ColorEffectHolder.SetWorld(Matrix.Identity);
            ShapeRenderer.ColorEffectHolder.Apply();
            ShapeRenderer.ColorEffectHolder.MyEffect.Parameters["ObjectColor"].SetValue(drawColor);
            Game1.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
            PrimitiveType.LineList, LineVerteces, 0, LineCount * 2, LineIndicies, 0, LineCount);
            base.Draw3D(camera, DrawTag);
        }
#endif
    }
}
