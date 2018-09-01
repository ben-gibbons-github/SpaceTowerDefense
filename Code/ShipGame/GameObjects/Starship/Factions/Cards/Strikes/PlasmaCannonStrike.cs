using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class PlasmaCannonStrike : StrikeCard
    {
        public PlasmaCannonStrike()
        {
            Name = "PlasmaCannon";
        }

        protected override void Strike(Vector2 Position)
        {
            Basic2DScene scene = (Basic2DScene)GameManager.GetLevel().getCurrentScene();

            foreach (UnitTurret t in scene.Enumerate(typeof(UnitTurret)))
                t.Destroy();
            /*
            float BulletExplosionDistance = 200;
            QuadGrid grid = scene.quadGrids.First.Value;

            for (int i = 0; i < 2; i++)
                foreach (Basic2DObject o in grid.Enumerate(Position, new Vector2(BulletExplosionDistance * 2)))
                    if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                    {
                        BasicShipGameObject s = (BasicShipGameObject)o;
                        float dist = Vector2.Distance(s.Position.get(), Position) - o.Size.X() / 2;

                        if (dist < BulletExplosionDistance && s.GetType().IsSubclassOf(typeof(UnitTurret)))
                        {
                            UnitTurret t = (UnitTurret)s;
                            t.Destroy();
                        }
                    }
            */
            base.Strike(Position);
        }
    }
}
