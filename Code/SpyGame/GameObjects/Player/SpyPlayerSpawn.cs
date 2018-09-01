using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot.SpyGame
{
    public class SpyPlayerSpawn : Basic3DObject
    {
        public IntValue PlayerNumber;

        public override void Create()
        {
            PlayerNumber = new IntValue("Player Number");
            base.Create();
        }
    }
}
