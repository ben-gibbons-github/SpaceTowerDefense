using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class ForceTurretBullet : Bullet
    {
        static Color ParticleColor = new Color(0.5f, 0.5f, 1);
        static float AttackLineWidth = 100;

        public override void Update(GameTime gameTime)
        {
            Vector2 StartPosition = Position.get();
            Vector2 EndPosition = Position.get() + Vector2.Normalize(Speed) * SpearTurretCard.EngagementDistance;

            ParticleManager.CreateParticle(new Vector3(StartPosition.X, 0, StartPosition.Y), Vector3.Zero, ParticleColor, 300, 5);
            ParticleManager.CreateParticle(new Vector3(StartPosition.X, 0, StartPosition.Y), Vector3.Zero, ParticleColor, 300, 7);
            ParticleManager.CreateParticle(new Vector3(StartPosition.X, 0, StartPosition.Y), Vector3.Zero, ParticleColor, 300, 4);
            for (int i = 0; i < 100; i++)
                ParticleManager.CreateParticle(
                    new Vector3(StartPosition.X, 0, StartPosition.Y) + (new Vector3(EndPosition.X - StartPosition.X, 0, EndPosition.Y - StartPosition.Y) * i / 100),
                    Vector3.Zero, ParticleColor, 150, 5);

            ParticleManager.CreateParticle(new Vector3(EndPosition.X, 0, EndPosition.Y), Vector3.Zero, ParticleColor, 300, 5);
            ParticleManager.CreateParticle(new Vector3(EndPosition.X, 0, EndPosition.Y), Vector3.Zero, ParticleColor, 300, 7);
            ParticleManager.CreateParticle(new Vector3(EndPosition.X, 0, EndPosition.Y), Vector3.Zero, ParticleColor, 300, 4);

            QuadGrid quadGrid = Parent2DScene.quadGrids.First.Value;

            Vector2 UpperLeftCorner = Logic.Min(StartPosition, EndPosition) - new Vector2(200);
            Vector2 LowerRightCorner = Logic.Max(StartPosition, EndPosition) + new Vector2(200);

            QuadGridXMin = (int)((UpperLeftCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridXMax = (int)((LowerRightCorner.X - quadGrid.Min.X) / quadGrid.CellSize.X);
            QuadGridYMin = (int)((UpperLeftCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);
            QuadGridYMax = (int)((LowerRightCorner.Y - quadGrid.Min.Y) / quadGrid.CellSize.Y);

            if (QuadGridXMax > quadGrid.CellsX - 1)
                QuadGridXMax = quadGrid.CellsX - 1;
            if (QuadGridXMin > quadGrid.CellsX - 1)
                QuadGridXMin = quadGrid.CellsX - 1;
            if (QuadGridYMax > quadGrid.CellsY - 1)
                QuadGridYMax = quadGrid.CellsY - 1;
            if (QuadGridYMin > quadGrid.CellsY - 1)
                QuadGridYMin = quadGrid.CellsY - 1;
            if (QuadGridXMax < 0)
                QuadGridXMax = 0;
            if (QuadGridXMin < 0)
                QuadGridXMin = 0;
            if (QuadGridYMax < 0)
                QuadGridYMax = 0;
            if (QuadGridYMin < 0)
                QuadGridYMin = 0;

            foreach (Basic2DObject g in quadGrid.Enumerate(QuadGridXMin, QuadGridYMin, QuadGridXMax, QuadGridYMax))
                if (g.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)g;
                    if (!s.IsAlly(ParentUnit) && CheckCircle(s, StartPosition, EndPosition, AttackLineWidth))
                    {
                        s = s.ReturnCollision();
                        if (s != null)
                        {
                            if (s.TestTag(UnitTag.Human))
                            s.Damage(Damage, 5, EndPosition - StartPosition, ParentUnit, AttackType.Blue);
                            if (s.GetType().IsSubclassOf(typeof(UnitShip)))
                            {
                                UnitShip ship = (UnitShip)s;
                                UnitTurret t = (UnitTurret)ParentUnit;
                                ship.EMP(ParentUnit, t.IsUpdgraded ? 1 : 0);
                            }
                        }
                    }
                }

            Destroy();
        }

        public bool CheckCircle(BasicShipGameObject g, Vector2 StartPosition, Vector2 EndPosition, float LineWidth)
        {
            return Logic.DistanceLineSegmentToPoint(StartPosition, EndPosition, g.getPosition()) < (g.getSize().X + AttackLineWidth) / 2;
        }
    }
}
