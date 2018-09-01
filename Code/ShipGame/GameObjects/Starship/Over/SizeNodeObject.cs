using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class SizeNodeObject : GameObject
    {
        LinkedList<SizeNode> Nodes = new LinkedList<SizeNode>();

        protected SizeNode AddSizeNode(SizeNode Node)
        {
            Nodes.AddLast(Node);
            return Node;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (SizeNode node in Nodes)
                node.Update(gameTime);
            base.Update(gameTime);
        }
    }
}
