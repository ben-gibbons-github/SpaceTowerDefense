using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BadRabbit.Carrot.AI
{
    public class DefendState : AiState
    {
        public MiningPlatform CurrentDefenseTarget;
        public MiningPlatform UrgentDefenseTarget;

        public Vector2 DefenseLocation;
        public UnitBasic AttackTarget;
        public UnitBasic DodgeTarget;
        public Bullet DodgeBullet;
        public Vector2 SmallBombLocation;
        bool Aiming = false;

        int SmallBombTimer = 1000;
        bool ShouldSmallBomb = false;

        int ResetTime = 0;
        int MaxResetTime = 5000;

        int SearchTime = 0;
        int MaxSearchTime = 200;

        public override void Enter(AiStateManager Parent)
        {
            ResetTime = 0;
            base.Enter(Parent);
            ResetDefenseTarget();
        }

        private void ResetDefenseTarget()
        {
            Dictionary<MiningPlatform, float> PlatformScores = new Dictionary<MiningPlatform, float>();

            foreach (UnitBasic u in FactionManager.SortedUnits[Parent.ParentController.ParentShip.GetTeam()])
                if (u.GetType().IsSubclassOf(typeof(MiningPlatform)))
                {
                    MiningPlatform m = (MiningPlatform)u;
                    if (!PlatformScores.ContainsKey(m))
                    {
                        PlatformScores.Add(m, 0);

                        foreach (UnitBasic u2 in FactionManager.SortedUnits[Parent.ParentController.ParentShip.GetTeam()])
                            if (u2.GetType().IsSubclassOf(typeof(UnitTurret)))
                            {
                                UnitTurret t2 = (UnitTurret)u2;
                                if (t2.IsAlly(Parent.ParentController.ParentShip) && !t2.Dead)
                                    PlatformScores[m] += Vector2.Distance(t2.Position.get(), m.Position.get());
                            }
                    }
                }
            
            MiningPlatform forwardPlatform = PathFindingManager.TraceToMiningPlatform(NeutralManager.GetSpawnPosition(),
                Parent.ParentController.ParentShip.GetTeam());

            if (forwardPlatform != null && PlatformScores.ContainsKey(forwardPlatform))
                PlatformScores[forwardPlatform] *= 2;

            float BestScore = 0;

            foreach (MiningPlatform m in PlatformScores.Keys)
                if (PlatformScores[m] > BestScore)
                {
                    CurrentDefenseTarget = m;
                    BestScore = PlatformScores[m];
                }

            DefenseLocation = PathFindingManager.TraceCellPoint(CurrentDefenseTarget.Position.get(), 10);
        }

        private void SearchForEnemies()
        {
            float BestDistance = 150;
            DodgeBullet = null;
            foreach (Bullet b in Bullet.DodgeBullets)
                if (!Parent.ParentController.ParentShip.IsAlly(b.ParentUnit))
                {
                    float d = Logic.DistanceLineSegmentToPoint(b.Position.get(), b.Position.get() + b.Speed * 8,
                        Parent.ParentController.ParentShip.Position.get());
                    if (d < 100)
                    {
                        BestDistance = d;
                        DodgeBullet = b;
                    }
                }

            int EnemyCount = 0;
            Vector2 AveragePosition = Vector2.Zero;

            AttackTarget = null;
            DodgeTarget = null;

            BestDistance = 1000;
            bool MiningInDanger = false;
            int MiningBonus = 0;
            Vector2 position = UrgentDefenseTarget == null ? Parent.ParentController.ParentShip.Position.get() :
                UrgentDefenseTarget.Position.get();

            float BestDodgeDistance = 200;
            foreach (Basic2DObject o in Parent.ParentController.ParentShip.Parent2DScene.quadGrids.First.Value.Enumerate(
                position, new Vector2(BestDistance * 2)))
            {
                if (o.GetType().IsSubclassOf(typeof(PlayerShip)))
                {
                    PlayerShip p = (PlayerShip)o;
                    if (p.InvTime > 1000)
                        DodgeTarget = p;
                }
                else if (o.GetType().IsSubclassOf(typeof(UnitBasic)) && !o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                {
                    UnitBasic u = (UnitBasic)o;
                    float d = Vector2.Distance(position, o.Position.get()) / u.ThreatLevel;

                    if (o.GetType().Equals(typeof(PlayerShip)))
                        d /= 1.25f;

                    if (!Parent.ParentController.ParentShip.IsAlly(u))
                    {
                        EnemyCount += u.GetUnitWeight();
                        AveragePosition += o.Position.get() * u.GetUnitWeight();

                        if (d < BestDistance && u.CanBeTargeted())
                        {
                            BestDistance = d;
                            AttackTarget = u;
                        }
                        float d2 = Vector2.Distance(Parent.ParentController.ParentShip.Position.get(), o.Position.get());
                        if (d2 < BestDodgeDistance && !u.Dead)
                        {
                            BestDodgeDistance = d2;
                            DodgeTarget = u;
                        }
                    }
                }
                else if (o.GetType().IsSubclassOf(typeof(MiningPlatform)))
                {
                    MiningPlatform m = (MiningPlatform)o;
                    if (Parent.ParentController.ParentShip.IsAlly(m) && m.SizeMult > 0 && m.HullDamage > 0)
                    {
                        MiningBonus = (int)(16 * m.HullDamage / m.HullToughness);
                        EnemyCount += MiningBonus;
                        MiningInDanger = true;
                    }
                }
            }

            if (EnemyCount > 20 && SmallBombTimer < 0 && MiningInDanger)
            {
                EnemyCount = MiningInDanger ? MiningBonus : 0;
                AveragePosition /= EnemyCount;
                foreach (Basic2DObject o in Parent.ParentController.ParentShip.Parent2DScene.quadGrids.First.Value.Enumerate(
                Parent.ParentController.ParentShip.Position.get(), new Vector2(1000)))
                    if (o.GetType().IsSubclassOf(typeof(UnitBasic)) && !o.GetType().IsSubclassOf(typeof(UnitBuilding)))
                    {
                        UnitBasic u = (UnitBasic)o;
                        float d = Vector2.Distance(o.Position.get(), position);
                        if (o.GetType().Equals(typeof(PlayerShip)))
                            d /= 1.25f;
                        if (d < 400)
                            EnemyCount += u.GetUnitWeight();
                    }

                if (EnemyCount > 20)
                {
                    Aiming = false;
                    SmallBombLocation = AveragePosition;
                    SmallBombTimer = 1000;
                    ShouldSmallBomb = true;
                }
            }

        }

        public override Vector2 RightStick()
        {
            if (!ShouldSmallBomb)
            {
                if (AttackTarget != null)
                    return Vector2.Normalize(AttackTarget.Position.get() - Parent.ParentController.ParentShip.Position.get()) *
                        new Vector2(1, -1);
                else
                    return Vector2.Zero;
            }
            else
            {
                Aiming = true;
                return Vector2.Normalize(SmallBombLocation - Parent.ParentController.ParentShip.Position.get()) *
                    new Vector2(1, -1);
            }
        }

        public override Vector2 LeftStick()
        {
            if (DodgeBullet == null)
            {
                if (DodgeTarget == null)
                {
                    if (UrgentDefenseTarget == null || Vector2.Distance(Parent.ParentController.ParentShip.Position.get(), UrgentDefenseTarget.Position.get()) < 400)
                    {
                        Vector2 TargetPos = AttackTarget == null ? DefenseLocation : AttackTarget.Position.get();
                        if (Parent.ParentController.ParentShip.FreezeTime < 1)
                        {
                            if (Vector2.Distance(TargetPos, Parent.ParentController.ParentShip.Position.get()) > 500)
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
                        return Vector2.Normalize(UrgentDefenseTarget.Position.get() - Parent.ParentController.ParentShip.Position.get())
                            * new Vector2(1, -1);
                }
                else
                    return Vector2.Normalize(DodgeTarget.Position.get() - Parent.ParentController.ParentShip.Position.get())
                        * new Vector2(-1, 1);
            }
            else
                return Vector2.Normalize(DodgeBullet.Position.get() - Parent.ParentController.ParentShip.Position.get())
                    * new Vector2(-1, 1);
        }

        public override void Update(GameTime gameTime)
        {
            if (DodgeBullet != null && DodgeBullet.TimeAlive >= DodgeBullet.LifeTime)
                DodgeBullet = null;
            if (DodgeTarget != null && DodgeTarget.Dead)
                DodgeTarget = null;

            float MostDamage = 0;

            UrgentDefenseTarget = null;
            foreach (UnitBasic b in FactionManager.SortedUnits[Parent.ParentController.ParentShip.GetTeam()])
                if (b.GetType().IsSubclassOf(typeof(MiningPlatform)))
                {
                    MiningPlatform m = (MiningPlatform)b;
                    if (FactionManager.Factions[Parent.ParentController.ParentShip.GetTeam()].MiningPlatformCount == 1 &&
                        m.HullDamage > 0 && FactionManager.CanBuildMiningPlatform(Parent.ParentController.ParentShip.FactionNumber))
                    {
                        AiState s = Parent.GetExistingState(typeof(PlaceTurretState));
                        Parent.SetState(s == null ? new PlaceTurretState() : s);
                    }
                    else if (m.SizeMult > 1)
                    {
                        float Damage = Math.Min(m.ShieldToughness, m.ShieldDamage) +
                            Math.Min(m.HullToughness, m.HullDamage);
                        if (Damage > MostDamage)
                        {
                            MostDamage = Damage;
                            UrgentDefenseTarget = m;
                        }
                    }
                }


            SmallBombTimer -= gameTime.ElapsedGameTime.Milliseconds;

            SearchTime += gameTime.ElapsedGameTime.Milliseconds;
            if (SearchTime > MaxSearchTime || (AttackTarget != null && !AttackTarget.CanBeTargeted()))
            {
                SearchTime = 0;
                SearchForEnemies();
            }

            ResetTime += gameTime.ElapsedGameTime.Milliseconds;
            if (ResetTime > MaxResetTime)
            {
                ResetTime -= MaxResetTime;
                ResetDefenseTarget();
            }

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
               (AttackTarget != null && Vector2.Distance(Parent.ParentController.ParentShip.Position.get(), AttackTarget.Position.get()) > 500 &&
               PathFindingManager.CollisionLine(Parent.ParentController.ParentShip.Position.get(), AttackTarget.Position.get()));
        }

        public override bool RightTrigger()
        {
            if (ShouldSmallBomb && Aiming)
            {
                ShouldSmallBomb = false;
                return true;
            }

            return false;
        }
    }
}
