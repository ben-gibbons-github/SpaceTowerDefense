using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class MineLayer : UnitTurret
    {
        new static int MaxSearchTime = 100;

        public LinkedList<Mine> Mines = new LinkedList<Mine>();

        int SearchTime = 0;
        int MaxMines = 12;
        int MinesToAdd = 0;
        int MaxMineAddTime = 1000;
        int MineAddTime = 0;


        public MineLayer(int FactionNumber)
            : base(FactionNumber)
        {
            MaxEngagementDistance = MineLayerCard.EngagementDistance;
        }

        public override bool CanBeTargeted()
        {
            return false;
        }

        public override bool InstancerCommit()
        {
            return false;
        }

        private void ChangeMaxMines(int Count)
        {
            MaxMines = Count;
            MinesToAdd = Count - Mines.Count;
        }

        public override void NewWaveEvent()
        {
            MinesToAdd = MaxMines - Mines.Count;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                MineAddTime += gameTime.ElapsedGameTime.Milliseconds;
                if (MineAddTime > MaxMineAddTime)
                {
                    MinesToAdd++;
                    MineAddTime = 0;
 
                    SearchTime -= MaxSearchTime;

                    QuadGrid quad = Parent2DScene.quadGrids.First.Value;

                    foreach (Basic2DObject o in quad.Enumerate(Position.get(), new Vector2(MaxEngagementDistance * 2)))
                        if (o.GetType().IsSubclassOf(typeof(UnitBasic)))
                        {
                            UnitBasic s = (UnitBasic)o;
                            float d = Vector2.Distance(Position.get(), o.Position.get());
                            if (!s.IsAlly(this) && s.CanBeTargeted() && d < MaxEngagementDistance + o.Size.X() / 2)
                            {
                                if (Mines.Count > 0)
                                {
                                    float BestDistance = 10000;
                                    Mine BestMine = null;

                                    foreach (Mine m in Mines)
                                    {
                                        float MineDist = Vector2.Distance(m.Position.get(), o.Position.get());
                                        if (MineDist < BestDistance)
                                        {
                                            BestDistance = MineDist;
                                            BestMine = m;
                                        }
                                    }

                                    BestMine.SetAttackTarget(s);
                                    Mines.Remove(BestMine);

                                    SoundManager.Play3DSound("MineFieldTarget", new Vector3(Position.X(), Y, Position.Y()), 0.35f,
                                        1000, 2);
                                }
                                if (d < (Size.X() + o.Size.X()))
                                    MinesToAdd = 0;

                                break;
                            }
                        }
                }

                if (Mines.Count >= MaxMines)
                    MinesToAdd = 0;

                while (MinesToAdd > 0)
                {
                    Mine m = new Mine(this, FactionNumber);
                    ParentLevel.AddObject(m);
                    Mines.AddLast(m);

                    Vector2 toPosition = Position.get() + Rand.V2() * Size.X() / 2;

                    m.Position.set(toPosition);
                    MinesToAdd--;
                }
            }
            base.Update(gameTime);
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(MineLayerCard.STurretSize));
            RemoveTag(GameObjectTag._2DSolid);
            ChangeMaxMines(6);
        }

        public override void Destroy()
        {
            foreach (Mine m in Mines)
                m.Destroy();
            Mines.Clear();

            base.Destroy();
        }

        protected override void Upgrade()
        {
            ChangeMaxMines(18);
            MaxEngagementDistance *= 2f;
            MaxMineAddTime /= 2;
            base.Upgrade();
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }
    }
}
