using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot
{
    public class WorldFlareSpawner : GameObject
    {
        public Vector3Value SpawnPosition;
        public ColorValue Color;
        public IntValue FlareCount;
        public FloatValue Distance;
        public FloatValue Speed;

        public IntValue MaxSparkTimer;
        int SparkTimer;

        public override void Create()
        {
            SpawnPosition = new Vector3Value("Spawn Position");
            Color = new ColorValue("Color");
            FlareCount = new IntValue("Flare Count", 100);
            Distance = new FloatValue("Distance", 2000);
            Speed = new FloatValue("Speed", 1);
            MaxSparkTimer = new IntValue("Spark Timer", 400);

            AddTag(GameObjectTag.Update);

            base.Create();
        }

        public override void Update(GameTime gameTime)
        {
            /*
            SparkTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (SparkTimer > MaxSparkTimer.get())
            {
                SparkTimer -= MaxSparkTimer.get();
                ParentLevel.AddObject(new PathfindingFlare(PathFindingManager.self, Rand.r.Next(PathFindingManager.self.CellsX.get()),
                    Rand.r.Next(PathFindingManager.self.CellsX.get()), 0, 0, Color.getAsColor()));
            }*/
            if (!WaveFSM.WaveStepState.WeaponsFree)
            {
                Basic2DScene Parent2DScene = (Basic2DScene)ParentScene;
                Vector2 p = NeutralManager.GetSpawnPosition();
                ParentLevel.AddObject(new PathfindingFlare(PathFindingManager.self,
                    (int)((p.X - Parent2DScene.MinBoundary.X()) / PathFindingManager.self.Divisor.X),
                    (int)((p.Y - Parent2DScene.MinBoundary.Y()) / PathFindingManager.self.Divisor.Y),
                    0, 0, Color.getAsColor()));
            }
            base.Update(gameTime);
        }

        public override void CreateInGame()
        {
            for (int i = 0; i < FlareCount.get(); i++)
                ParentLevel.AddObject(new WorldFlare(Distance.get(), SpawnPosition.get(), Color.getAsColor(), Speed.get()));
            base.CreateInGame();
        }
    }
}
