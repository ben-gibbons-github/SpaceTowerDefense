using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PathFindingNode : Basic2DObject
    {
        static LinkedList<PathFindingNode> AllNodes = new LinkedList<PathFindingNode>();

        public static void Rebuild()
        {
            foreach (PathFindingNode node in AllNodes)
                node.Destroy();

            AllNodes.Clear();
        }

        public static PathFindingNode GetBestNode(Vector2 Position)
        {
            foreach (PathFindingNode node in AllNodes)
            {
                if (!PathFindingManager.CollisionLine(Position, node.Position.get()))
                    return node.Get();
            }

            PathFindingNode Node = new PathFindingNode(Position);
            GameManager.GetLevel().AddObject(Node);
            return Node.Get();
        }

        PathFindingNode NextNode;
        int NodeValue;
        Vector2 ToPosition;

        public PathFindingNode(Vector2 Position)
        {
            this.ToPosition = Position;
        }

        public override void Create()
        {
            base.Create();

            AddTag(GameObjectTag._2DOverDraw);

            this.Size.set(new Vector2(32));
            this.Position.set(ToPosition);

            NodeValue = PathFindingManager.GetCellValue(this.Position.get());

            bool Found = false;
            LinkedListNode<PathFindingNode> NextNode = AllNodes.First;
            if (AllNodes.Count > 0)
            {
                do
                {
                    if (NextNode.Value.NodeValue < NodeValue)
                    {
                        AllNodes.AddBefore(NextNode, this);
                        Found = true;
                        break;
                    }

                    NextNode = NextNode.Next;
                }
                while (NextNode != null);
            }

            if (!Found)
                AllNodes.AddLast(this);
        }

        public PathFindingNode Get()
        {
            if (PathFindingManager.GetCellValue(Position.get()) != PathFindingManager.StartingCell)
            {
                if (NextNode == null)
                {
                    NextNode = new PathFindingNode(PathFindingManager.TraceCellPoint(Position.get(), 8));
                    GameManager.GetLevel().AddObject(NextNode);
                }
            }

            return this;
        }

        public PathFindingNode GetNext()
        {
            Get();
            return NextNode;
        }

        public override void Draw2D(GameObjectTag DrawTag)
        {
            return;
            Vector3 Position3 = Game1.graphicsDevice.Viewport.Project(
                   new Vector3(this.Position.X(), 0, this.Position.Y()), StarshipScene.CurrentCamera.ProjectionMatrix,
                   StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);

            Vector3 Size3 = Game1.graphicsDevice.Viewport.Project(
                new Vector3(this.Position.X() + this.Size.X(), 0, this.Position.Y()), StarshipScene.CurrentCamera.ProjectionMatrix,
                StarshipScene.CurrentCamera.ViewMatrix, Matrix.Identity);

            Vector2 Position = new Vector2(Position3.X, Position3.Y) - Render.CurrentView.Position;
            float Size = Vector2.Distance(Position, new Vector2(Size3.X, Size3.Y) - Render.CurrentView.Position) / 1.6f;

            Render.DrawSolidRect(Position - new Vector2(Size), Position + new Vector2(Size), Color.White);

            base.Draw2D(DrawTag);
            
        }
    }
}
