using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BadRabbit.Carrot;
using Microsoft.Xna.Framework;

namespace SpyGame
{
    public class Bullet : Basic3DObject
    {
        public float Damage = 0;
        public BasicSpyUnit Parent;
    }
}
