using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class WallNode : Basic2DObject, WallItem
    {
        public StringValue NodeFile;
        public StringValue ConnectorFile;

        public Matrix WorldMatrix = Matrix.Identity;

        public WallChain ParentChain;
        public WallConnector wallConnector;


        public Matrix GetMatrix()
        {
            return WorldMatrix;
        }

        public string GetFname()
        {
            return NodeFile.get() != "" ? NodeFile.get() : ParentChain.NodeFile.get();
        }

        public override void Create()
        {
            NodeFile = new StringValue("Node File");
            ConnectorFile = new StringValue("Conector File");

            AddTag(GameObjectTag._2DSolid);

            base.Create();
            Position.ChangeEvent = CreateMatrix;
            Size.ChangeEvent = CreateMatrix;
        }

        public override void CreateInGame()
        {
#if WINDOWS && EDITOR
            if (!ParentLevel.LevelForEditing)
#endif
            {
                WallInstancer.AddChild(this);
                if (wallConnector != null)
                    WallInstancer.AddChild(wallConnector);
            }

            base.CreateInGame();
        }

        public override void Destroy()
        {
#if WINDOWS && EDITOR
            if (!ParentLevel.LevelForEditing)
#endif
                WallInstancer.RemoveChild(this);

            base.Destroy();
        }

        void CreateMatrix()
        {
            WorldMatrix = Matrix.CreateScale(Size.X()) * Matrix.CreateTranslation(new Vector3(Position.X(), 0, Position.Y()));
        }
    }
}
