using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class WallChain : GameObject
    {
#if EDITOR && WINDOWS
        static Texture2D NodeTexture;
        static Texture2D ConnectorTexture;
        static bool Loaded = false;
#endif

        public ObjectListValue Nodes;

        public StringValue NodeFile;
        public StringValue ConnectorFile;

        Vector2 MinPos;
        Vector2 MaxPos;

        public override void Create()
        {
            NodeFile = new StringValue("Node File");
            ConnectorFile = new StringValue("Conector File");

            Nodes = new ObjectListValue("Nodes", typeof(WallNode));
            Nodes.ChangeEvent = NodeChange;

            Load();

#if EDITOR && WINDOWS
            if (ParentLevel.LevelForEditing)
            {
                this.AddRightClickEvent("Add Node", AddNode);
                AddTag(GameObjectTag._2DForward);
            }
#endif

            AddTag(GameObjectTag.Update);

            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            NodeChange();
            base.Update(gameTime);
        }

        void NodeChange()
        {
            LinkedListNode<GameObject> CurrentNode = Nodes.Value.First;

            while (CurrentNode != null)
            {
                WallNode n = (WallNode)CurrentNode.Value;
                n.ParentChain = this;

                MinPos = Logic.Min(MinPos, n.Position.get());
                MaxPos = Logic.Max(MaxPos, n.Position.get());

                if (CurrentNode.Previous == null)
                    n.SetQuadGridPosition();

                if (CurrentNode.Next != null)
                {
                    WallNode n2 = (WallNode)CurrentNode.Next.Value;

                    if (n2.wallConnector == null)
                        n2.wallConnector = new WallConnector(n2);
                    n2.wallConnector.SetFrom(n.Position.get(), n2.Position.get(), Math.Min(n.Size.X(), n2.Size.X()));
                    n2.SetQuadGridPosition(Logic.Min(n.Position.get() - n.Size.get() / 2, n2.Position.get() - n2.Size.get() / 2), Logic.Max(n.Position.get() + n.Size.get() / 2, n2.Position.get() + n2.Size.get() / 2));
                }

                CurrentNode = CurrentNode.Next;
            }
        }

        new static void Load()
        {
#if EDITOR && WINDOWS
            if (!Loaded)
            {
                Loaded = true;
                NodeTexture = AssetManager.Load<Texture2D>("Textures/ShipGame/World/WallNodeIcon");
                ConnectorTexture = Render.BlankTexture;
            }
#endif
        }

        private void AddNode(Button b)
        {
            Vector2 p = Vector2.Zero;
            if (Nodes.Value.Count > 0)
            {
                WallNode n = (WallNode)Nodes.Value.Last.Value;
                p = n.Position.get();
            }

            WallNode a;
            Nodes.add(Add(a = new WallNode()));
            a.Position.set(p);
        }
#if EDITOR && WINDOWS
        public override void Draw2D(GameObjectTag DrawTag)
        {
            LinkedListNode<GameObject> Node = Nodes.Value.First;

            while (Node != null)
            {
                WallNode n = (WallNode)Node.Value;
                Render.DrawSprite(NodeTexture, n.Position, n.Size, n.Rotation);
                if (Node.Next != null)
                {
                    WallNode n2 = (WallNode)Node.Next.Value;
                    Render.DrawSquare(n.Position.get(), n2.Position.get(), (int)(n.Size.X() / 2), ConnectorTexture, Color.White);
                }
                Node = Node.Next;
            }


            base.Draw2D(DrawTag);
        }
#endif

        public void DrawFromMiniMap(Vector2 Position, float Size, Vector2 Min, Vector2 Max)
        {
            if (this.MinPos.X > Max.X || this.MinPos.Y > Max.Y || this.MaxPos.X < Min.X || this.MaxPos.Y < Min.Y)
                return;

            LinkedListNode<GameObject> Node = Nodes.Value.First;

            while (Node != null)
            {
                WallNode n = (WallNode)Node.Value;
                if (Node.Next != null)
                {
                    WallNode n2 = (WallNode)Node.Next.Value;

                    Vector2 MapPosition = (n.Position.get() - Min) /
                        (Max - Min) * Size + Position;
                    Vector2 MapPosition2 = (n2.Position.get() - Min) /
                        (Max - Min) * Size + Position;

                    Vector2 MPosition = Position + new Vector2(Size);

                    MapPosition = Logic.Clamp(MapPosition, Position, MPosition);
                    MapPosition2 = Logic.Clamp(MapPosition2, Position, MPosition);

                    Render.DrawLine(MapPosition, MapPosition2, Color.Gray);
                }
                Node = Node.Next;
            }
        }
    }
}
