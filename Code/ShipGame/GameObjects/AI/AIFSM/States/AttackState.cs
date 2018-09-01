using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class AttackState : AiState
    {
        public UnitBasic AttackTarget;
        public Bullet DodgeBullet;
        public Vector2 PathfindingTarget;
        bool isPathfinding = false;

        int SearchTime = 0;
        int MaxSearchTime = 400;

        public override void Enter(AiStateManager Parent)
        {
            PathFindingManager.BuildAttackGrid();
            base.Enter(Parent);
        }
       
        private void SearchForEnemies()
        {
            DodgeBullet = null;
            float BestDistance = 150;
            foreach (Bullet b in Bullet.DodgeBullets)
                if (!Parent.ParentController.ParentShip.IsAlly(b.ParentUnit) && b.attackType != Parent.ParentController.ParentShip.Resistence)
            {
                float d = Logic.DistanceLineSegmentToPoint(b.Position.get(), b.Position.get() + b.Speed * 8,
                    Parent.ParentController.ParentShip.Position.get());
                if (d < 100)
                {
                    BestDistance = d;
                    DodgeBullet = b;
                }
            }

            BestDistance = 1500;
            foreach (Basic2DObject o in Parent.ParentController.ParentShip.Parent2DScene.quadGrids.First.Value.Enumerate(
                Parent.ParentController.ParentShip.Position.get(), new Vector2(BestDistance * 2)))
                if (o.GetType().IsSubclassOf(typeof(UnitBasic)))
                {
                    float d = Vector2.Distance(o.Position.get(), Parent.ParentController.ParentShip.Position.get());
                    if (d < BestDistance)
                    {
                        UnitBasic u = (UnitBasic)o;
                        if (!Parent.ParentController.ParentShip.IsAlly(u) && u.CanBeTargeted())
                        {
                            if (u.GetType().IsSubclassOf(typeof(UnitTurret)))
                            {
                                UnitTurret t = (UnitTurret)u;
                                if (t.MyCard != null && t.MyCard.StrongVs.Equals("Light"))
                                    continue;
                            }

                            BestDistance = d;
                            AttackTarget = u;
                        }
                    }
                }
        }

        public override Vector2 RightStick()
        {
            if (AttackTarget != null)
                return Vector2.Normalize(AttackTarget.Position.get() - Parent.ParentController.ParentShip.Position.get()) *
                    new Vector2(1, -1);
            else
                return Vector2.Zero;
        }

        public override Vector2 LeftStick()
        {
            if (DodgeBullet == null)
            {
                if (!isPathfinding)
                {
                    Vector2 TargetPos = (AttackTarget == null ? Vector2.Zero : AttackTarget.Position.get());

                    if (Parent.ParentController.ParentShip.Guns[0] != null)
                    {
                        if (Parent.ParentController.ParentShip.FreezeTime < 1)
                        {
                            if (Vector2.Distance(TargetPos, Parent.ParentController.ParentShip.Position.get()) >
                                Parent.ParentController.ParentShip.Guns[0].IdealEngagementDistance)
                                return Vector2.Normalize(TargetPos - Parent.ParentController.ParentShip.Position.get()) * new Vector2(1, -1);
                            else
                                return Logic.ToVector2(Logic.ToAngle(TargetPos - Parent.ParentController.ParentShip.Position.get()) +
                                    (float)Math.PI / 2.5f) * new Vector2(1, -1);
                        }
                        else
                            return Logic.ToVector2(Logic.ToAngle(TargetPos - Parent.ParentController.ParentShip.Position.get()) +
                                (float)Math.PI * 0.85f) * new Vector2(1, -1);
                    }
                    else
                        return Vector2.Zero;
                }
                else
                    return Vector2.Normalize(PathfindingTarget - Parent.ParentController.ParentShip.Position.get()) * new Vector2(1, -1);
            }
            else
                return Vector2.Normalize(DodgeBullet.Position.get() - Parent.ParentController.ParentShip.Position.get())
                    * new Vector2(-1, 1);
        }

        public override void Update(GameTime gameTime)
        {
            if (DodgeBullet != null && DodgeBullet.TimeAlive >= DodgeBullet.LifeTime)
                DodgeBullet = null;

            SearchTime += gameTime.ElapsedGameTime.Milliseconds;
            if (SearchTime > MaxSearchTime || (AttackTarget != null && !AttackTarget.CanBeTargeted()))
            {
                SearchTime = 0;
                SearchForEnemies();
            }

            if (AttackTarget == null || AttackTarget.GetType().IsSubclassOf(typeof(UnitTurret)))
            {
                UnitTurret u = (UnitTurret)AttackTarget;
                if (AttackTarget == null || (u.MyCard == null && NeutralManager.MyPattern.CurrentCard.Type.Equals("Heavy") ||
                    (u.MyCard != null && NeutralManager.MyPattern.CurrentCard.Type.Equals(u.MyCard.StrongVs))))
                {
                    if (!isPathfinding || Vector2.Distance(Parent.ParentController.ParentShip.Position.get(), PathfindingTarget) < 300)
                    {
                        isPathfinding = true;
                        PathfindingTarget = PathFindingManager.TraceAttackPoint(Parent.ParentController.ParentShip.Position.get(), 10);
                    }
                }
                else
                    isPathfinding = false;
            }
            else 
                isPathfinding = false;

            if (Parent.ParentController.ParentShip.Dead || !WaveFSM.WaveStepState.WeaponsFree)
            {
                AiState s = Parent.GetExistingState(typeof(PlaceTurretState));
                Parent.SetState(s == null ? new PlaceTurretState() : s);
            }

            base.Update(gameTime);
        }

        public override bool YButton()
        {
            if (Parent.ParentController.ParentShip.viewMode == ViewMode.Admiral)
                return true;
            return base.YButton();
        }

        public override bool AButton()
        {
            return Parent.ParentController.ParentShip.FreezeTime > -1 || DodgeBullet != null ||
                (AttackTarget != null && Vector2.Distance(Parent.ParentController.ParentShip.Position.get(), AttackTarget.Position.get()) > 300 &&
                PathFindingManager.CollisionLine(Parent.ParentController.ParentShip.Position.get(), AttackTarget.Position.get()) ||
                (isPathfinding && PathFindingManager.CollisionLine(Parent.ParentController.ParentShip.Position.get(), PathfindingTarget)));
        }

        public override bool RightTrigger()
        {
            if (Parent.ParentController.ParentShip.FreezeTime < 1 &&
                PathFindingManager.GetAttackValue(Parent.ParentController.ParentShip.Position.get()) >
                PathFindingManager.StartingCell - 50 && WaveManager.CurrentWave > 3 && FactionManager.UnitCount > 20)
                return true;

            return base.RightTrigger();
        }

        public override bool LeftTrigger()
        {
            if (!Parent.ParentController.ParentShip.Dead)
                return AttackTarget != null && Parent.ParentController.ParentShip.FreezeTime < 1 &&
                    AttackTarget.GetType().IsSubclassOf(typeof(UnitTurret));
            else
                return WaveManager.CurrentWave > 6;
        }
    }
}
