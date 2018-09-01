using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadRabbit.Carrot
{
    public class CrystalWall : UnitTurret
    {
        public static Dictionary<int, LinkedList<CrystalWall>> SortedWalls = new Dictionary<int, LinkedList<CrystalWall>>();
        static Color WallColor = new Color(0.75f, 0.75f, 1);
        static float GlowSize = 100;

        public LinkedList<CrystalWallConnection> ConnectedWalls = new LinkedList<CrystalWallConnection>();

        bool Committed = false;

        public CrystalWall(int FactionNumber)
            : base(FactionNumber)
        {
            DeathSound = "CrystalTurretExplode";
            DeathVolume = 0.5f;
            ShieldToughness = 40;
            HullToughness = 40;
            ShieldColor = ShieldInstancer.WhiteShield;
            Weakness = AttackType.Red;
            ThreatLevel = 0;

            MaxEngagementDistance = CrystalWallCard.EngagementDistance;
        }

        public override void Create()
        {
            base.Create();
            Size.set(new Vector2(CrystalWallCard.STurretSize));
        }

        public override void Update(GameTime gameTime)
        {
            ShutDownTime = 0;
            if (!Dead)
            {
                if (!Committed)
                {
                    Committed = true;

                    if (!SortedWalls.ContainsKey(FactionManager.Factions[FactionNumber].Team))
                        SortedWalls.Add(FactionManager.Factions[FactionNumber].Team, new LinkedList<CrystalWall>());
                    SortedWalls[FactionManager.Factions[FactionNumber].Team].AddLast(this);

                    foreach (CrystalWall n in SortedWalls[FactionManager.Factions[FactionNumber].Team])
                        if (Vector2.Distance(Position.get(), n.Position.get()) < MaxEngagementDistance)
                        {
                            ConnectedWalls.AddLast(new CrystalWallConnection(n, this));
                        }
                }

                foreach (CrystalWallConnection n in ConnectedWalls)
                    if (!n.wall.Dead)
                    {
                        CreateGlow(Position.get(), n.wall.Position.get());
                    }
            }

            base.Update(gameTime);
        }

        public override void Relocate()
        {
            return;
        }

        public override void Virus(int VirusTime)
        {
            return;
        }

        void CreateGlow(Vector2 To, Vector2 From)
        {
            ParticleManager.CreateParticle(new Vector3(From.X + (To.X - From.X) * 0.2f, 0, From.Y + (To.Y - From.Y) * 0.2f)
                , Vector3.Zero, WallColor, GlowSize, 1);
            ParticleManager.CreateParticle(new Vector3(From.X + (To.X - From.X) * 0.4f, 0, From.Y + (To.Y - From.Y) * 0.4f)
                , Vector3.Zero, WallColor, GlowSize, 1);
            ParticleManager.CreateParticle(new Vector3(From.X + (To.X - From.X) * 0.6f, 0, From.Y + (To.Y - From.Y) * 0.6f)
                , Vector3.Zero, WallColor, GlowSize, 1);
            ParticleManager.CreateParticle(new Vector3(From.X + (To.X - From.X) * 0.8f, 0, From.Y + (To.Y - From.Y) * 0.8f)
                , Vector3.Zero, WallColor, GlowSize, 1);
        }

        public override void Destroy()
        {
            SortedWalls[FactionManager.Factions[FactionNumber].Team].Remove(this);
            base.Destroy();
        }

        public override bool AllowInteract(PlayerShip p)
        {
            return false;
        }

        public override void EMP(BasicShipGameObject Damager, int Level)
        {
            return;
        }

        public override int GetIntType()
        {
            return InstanceManager.AlienTurretIndex + 4;
        }

        public override DrawItem getDrawItem()
        {
            return new DrawShip("Alien/Turret5");
        }

        public override void Damage(float damage, float pushTime, Vector2 pushSpeed, BasicShipGameObject Damager, AttackType attackType)
        {
            if (attackType == AttackType.Melee)
                damage /= 4f;

            base.Damage(damage, pushTime, pushSpeed, Damager, attackType);
        }
    }
}
