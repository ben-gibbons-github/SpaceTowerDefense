﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class NeutralSpawnCreator : CreatorBasic
    {
        public override void Create()
        {
            this.MyType = typeof(NeutralSpawn);
            this.Catagory = "StarShip";
            base.Create();
        }

        public override GameObject ReturnObject()
        {
            return new NeutralSpawn();
        }
    }
}
