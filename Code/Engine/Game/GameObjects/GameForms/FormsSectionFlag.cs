using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadRabbit.Carrot
{
    public class FormSectionFlag : Basic2DObject
    {
        IntValue PlayerIndex;

        public override void Create()
        {
            PlayerIndex = new IntValue("Player Index");
            base.Create();
        }
    }
}
