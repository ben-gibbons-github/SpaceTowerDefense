using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class TurretSpawn : Basic2DObject
    {
        public IntValue FactionNumber;

        public override void Create()
        {
            FactionNumber = new IntValue("Faction Number");
            base.Create();
        }

        public override void CreateInGame()
        {
            PlasmaTurret p = new PlasmaTurret(FactionNumber.get());
            ParentLevel.AddObject(p);
            p.SetPosition(Position.get());
            base.CreateInGame();
        }
    }
}
