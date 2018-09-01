using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class MedicTurret : UnitTurret
    {
        new public static void TurretPlaceEvent()
        {
            foreach (MedicTurret m in AllMedicTurrets)
                m.SearchForNearby();
        }

        static LinkedList<MedicTurret> AllMedicTurrets = new LinkedList<MedicTurret>();
        static int MedicSearchTime = 100;

        LinkedList<UnitTurret> TurretsInRange = new LinkedList<UnitTurret>();
        LinkedList<UnitTurret> TurretsIHaveHealed = new LinkedList<UnitTurret>();
        int SearchTime = 0;

        public MedicTurret(int FactionNumber)
            : base(FactionNumber)
        {
            ShieldToughness = 20;
            HullToughness = 20;
            MaxEngagementDistance = MedicTurretCard.EngagementDistance;
            MaxBuildTime = 5000;
            Resistence = AttackType.None;
            Weakness = AttackType.None;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(MedicTurretCard.STurretSize));
            AllMedicTurrets.AddLast(this);
            ShieldColor = new Color(0.75f, 0.75f, 0.75f);
            SearchForNearby();
        }

        private void SearchForNearby()
        {
            QuadGrid quad = Parent2DScene.quadGrids.First.Value;

            TurretsInRange.Clear();
            foreach (GameObject o in Parent2DScene.Children)
                if (o.GetType().IsSubclassOf(typeof(BasicShipGameObject)))
                {
                    BasicShipGameObject s = (BasicShipGameObject)o;
                    if (o != this && o.GetType().IsSubclassOf(typeof(UnitTurret)) && !o.GetType().Equals(typeof(MedicTurret)) && Vector2.Distance(Position.get(), s.Position.get()) < MaxEngagementDistance)
                        TurretsInRange.AddLast((UnitTurret)o);
                }
        }

        protected override void Upgrade()
        {
            HullToughness *= 5;
            ShieldToughness *= 5;
            TurretsIHaveHealed.Clear();
            base.Upgrade();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                bool PlaySound = false;

                SearchTime += gameTime.ElapsedGameTime.Milliseconds;
                if (SearchTime > MedicSearchTime)
                {
                    SearchTime -= MedicSearchTime;
                    SearchForNearby();
                    foreach (UnitTurret t in TurretsInRange)
                        if (!TurretsIHaveHealed.Contains(t))
                            if (t.Dead)
                            {
                                t.Lives = 1;
                                t.HullDamage = 0;
                                t.ShieldDamage = 0;
                                t.Rebuild();

                                t.IsCrushed = false;
                                if (!IsUpdgraded)
                                {
                                    ShouldDeathSound = false;
                                    BlowUp();
                                }
                                if (IsUpdgraded)
                                    TurretsIHaveHealed.AddLast(t);
                                if (!PlaySound)
                                {
                                    PlaySound = true;

                                    SoundManager.Play3DSound("MedicRevive",
                                        new Vector3(Position.X(), Y, Position.Y()), 0.5f, 1000, 2);
                                }
                            }
                }
            }
            base.Update(gameTime);
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override void NewWave()
        {
            SearchTime = -5000;
            base.NewWave();
        }

        public override void NewWaveEvent()
        {
            TurretsIHaveHealed.Clear();
            base.NewWaveEvent();
        }

        public override int GetIntType()
        {
            return InstanceManager.HumanUnitIndex + 4;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Human/Turret5");
        }

        public override void Destroy()
        {
            AllMedicTurrets.Remove(this);
            base.Destroy();
        }
    }
}
